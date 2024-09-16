using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CreateAssetMenu(fileName = "New Face Emote Group", menuName = "MitarashiDangoAvatarUtils/FaceEmoteGroup", order = 1)]
    public class FaceEmoteGroup : ScriptableObject
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
