using System;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [Serializable]
    public class AnimatorControllerLayerModifyOption
    {
        public PlayableLayerType layerType;

        public string layerName;

        public bool removeLayer;
        public bool replaceToDummyLayer;

        public bool overwriteDefaultWeight;
        public float defaultWeight;

        public bool overwriteAvatarMask;
        public AvatarMask avatarMask;

        public bool overwriteBlendingMode;
#if UNITY_EDITOR
        public AnimatorLayerBlendingMode blendingMode;
#endif

        public bool overwriteIkPass;
        public bool ikPass;
    }
}
