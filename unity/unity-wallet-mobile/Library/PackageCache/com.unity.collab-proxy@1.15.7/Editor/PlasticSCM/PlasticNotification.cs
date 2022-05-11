using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal static class PlasticNotification
    {
        internal enum Status
        {
            None,
            IncomingChanges,
            Conflicts
        }

        internal static Texture GetIcon(Status status)
        {
            Images.Name iconName = Images.Name.IconPlasticView;
            if (status == Status.IncomingChanges)
            {
                iconName = Images.Name.IconPlasticNotifyIncoming;
            }
            else if (status == Status.Conflicts)
            {
                iconName = Images.Name.IconPlasticNotifyConflict;
            }
            return Images.GetImage(iconName);
        }
    }
}
