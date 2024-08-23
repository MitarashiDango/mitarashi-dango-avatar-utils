using System;
using UnityEngine;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [Serializable]
    public class FaceEmote
    {
        [HideInInspector]
        public string name = "";

        [HideInInspector]
        public AnimationClip animationClip;

        [HideInInspector]
        public Texture2D icon;
    }
#endif
}
