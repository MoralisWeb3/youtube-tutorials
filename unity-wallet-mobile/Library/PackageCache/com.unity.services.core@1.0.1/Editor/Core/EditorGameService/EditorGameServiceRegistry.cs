using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Base implementation of the <see cref="IEditorGameServiceRegistry"/>
    /// </summary>
    public sealed class EditorGameServiceRegistry : IEditorGameServiceRegistry
    {
        internal Dictionary<string, IEditorGameService> Services;
        IProjectStateRequest m_ProjectStateRequest;
        IProjectStateHelper m_ProjectStateHelper;
        IServiceFlagRequest m_ServiceFlagRequest;
        ProjectState? m_CachedProjectState;
        UserRoleHandler m_UserRoleHandler;

        internal UserRoleHandler UserRoleHandler => m_UserRoleHandler;

        /// <summary>
        /// Default constructor for the registry.
        /// </summary>
        internal EditorGameServiceRegistry(IProjectStateRequest projectStateRequest = null,
                                           IProjectStateHelper projectStateHelper = null,
                                           IServiceFlagRequest serviceFlagRequest = null,
                                           UserRoleHandler userRoleHandler = null)
        {
            Services = new Dictionary<string, IEditorGameService>();
            m_ProjectStateRequest = projectStateRequest;
            m_CachedProjectState = m_ProjectStateRequest?.GetProjectState();
            m_ProjectStateHelper = projectStateHelper;
            m_ServiceFlagRequest = serviceFlagRequest;
            m_UserRoleHandler = userRoleHandler;
        }

        /// <summary>
        /// Access to the editor game service registry
        /// </summary>
        public static EditorGameServiceRegistry Instance { get; internal set; } = new EditorGameServiceRegistry(
            new ProjectStateRequest(), new ProjectStateHelper(), new ServiceFlagRequest(), new UserRoleHandler());

        [InitializeOnLoadMethod]
        static void RegisterAllServices()
        {
            var types = TypeCache.GetTypesDerivedFrom<IEditorGameService>().ToList();

            foreach (var type in types)
            {
                if (TryGetServiceFromType(type, out var service))
                {
                    // Check to see if the service has already been added
                    // There is a use-case where this method would be called via the [InitializeOnLoadMethod] attribute
                    // but the domain has not actually reloaded
                    // That is an intended Unity feature, and as such this is a safety check.
                    if (!Instance.Services.ContainsKey(service.Identifier.GetKey()))
                    {
                        Instance.RegisterService(service);
                    }
                }
                else
                {
                    Debug.LogError($"Type `{type.FullName}` is not a valid service.");
                }
            }

            Instance.StartListeningToProjectStateChanges();
        }

        void StartListeningToProjectStateChanges()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged += VerifyIfProjectBindChanges;
            CloudProjectSettingsEventManager.instance.projectRefreshed += VerifyIfProjectBindChanges;
#endif
        }

        ~EditorGameServiceRegistry()
        {
            StopListeningToProjectStateChanges();
            m_UserRoleHandler?.Dispose();
        }

        void StopListeningToProjectStateChanges()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged -= VerifyIfProjectBindChanges;
            CloudProjectSettingsEventManager.instance.projectRefreshed -= VerifyIfProjectBindChanges;
#endif
        }

        void VerifyIfProjectBindChanges()
        {
            if (m_ProjectStateRequest != null && m_ProjectStateHelper != null)
            {
                var currentProjectState = m_ProjectStateRequest.GetProjectState();
                if (m_ProjectStateHelper.IsProjectOnlyPartiallyBound(currentProjectState))
                {
                    return;
                }
                if (m_ProjectStateHelper.IsProjectBeingUnbound(m_CachedProjectState, currentProjectState))
                {
                    UpdateServicesForProjectUnbinding();
                }
                else if (m_ProjectStateHelper.IsProjectBeingBound(m_CachedProjectState, currentProjectState))
                {
                    UpdateServicesForProjectBinding();
                }
                m_CachedProjectState = currentProjectState;
            }
        }

        void UpdateServicesForProjectBinding()
        {
            if (m_ServiceFlagRequest != null)
            {
                var asyncOperation = m_ServiceFlagRequest.FetchServiceFlags();
                asyncOperation.Completed += UpdateServiceActivation;
            }
        }

        void UpdateServiceActivation(IAsyncOperation<IServiceFlags> flagsAsyncOperation)
        {
            foreach (var service in Services)
            {
                var serviceFlagEnabler = service.Value.Enabler as EditorGameServiceFlagEnabler;
                if (serviceFlagEnabler != null)
                {
                    var serviceFlags = flagsAsyncOperation.Result;
                    if (serviceFlags.DoesFlagExist(serviceFlagEnabler.GetFlagName()))
                    {
                        if (serviceFlags.IsFlagActive(serviceFlagEnabler.GetFlagName()))
                        {
                            serviceFlagEnabler.Enable();
                        }
                        else
                        {
                            serviceFlagEnabler.Disable();
                        }
                    }
                }
            }
        }

        void UpdateServicesForProjectUnbinding()
        {
            foreach (var service in Services)
            {
                if (service.Value.Enabler != null)
                {
                    var serviceFlagEnabler = service.Value.Enabler as EditorGameServiceFlagEnabler;
                    if (serviceFlagEnabler != null)
                    {
                        serviceFlagEnabler.InternalDisableLocalSettings();
                    }
                    else
                    {
                        service.Value.Enabler.Disable();
                    }
                }
            }
        }

        internal static bool TryGetServiceFromType(Type type, out IEditorGameService service)
        {
            var output = false;
            service = null;

            try
            {
                service = (IEditorGameService)Activator.CreateInstance(type);
                output = IsIdentifierValid(service);
            }
            catch (MissingMethodException)
            {
                output = false;
            }

            if (!output)
            {
                service = null;
            }

            return output;
        }

        internal static bool IsIdentifierValid(IEditorGameService editorGameService)
        {
            if (editorGameService.Identifier == null)
            {
                throw new NoNullAllowedException($"The Identifier for the service {editorGameService.Name} is null. Cannot register the service.");
            }

            if (string.IsNullOrEmpty(editorGameService.Identifier.GetKey()))
            {
                throw new ArgumentException($"The Identifier key for the service {editorGameService.Name} is null or empty. Cannot register the service.");
            }

            return true;
        }

        internal void RegisterService(IEditorGameService editorGameService)
        {
            if (IsIdentifierValid(editorGameService))
            {
                if (Services.ContainsKey(editorGameService.Identifier.GetKey()))
                {
                    throw new DuplicateNameException($"The Identifier key {editorGameService.Identifier.GetKey()} already exists in the registry. Cannot register the service.");
                }
                else
                {
                    Services.Add(editorGameService.Identifier.GetKey(), editorGameService);
                }
            }
        }

        /// <summary>
        /// Get the instance of a <see cref="IEditorGameService"/> type.
        /// </summary>
        /// <param name="serviceIdentifier">The identifier for a service</param>
        /// <returns>Return the instance of the given <see cref="IEditorGameService"/> type if it has been registered.</returns>
        public IEditorGameService GetEditorGameService<TIdentifier>()
            where TIdentifier : struct, IEditorGameServiceIdentifier
        {
            IEditorGameService output = null;

            var serviceIdentifier = new TIdentifier();
            if (Services.ContainsKey(serviceIdentifier.GetKey()))
            {
                output = Services[serviceIdentifier.GetKey()];
            }

            return output;
        }
    }
}
