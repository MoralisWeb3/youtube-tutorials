using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SkinningSerializerJSON : ISkinningSerializer
    {
        public bool CanDeserialize(string data)
        {
            bool result = true;
            try
            {
                JsonUtility.FromJson<SkinningCopyData>(data);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public SkinningCopyData Deserialize(string data)
        {
            return JsonUtility.FromJson<SkinningCopyData>(data);
        }

        public string Serialize(SkinningCopyData skinningData)
        {
            return JsonUtility.ToJson(skinningData);
        }
    }
}
