using System;
using UnityEditor.Animations;
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
        public AnimatorLayerBlendingMode blendingMode;

        public bool overwriteIkPass;
        public bool ikPass;
    }
}
