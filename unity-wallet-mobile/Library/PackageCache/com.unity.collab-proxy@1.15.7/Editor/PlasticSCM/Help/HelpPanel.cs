using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Help
{
    internal class HelpPanel
    {
        internal Vector2 TextScroll;

        internal bool Visible { get; private set; }

        internal PlasticGui.Help.HelpImage Image
        {
            get { return mHelpImage; }
        }

        internal HelpData Data
        {
            get { return mHelpData; }
        }

        internal GUIContent GUIContent
        {
            get { return mHelpGUIContent; }
        }

        internal HelpPanel(EditorWindow window)
        {
            mWindow = window;
        }

        internal void Show(PlasticGui.Help.HelpImage helpImage, HelpData helpData)
        {
            ClearData();

            UpdateData(helpImage, helpData);

            Visible = true;

            mWindow.Repaint();
        }

        internal void Hide()
        {
            ClearData();

            Visible = false;

            mWindow.Repaint();
        }

        internal bool TryGetLinkAtChar(
            int charIndex,
            out HelpLink link)
        {
            link = null;

            FormattedHelpLink formattedLink = GetFormattedLinkAtChar(
                mFormattedLinks, charIndex);

            if (formattedLink == null)
                return false;

            link = formattedLink.Source;

            return !BuildFormattedHelp.IsLinkMetaChar(formattedLink, charIndex);
        }

        void ClearData()
        {
            mHelpImage = PlasticGui.Help.HelpImage.GenericBuho;
            mHelpData = null;
            mHelpGUIContent = null;
            mFormattedLinks = null;
        }

        void UpdateData(PlasticGui.Help.HelpImage helpImage, HelpData helpData)
        {
            mHelpImage = helpImage;
            mHelpData = helpData;

            string formattedHelpText;
            BuildFormattedHelp.ForData(
                mHelpData.CleanText,
                mHelpData.FormattedBlocks.ToArray(),
                mHelpData.Links.ToArray(),
                out formattedHelpText,
                out mFormattedLinks);

            mHelpGUIContent = new GUIContent(formattedHelpText);
        }

        static FormattedHelpLink GetFormattedLinkAtChar(
            List<FormattedHelpLink> formattedLinks, int charIndex)
        {
            for(int i = 0; i < formattedLinks.Count; i++)
            {
                FormattedHelpLink link = formattedLinks[i];

                if (link.Position <= charIndex &&
                    charIndex < link.Position + link.Length)
                    return link;

                if (charIndex <= link.Position + link.Length)
                    return null;
            }

            return null;
        }

        PlasticGui.Help.HelpImage mHelpImage;
        HelpData mHelpData;

        GUIContent mHelpGUIContent;
        List<FormattedHelpLink> mFormattedLinks;

        EditorWindow mWindow;
    }
}
