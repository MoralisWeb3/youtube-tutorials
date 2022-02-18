using Codice.Client.Commands.WkTree;
using Codice.CM.Common;

namespace Unity.PlasticSCM.Tests.Editor.Mock
{
    internal static class BuildWorkspaceTreeNode
    {
        internal static WorkspaceTreeNode Controlled()
        {
            return new WorkspaceTreeNode()
            {
                RevInfo = new RevisionInfo()
                {
                    ParentId = 156,
                }
            };
        }

        internal static WorkspaceTreeNode CheckedOut()
        {
            return new WorkspaceTreeNode()
            {
                RevInfo = new RevisionInfo()
                {
                    CheckedOut = true,
                    ParentId = 156,
                }
            };
        }

        internal static WorkspaceTreeNode Added()
        {
            return new WorkspaceTreeNode()
            {
                RevInfo = new RevisionInfo()
                {
                    CheckedOut = true,
                    ParentId = -1,
                }
            };
        }
    }
}
