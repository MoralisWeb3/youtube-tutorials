using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class DefaultPoseScope : IDisposable
    {
        private bool m_Disposed;
        private SkeletonCache m_Skeleton;
        private BonePose[] m_Pose;
        private bool m_DoRestorePose = false;
        private bool m_UseLocalPose;

        public DefaultPoseScope(SkeletonCache skeleton, bool useLocalPose = true)
        {
            Debug.Assert(skeleton != null);

            m_Skeleton = skeleton;
            m_UseLocalPose = useLocalPose;

            if (m_Skeleton.isPosePreview)
            {
                m_DoRestorePose = true;

                if(useLocalPose)
                    m_Pose = m_Skeleton.GetLocalPose();
                else
                    m_Pose = m_Skeleton.GetWorldPose();

                m_Skeleton.RestoreDefaultPose();
            }
        }

        ~DefaultPoseScope()
        {
            if (!m_Disposed)
                Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
        }

        public void Dispose()
        {
            if (m_Disposed)
                return;

            m_Disposed = true;
            if (m_Skeleton != null && m_DoRestorePose)
            {
                if(m_UseLocalPose)
                    m_Skeleton.SetLocalPose(m_Pose);
                else
                    m_Skeleton.SetWorldPose(m_Pose);
            }
        }
    }
}
