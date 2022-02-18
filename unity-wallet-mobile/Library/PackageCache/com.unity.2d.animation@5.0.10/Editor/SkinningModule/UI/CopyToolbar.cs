using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class CopyToolbar : Toolbar
    {
        public class CopyToolbarFactory : UxmlFactory<CopyToolbar, CopyToolbarUxmlTraits> {}
        public class CopyToolbarUxmlTraits : UxmlTraits {}

        public event Action onDoCopy = () => {};
        public event Action onDoPaste = () => {};

        public CopyToolbar()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/CopyToolbarStyle.uss"));
        }

        public void DoCopy()
        {
            onDoCopy();
        }

        public void DoPaste()
        {
            onDoPaste();
        }

        public void BindElements()
        {
            var copyButton = this.Q<Button>("Copy");
            copyButton.clickable.clicked += DoCopy;

            var pasteButton = this.Q<Button>("Paste");
            pasteButton.clickable.clicked += DoPaste;
        }

        public static CopyToolbar GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/CopyToolbar.uxml");
            var clone = visualTree.CloneTree().Q<CopyToolbar>("CopyToolbar");
            clone.BindElements();
            return clone;
        }
    }
}
