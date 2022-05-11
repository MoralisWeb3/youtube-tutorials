/**
 *           Module: WalletSelectListEditor.cs
 *  Descriptiontion: Editor tool that forms a menu of wallet options giving the 
 *                   developer the means of choosing which wallet images should 
 *                   be offered to the gamer user. As there are over 100 
 *                   wallets with 3 images each defeind to Wallet Connect this 
 *                   greatly enhances the user experience by speeding up how 
 *                   quickly the wallet options are displayed.
 *                   
 *           Author: Moralis Web3 Technology AB, 559307-5988 - David B. Goodrich
 *  
 *  MIT License
 *  
 *  Copyright (c) 2021 Moralis Web3 Technology AB, 559307-5988
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using WalletConnectSharp.Unity.UI;

namespace Assets.Scripts.EditorTools
{
    [CustomEditor(typeof(ChooseWalletScreen))]
    [CanEditMultipleObjects]
    public class WalletSelectListEditor : Editor
    {
        Vector2 currentWalletViewPosition = Vector2.zero;
        ChooseWalletScreen value;

        void OnEnable()
        {
            value = (ChooseWalletScreen)target;
        }

        public override void OnInspectorGUI()
        {
            // Only load the wallets if they have not already been loaded.
            if (value.wallets == null || value.wallets.Length < 1)
            {
                value.wallets = ChooseWalletScreen.GetWalletNameList().ToArray();
            }

            serializedObject.Update();
           
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonGridTransform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loadingText"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WalletConnect"));

            EditorGUILayout.Separator();
            // Title
            EditorGUILayout.LabelField("Wallets to Display (iOS)");
            // Draw wallet options.
            ArrayGUI(serializedObject.FindProperty("wallets"));

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws wallet options for all wallets in the wallets list.
        /// </summary>
        /// <param name="property"></param>
        private void ArrayGUI(SerializedProperty property)
        {
            SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(arraySizeProp);

            currentWalletViewPosition = EditorGUILayout.BeginScrollView(currentWalletViewPosition, GUILayout.Height(250));
            EditorGUI.indentLevel++;
            
            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                SerializedProperty name = property.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
                SerializedProperty selected = property.GetArrayElementAtIndex(i).FindPropertyRelative("Selected");
                bool v = EditorGUILayout.Toggle(name.stringValue, selected.boolValue);
                value.wallets[i].Selected = v;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif
