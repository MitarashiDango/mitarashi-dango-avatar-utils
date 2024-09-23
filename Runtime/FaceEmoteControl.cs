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
        public FaceEmoteGestureGroup leftFaceEmoteGestureGroup;

        [HideInInspector]
        public FaceEmoteGestureGroup rightFaceEmoteGestureGroup;

        [HideInInspector]
        public List<FaceEmoteGroup> faceEmoteGroups;

        public bool IsLeftGestureAvailable
        {
            get
            {
                return leftFaceEmoteGestureGroup?.fist?.motion != null
                    || leftFaceEmoteGestureGroup?.handOpen?.motion != null
                    || leftFaceEmoteGestureGroup?.fingerPoint?.motion != null
                    || leftFaceEmoteGestureGroup?.victory?.motion != null
                    || leftFaceEmoteGestureGroup?.rockNRoll?.motion != null
                    || leftFaceEmoteGestureGroup?.handGun?.motion != null
                    || leftFaceEmoteGestureGroup?.thumbsUp?.motion != null;
            }
        }

        public bool IsRightGestureAvailable
        {
            get
            {
                return rightFaceEmoteGestureGroup?.fist?.motion != null
                    || rightFaceEmoteGestureGroup?.handOpen?.motion != null
                    || rightFaceEmoteGestureGroup?.fingerPoint?.motion != null
                    || rightFaceEmoteGestureGroup?.victory?.motion != null
                    || rightFaceEmoteGestureGroup?.rockNRoll?.motion != null
                    || rightFaceEmoteGestureGroup?.handGun?.motion != null
                    || rightFaceEmoteGestureGroup?.thumbsUp?.motion != null;
            }
        }
    }
}
