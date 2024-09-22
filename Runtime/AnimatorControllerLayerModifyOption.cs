using System;

namespace MitarashiDango.AvatarUtils
{
    [Serializable]
    public class AnimatorControllerLayerModifyOption
    {
        public PlayableLayerType layerType;

        public string layerName;

        public bool removeLayer;
    }
}
