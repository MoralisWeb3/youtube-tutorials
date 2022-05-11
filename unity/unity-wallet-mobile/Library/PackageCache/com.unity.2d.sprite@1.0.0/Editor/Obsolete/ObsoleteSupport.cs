using System;
using System.Linq;
using UnityEditor.Sprites;
using UnityEngine;

namespace UnityEditor.U2D.Sprites.Obsolete
{
    public static class ObsoleteSupport
    {
        [Obsolete("Sprite Packer is no longer supported. Consider switching to the new Sprite Altas System")]
        public static void ShowSpritePacker()
        {
            EditorWindow.GetWindow<PackerWindow>();
        }
    }
}
