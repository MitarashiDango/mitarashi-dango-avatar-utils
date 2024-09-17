using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CreateAssetMenu(fileName = "New Face Emote Gesture Group", menuName = "MitarashiDango AvatarUtils/Face Emote Gesture Group", order = 1)]
    public class FaceEmoteGestureGroup : ScriptableObject
    {
        public string groupName;

        public FaceEmote fist;

        public FaceEmote handOpen;

        public FaceEmote fingerPoint;

        public FaceEmote victory;

        public FaceEmote rockNRoll;

        public FaceEmote handGun;

        public FaceEmote thumbsUp;
    }
}
