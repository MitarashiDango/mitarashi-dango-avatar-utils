using System;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [Serializable]
    public class BlendShape
    {
        public string name;

        [Range(0, 100)]
        public float weight;

        public BlendShape()
        {
            name = "";
            weight = 0;
        }

        public BlendShape(string name, float weight)
        {
            this.name = name;
            this.weight = weight;
        }
    }
}