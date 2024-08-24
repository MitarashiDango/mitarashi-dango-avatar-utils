
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.FaceEmoteControl
{
    [DisallowMultipleComponent]
    public class FaceEmoteControl : MonoBehaviour, IEditorOnly
    {
        [HideInInspector]
        public AnimationClip defaultFaceAnimationClip;

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
                return (leftFist != null && leftFist.animationClip != null)
                  || (leftHandOpen != null && leftHandOpen.animationClip != null)
                  || (leftFingerPoint != null && leftFingerPoint.animationClip != null)
                  || (leftVictory != null && leftVictory.animationClip != null)
                  || (leftRockNRoll != null && leftRockNRoll.animationClip != null)
                  || (leftHandGun != null && leftHandGun.animationClip != null)
                  || (leftThumbsUp != null && leftThumbsUp.animationClip != null);
            }
        }

        public bool IsRightGestureAvailable
        {
            get
            {
                return (rightFist != null && rightFist.animationClip != null)
                  || (rightHandOpen != null && rightHandOpen.animationClip != null)
                  || (rightFingerPoint != null && rightFingerPoint.animationClip != null)
                  || (rightVictory != null && rightVictory.animationClip != null)
                  || (rightRockNRoll != null && rightRockNRoll.animationClip != null)
                  || (rightHandGun != null && rightHandGun.animationClip != null)
                  || (rightThumbsUp != null && rightThumbsUp.animationClip != null);
            }
        }
    }
}
