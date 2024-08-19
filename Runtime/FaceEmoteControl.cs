
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
    public class FaceEmoteControl : MonoBehaviour, IEditorOnly
    {
        [SerializeField, HideInInspector]
        public AnimationClip defaultFaceAnimationClip;

        [SerializeField, HideInInspector]
        public AnimationClip leftFistAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftFistRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip leftHandOpenAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftHandOpenRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip leftFingerPointAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftFingerPointRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip leftVictoryAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftVictoryRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip leftRockNRollAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftRockNRollRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip leftHandGunAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftHandGunRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip leftThumbsUpAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D leftThumbsUpRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightFistAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightFistRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightHandOpenAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightHandOpenRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightFingerPointAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightFingerPointRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightVictoryAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightVictoryRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightRockNRollAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightRockNRollRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightHandGunAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightHandGunRadialMenuIcon;

        [SerializeField, HideInInspector]
        public AnimationClip rightThumbsUpAnimationClip;

        [SerializeField, HideInInspector]
        public Texture2D rightThumbsUpRadialMenuIcon;

        [SerializeField, HideInInspector]
        public List<AnimationClip> additionalFaceAnimationClips = new List<AnimationClip>();

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