using System;

using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class CooldownWindowDelayer
    {
        internal CooldownWindowDelayer(Action action, double cooldownSeconds)
        {
            mAction = action;
            mCooldownSeconds = cooldownSeconds;
        }

        internal void Ping()
        {
            if (mIsOnCooldown)
            {
                RefreshCooldown();
                return;
            }

            StartCooldown();
        }

        void RefreshCooldown()
        {
            mIsOnCooldown = true;

            mSecondsOnCooldown = mCooldownSeconds;
        }

        void StartCooldown()
        {
            mLastUpdateTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += OnUpdate;

            RefreshCooldown();
        }

        void EndCooldown()
        {
            EditorApplication.update -= OnUpdate;

            mIsOnCooldown = false;

            mAction();
        }

        void OnUpdate()
        {
            double updateTime = EditorApplication.timeSinceStartup;
            double deltaSeconds = updateTime - mLastUpdateTime;

            mSecondsOnCooldown -= deltaSeconds;

            if (mSecondsOnCooldown < 0)
                EndCooldown();

            mLastUpdateTime = updateTime;
        }

        readonly Action mAction;
        readonly double mCooldownSeconds;

        double mLastUpdateTime;
        bool mIsOnCooldown;
        double mSecondsOnCooldown;
    }
}
