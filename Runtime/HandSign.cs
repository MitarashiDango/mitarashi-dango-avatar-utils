using System;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [Serializable]
    public class HandSign
    {
        [HideInInspector]
        public string name = "";

        [HideInInspector]
        public Motion motion;

        [HideInInspector]
        public Texture2D icon;
    }
}
