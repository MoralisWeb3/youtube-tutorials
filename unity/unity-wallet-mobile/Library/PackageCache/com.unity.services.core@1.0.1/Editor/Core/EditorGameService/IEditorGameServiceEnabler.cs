using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// An interface to allow you to control the enablement state of a service.
    /// </summary>
    public interface IEditorGameServiceEnabler
    {
        /// <summary>
        /// Enables the service
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables service.
        /// </summary>
        void Disable();

        /// <summary>
        /// Gets the enablement status of the service
        /// </summary>
        /// <returns>The status of the service</returns>
        bool IsEnabled();
    }
}
