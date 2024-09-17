using System.Collections.Generic;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    public class MiscUtil
    {
        /// <summary>
        /// オブジェクトのヒエラルキー上でのパスを取得する
        /// </summary>
        /// <param name="targetObject">パス取得対象のオブジェクト</param>
        /// <param name="rootObject">パス取得時の基準とするオブジェクト（絶対パスを取得する場合、nullを指定する)</param>
        /// <returns></returns>
        public static string GetPathInHierarchy(GameObject targetObject, GameObject rootObject)
        {
            if (targetObject == null)
            {
                return null;
            }

            var objectNames = new List<string>();
            var currentTransform = targetObject.transform;
            var rootTransform = rootObject?.transform;

            while (true)
            {
                if (rootTransform != null && currentTransform == rootTransform)
                {
                    return string.Join("/", objectNames);
                }

                objectNames.Insert(0, currentTransform.name);

                if (currentTransform.parent == null)
                {
                    return string.Join("/", objectNames);
                }

                currentTransform = currentTransform.parent;
            }
        }
    }
}