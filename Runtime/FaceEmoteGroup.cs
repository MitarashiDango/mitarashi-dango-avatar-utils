using System.Collections.Generic;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CreateAssetMenu(fileName = "New Face Emote Group", menuName = "MitarashiDango AvatarUtils/Face Emote Group", order = 1)]
    public class FaceEmoteGroup : ScriptableObject
    {
        public string groupName;

        public List<FaceEmote> faceEmotes;
    }
}