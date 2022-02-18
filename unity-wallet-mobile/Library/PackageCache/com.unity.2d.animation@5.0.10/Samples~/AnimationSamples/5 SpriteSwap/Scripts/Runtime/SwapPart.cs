using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.UI;

namespace Unity.U2D.Animation.Sample
{
    [Serializable]
    public struct SwapOptionData
    {
        public Dropdown dropdown;
        public SpriteResolver spriteResolver;
        public string category;
    }

    public class SwapPart : MonoBehaviour
    {
        public SpriteLibraryAsset spriteLibraryAsset;
        public SwapOptionData[] swapOptionData;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var swapOption in swapOptionData)
            {
                swapOption.dropdown.ClearOptions();
                var labels = spriteLibraryAsset.GetCategoryLabelNames(swapOption.category);
                var dropDownOption = new List<Dropdown.OptionData>(labels.Count());
                foreach (var label in labels)
                {
                    dropDownOption.Add(new Dropdown.OptionData(label));
                }
                swapOption.dropdown.options = dropDownOption;
                swapOption.dropdown.onValueChanged.AddListener((x)=>
                {
                    swapOption.spriteResolver.SetCategoryAndLabel(swapOption.category, swapOption.dropdown.options[x].text);
                });
            }
        }
    }

}
