using UnityEditor;

namespace MitarashiDango.AvatarUtils
{
    public class AssetUtil
    {
        public static T LoadAssetAtGUID<T>(string guid) where T : UnityEngine.Object
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
