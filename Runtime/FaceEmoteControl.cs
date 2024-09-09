
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent]
    public class FaceEmoteControl : MonoBehaviour, IEditorOnly
    {
        [HideInInspector]
        public Motion defaultFaceMotion;

        [HideInInspector]
        public FaceEmote leftFist;

        [HideInInspector]
        public FaceEmote leftHandOpen;

        [HideInInspector]
        public FaceEmote leftFingerPoint;

        [HideInInspector]
        public FaceEmote leftVictory;

        [HideInInspector]
        public FaceEmote leftRockNRoll;

        [HideInInspector]
        public FaceEmote leftHandGun;

        [HideInInspector]
        public FaceEmote leftThumbsUp;

        [HideInInspector]
        public FaceEmote rightFist;

        [HideInInspector]
        public FaceEmote rightHandOpen;

        [HideInInspector]
        public FaceEmote rightFingerPoint;

        [HideInInspector]
        public FaceEmote rightVictory;

        [HideInInspector]
        public FaceEmote rightRockNRoll;

        [HideInInspector]
        public FaceEmote rightHandGun;

        [HideInInspector]
        public FaceEmote rightThumbsUp;

        [HideInInspector]
        public List<FaceEmote> additionalFaceEmotes;

        public bool IsLeftGestureAvailable
        {
            get
            {
                return (leftFist != null && leftFist.motion != null)
                  || (leftHandOpen != null && leftHandOpen.motion != null)
                  || (leftFingerPoint != null && leftFingerPoint.motion != null)
                  || (leftVictory != null && leftVictory.motion != null)
                  || (leftRockNRoll != null && leftRockNRoll.motion != null)
                  || (leftHandGun != null && leftHandGun.motion != null)
                  || (leftThumbsUp != null && leftThumbsUp.motion != null);
            }
        }

        public bool IsRightGestureAvailable
        {
            get
            {
                return (rightFist != null && rightFist.motion != null)
                  || (rightHandOpen != null && rightHandOpen.motion != null)
                  || (rightFingerPoint != null && rightFingerPoint.motion != null)
                  || (rightVictory != null && rightVictory.motion != null)
                  || (rightRockNRoll != null && rightRockNRoll.motion != null)
                  || (rightHandGun != null && rightHandGun.motion != null)
                  || (rightThumbsUp != null && rightThumbsUp.motion != null);
            }
        }
    }
}
