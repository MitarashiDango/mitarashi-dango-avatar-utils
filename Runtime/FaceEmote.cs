using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [Serializable]
    public class FaceEmote
    {
        [HideInInspector]
        public string faceEmoteName = "";

        [HideInInspector]
        public AnimationClip faceEmote;

        [HideInInspector]
        public Texture2D icon;
    }
#endif
}
