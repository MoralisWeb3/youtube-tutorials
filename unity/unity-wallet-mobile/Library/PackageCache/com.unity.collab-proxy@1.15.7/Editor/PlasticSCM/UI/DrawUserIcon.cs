using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawUserIcon
    {
        static internal void ForPendeingChangesTab(string commentText)
        {
            Rect rect = BuildUserIconAreaRect(commentText, 35f);

            GUI.DrawTexture(rect, GetIconTexture());
        }

        static Rect BuildUserIconAreaRect(string commentText, float sizeOfImage)
        {
            GUIStyle commentTextAreaStyle = UnityStyles.PendingChangesTab.CommentTextArea;

            Rect result = GUILayoutUtility.GetRect(sizeOfImage, sizeOfImage); // Needs to be a square
            result.x = commentTextAreaStyle.margin.left;

            return result;
        }

        static Texture2D GetIconTexture()
        {
            if (sUserIconImage == null)
                sUserIconImage = Images.GetImage(Images.Name.IconEmptyGravatar);

            return sUserIconImage;
        }

        static Texture2D sUserIconImage;
    }
}
