using Unity.Services.Core.Editor;

namespace Unity.Cloud.Collaborate.EditorGameService
{
    /// <summary>
    /// Identifier for the Cloud Collab Service
    /// </summary>
    public struct CloudCollabServiceIdentifier : IEditorGameServiceIdentifier
    {
        /// <summary>
        /// Get the Identifier key for Cloud Collab
        /// </summary>
        /// <returns>The Identifier key</returns>
        public string GetKey() => "Collab";
    }
}
