using UnityEditor;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Data container of the state of the project when events are forwarded to the <see cref="IEditorGameService"/>
    /// </summary>
    struct ProjectState
    {
        /// <inheritdoc cref="CloudProjectSettings.userId"/>
        public string UserId;
        /// <inheritdoc cref="CloudProjectSettings.userName"/>
        public string UserName;
        /// <inheritdoc cref="CloudProjectSettings.accessToken"/>
        public string AccessToken;
        /// <inheritdoc cref="CloudProjectSettings.projectId"/>
        public string ProjectId;
        /// <inheritdoc cref="CloudProjectSettings.projectName"/>
        public string ProjectName;
        /// <inheritdoc cref="CloudProjectSettings.organizationId"/>
        public string OrganizationId;
        /// <inheritdoc cref="CloudProjectSettings.organizationName"/>
        public string OrganizationName;
#if ENABLE_EDITOR_GAME_SERVICES
        /// <inheritdoc cref="CloudProjectSettings.CoppaCompliance"/>
        public CoppaCompliance CoppaCompliance;
#endif
        /// <inheritdoc cref="CloudProjectSettings.projectBound"/>
        public bool ProjectBound;
        public bool IsOnline;

#if ENABLE_EDITOR_GAME_SERVICES
        public ProjectState(string userId, string userName, string accessToken, string projectId, string projectName,
                            string organizationId, string organizationName, CoppaCompliance coppaCompliance, bool projectBound,
                            bool isOnline)
        {
            UserId = userId;
            UserName = userName;
            AccessToken = accessToken;
            ProjectId = projectId;
            ProjectName = projectName;
            OrganizationId = organizationId;
            OrganizationName = organizationName;
            ProjectBound = projectBound;
            CoppaCompliance = coppaCompliance;
            IsOnline = isOnline;
        }

#else
        public ProjectState(string userId, string userName, string accessToken, string projectId, string projectName,
                            string organizationId, string organizationName, bool projectBound, bool isOnline)
        {
            UserId = userId;
            UserName = userName;
            AccessToken = accessToken;
            ProjectId = projectId;
            ProjectName = projectName;
            OrganizationId = organizationId;
            OrganizationName = organizationName;
            ProjectBound = projectBound;
            IsOnline = isOnline;
        }

#endif


        public bool HasDiff(ProjectState projectStateObj)
        {
            return !(this.UserId.Equals(projectStateObj.UserId) &&
                this.UserName.Equals(projectStateObj.UserName) &&
                this.AccessToken.Equals(projectStateObj.AccessToken) &&
                this.ProjectId.Equals(projectStateObj.ProjectId) &&
                this.ProjectName.Equals(projectStateObj.ProjectName) &&
                this.OrganizationId.Equals(projectStateObj.OrganizationId) &&
                this.OrganizationName.Equals(projectStateObj.OrganizationName) &&
                this.ProjectBound.Equals(projectStateObj.ProjectBound) &&
#if ENABLE_EDITOR_GAME_SERVICES
                this.CoppaCompliance.Equals(projectStateObj.CoppaCompliance) &&
#endif
                this.IsOnline.Equals(projectStateObj.IsOnline));
        }
    }
}
