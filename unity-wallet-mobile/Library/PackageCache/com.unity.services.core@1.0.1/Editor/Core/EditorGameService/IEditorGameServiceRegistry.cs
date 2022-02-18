using System;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// A container to store all available <see cref="IEditorGameService"/>
    /// </summary>
    public interface IEditorGameServiceRegistry
    {
        /// <summary>
        /// Method to get a service with a specific identifier
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IEditorGameServiceIdentifier"/> to use when getting</typeparam>
        /// <returns>The <see cref="IEditorGameService"/>you are trying to get</returns>
        IEditorGameService GetEditorGameService<T>() where T : struct, IEditorGameServiceIdentifier;
    }
}
