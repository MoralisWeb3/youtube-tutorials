using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IPFS_Uploader
{
    public class SelectionPanel : MonoBehaviour
    {
        public Action<string, string, string, byte[]> UploadButtonPressed;

        [Header("UI Elements")]
        //[SerializeField] private RawImage image;
        [SerializeField] private Image image;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_InputField descriptionInput;
        [SerializeField] private Button uploadButton;
        
        //Control vars
        private string _imagePath;
        private byte[] _imageData;

        public async void SelectImage()
        {
            _imagePath = EditorUtility.OpenFilePanel("Select a PNG", "", "png");

            if (_imagePath.Length == 0)
            {
                Debug.Log("Image not selected");
                return;
            }
            
            Texture2D texture = new Texture2D(0, 0);
            _imageData = await File.ReadAllBytesAsync(_imagePath);

            if (_imageData == null)
            {
                Debug.Log("Failed to read image");
                return;   
            }
                
            var isLoaded = texture.LoadImage(_imageData);

            if (!isLoaded)
            {
                Debug.Log("Image not loaded");
                return;
            }
                
            Debug.Log("Image loaded successfully!");
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.preserveAspect = true;
        }

        public void OnUploadButtonPressed()
        {
            if (image.sprite == null || nameInput.text == string.Empty || descriptionInput.text == string.Empty)
            {
                Debug.Log("All fields (image, name and description) need to be filled");
                return;
            }
            
            UploadButtonPressed?.Invoke(nameInput.text, descriptionInput.text, _imagePath, _imageData);
            uploadButton.interactable = false;
        }

        public void ResetUploadButton()
        {
            uploadButton.interactable = true;
        }
    }   
}
