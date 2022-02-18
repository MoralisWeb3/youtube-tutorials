namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Interface used to identify services
    /// </summary>
    public interface IEditorGameServiceIdentifier
    {
        /// <summary>
        /// The key used to identify a service.
        /// Used when registering/fetching a service to/from the <see cref="IEditorGameServiceRegistry"/>
        /// </summary>
        /// <returns>The key for the service</returns>
        string GetKey();
    }
}
