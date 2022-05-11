namespace Unity.Services.Core.Editor
{
    interface IProjectStateHelper
    {
        bool IsProjectOnlyPartiallyBound(ProjectState projectState);

        bool IsProjectBeingBound(ProjectState? previousProjectState, ProjectState currentProjectState);

        bool IsProjectBeingUnbound(ProjectState? cachedProjectState, ProjectState currentProjectState);
    }
}
