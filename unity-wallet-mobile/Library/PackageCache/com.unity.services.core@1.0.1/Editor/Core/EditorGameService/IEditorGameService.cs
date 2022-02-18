using UnityEngine;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Base contract for all editor game services defined by Core.Editor package.
    /// </summary>
    public interface IEditorGameService
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The identifier of this service.
        /// It is used when registering/fetching the service to/from the <see cref="EditorGameServiceRegistry"/>
        /// </summary>
        IEditorGameServiceIdentifier Identifier { get; }

        /// <summary>
        /// If true, this service requires COPPACompliance to be defined
        /// </summary>
        bool RequiresCoppaCompliance { get; }

        /// <summary>
        /// If true, this service will provide access to a Dashboard
        /// </summary>
        bool HasDashboard { get; }

        /// <summary>
        /// The Url that is used to reach the dashboard for this service
        /// </summary>
        /// <returns>
        /// Return a formatted URL to this service's dashboard.
        /// </returns>
        string GetFormattedDashboardUrl();

        /// <summary>
        /// The implementation of the enabler required by this service.
        /// If no enabler is required, set to null.
        /// </summary>
        IEditorGameServiceEnabler Enabler { get; }
    }
}
