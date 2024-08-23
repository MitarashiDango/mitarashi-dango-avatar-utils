
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
    public class FaceEmoteControl : MonoBehaviour, IEditorOnly
    {
        [HideInInspector]
        public AnimationClip defaultFaceAnimationClip;

        [HideInInspector]
        public AnimationClip leftFistAnimationClip;

        [HideInInspector]
        public FaceEmote leftFist;

        [HideInInspector]
        public Texture2D leftFistRadialMenuIcon;

        [HideInInspector]
        public AnimationClip leftHandOpenAnimationClip;

        [HideInInspector]
        public FaceEmote leftHandOpen;

        [HideInInspector]
        public Texture2D leftHandOpenRadialMenuIcon;

        [HideInInspector]
        public AnimationClip leftFingerPointAnimationClip;

        [HideInInspector]
        public FaceEmote leftFingerPoint;

        [HideInInspector]
        public Texture2D leftFingerPointRadialMenuIcon;

        [HideInInspector]
        public AnimationClip leftVictoryAnimationClip;

        [HideInInspector]
        public Texture2D leftVictoryRadialMenuIcon;

        [HideInInspector]
        public AnimationClip leftRockNRollAnimationClip;

        [HideInInspector]
        public Texture2D leftRockNRollRadialMenuIcon;

        [HideInInspector]
        public AnimationClip leftHandGunAnimationClip;

        [HideInInspector]
        public Texture2D leftHandGunRadialMenuIcon;

        [HideInInspector]
        public AnimationClip leftThumbsUpAnimationClip;

        [HideInInspector]
        public Texture2D leftThumbsUpRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightFistAnimationClip;

        [HideInInspector]
        public Texture2D rightFistRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightHandOpenAnimationClip;

        [HideInInspector]
        public Texture2D rightHandOpenRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightFingerPointAnimationClip;

        [HideInInspector]
        public Texture2D rightFingerPointRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightVictoryAnimationClip;

        [HideInInspector]
        public Texture2D rightVictoryRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightRockNRollAnimationClip;

        [HideInInspector]
        public Texture2D rightRockNRollRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightHandGunAnimationClip;

        [HideInInspector]
        public Texture2D rightHandGunRadialMenuIcon;

        [HideInInspector]
        public AnimationClip rightThumbsUpAnimationClip;

        [HideInInspector]
        public Texture2D rightThumbsUpRadialMenuIcon;

        [HideInInspector]
        public List<FaceEmote> additionalFaceEmotes;

        public bool IsLeftGestureAvailable
        {
            get
            {
                return leftFingerPointAnimationClip != null
                  || leftFistAnimationClip != null
                  || leftHandGunAnimationClip != null
                  || leftHandOpenAnimationClip != null
                  || leftRockNRollAnimationClip != null
                  || leftThumbsUpAnimationClip != null
                  || leftVictoryAnimationClip != null;
            }
        }

        public bool IsRightGestureAvailable
        {
            get
            {
                return rightFingerPointAnimationClip != null
                  || rightFistAnimationClip != null
                  || rightHandGunAnimationClip != null
                  || rightHandOpenAnimationClip != null
                  || rightRockNRollAnimationClip != null
                  || rightThumbsUpAnimationClip != null
                  || rightVictoryAnimationClip != null;
            }
        }
    }
#endif
}