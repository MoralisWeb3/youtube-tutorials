using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace WalletConnectSharp.Unity.Models
{
    public class NFTTokenData
    {
        public string name;
        public string image;
        public string description;

        private Sprite _cache;

        public Sprite imageSprite
        {
            get
            {
                if (_cache == null)
                {
                    throw new IOException("You must run `yield return token.DownloadImageSprite()` first");
                }

                return _cache;
            }
        }

        public IEnumerator DownloadImageSprite()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(image);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                throw new IOException(www.error);
            }
            else
            {
                Texture2D imageTexture = DownloadHandlerTexture.GetContent(www);

                Rect rect = new Rect(0, 0, imageTexture.width, imageTexture.height);
                Sprite imageSprite = Sprite.Create(imageTexture, rect, new Vector2(0.5f, 0.5f), 100);

                _cache = imageSprite;
            }
        }
    }
}