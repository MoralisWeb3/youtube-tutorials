using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.UI;

namespace Unity.U2D.Animation.Sample
{
    public class SwapFullSkin : MonoBehaviour
    {
        public SpriteLibraryAsset[] spriteLibraries;
        public SpriteLibrary spriteLibraryTarget;
        public Dropdown dropDownSelection;

        // Start is called before the first frame update
        void Start()
        {
            UpdateSelectionChoice();
        }

        void OnDropDownValueChanged(int value)
        {
            spriteLibraryTarget.spriteLibraryAsset = spriteLibraries[value];
        }

        internal void UpdateSelectionChoice()
        {
            dropDownSelection.ClearOptions();
            var options = new List<Dropdown.OptionData>(spriteLibraries.Length);
            for (int i = 0; i < spriteLibraries.Length; ++i)
            {
                options.Add(new Dropdown.OptionData(spriteLibraries[i].name));
            }
            dropDownSelection.options = options;
            dropDownSelection.onValueChanged.AddListener(OnDropDownValueChanged);
        }
    }

}
