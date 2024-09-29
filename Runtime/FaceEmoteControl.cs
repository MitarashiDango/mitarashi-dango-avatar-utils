using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MitarashiDango AvatarUtils/Face Emote Control")]
    public class FaceEmoteControl : MonoBehaviour, IEditorOnly
    {
        [HideInInspector]
        public Motion defaultFaceMotion;

        [HideInInspector]
        public float time = 0.1f;

        [HideInInspector]
        public List<FaceEmoteGestureGroup> faceEmoteGestureGroups;

        [HideInInspector]
        public int leftFaceEmoteGestureGroupNumber;

        [HideInInspector]
        public int rightFaceEmoteGestureGroupNumber;

        [HideInInspector]
        public List<FaceEmoteGroup> faceEmoteGroups;
    }
}
