using System;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [Serializable]
    public class HandSignGroup
    {
        [HideInInspector]
        public HandSign fist;

        [HideInInspector]
        public HandSign handOpen;

        [HideInInspector]
        public HandSign fingerPoint;

        [HideInInspector]
        public HandSign victory;

        [HideInInspector]
        public HandSign rockNRoll;

        [HideInInspector]
        public HandSign handGun;

        [HideInInspector]
        public HandSign thumbsUp;
    }
}
