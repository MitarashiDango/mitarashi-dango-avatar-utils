using System.Collections.Generic;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent]
    public class HandSignControl : MonoBehaviour
    {
        [HideInInspector]
        public HandSignGroup leftHandSignGroup;

        [HideInInspector]
        public HandSignGroup rightHandSignGroup;

        [HideInInspector]
        public List<HandSign> additionalLeftHandSigns;

        [HideInInspector]
        public List<HandSign> additionalRightHandSigns;
    }
}
