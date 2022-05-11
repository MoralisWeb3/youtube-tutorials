namespace UnityEditor.U2D.Animation
{
    internal static class SkinningCopyUtility
    {
        private static ISkinningSerializer s_SkinningSerializer = new SkinningSerializerJSON();
        public static ISkinningSerializer Serializer
        {
            get { return s_SkinningSerializer; }
            set { s_SkinningSerializer = value; }
        }

        public static string SerializeSkinningCopyDataToString(SkinningCopyData skinningData)
        {
            return s_SkinningSerializer.Serialize(skinningData);
        }

        public static bool CanDeserializeSystemCopyBufferToSkinningCopyData()
        {
            if (!string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
                return CanDeserializeStringToSkinningCopyData(EditorGUIUtility.systemCopyBuffer);
            return false;
        }

        public static bool CanDeserializeStringToSkinningCopyData(string data)
        {
            return s_SkinningSerializer.CanDeserialize(data);
        }

        public static SkinningCopyData DeserializeStringToSkinningCopyData(string data)
        {
            return s_SkinningSerializer.Deserialize(data);
        }
    }
}
