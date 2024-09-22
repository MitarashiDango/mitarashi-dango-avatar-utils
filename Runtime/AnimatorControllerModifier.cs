using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace MitarashiDango.AvatarUtils
{
    [AddComponentMenu("MitarashiDango AvatarUtils/Animator Controller Modifier")]
    public class AnimatorControllerModifier : MonoBehaviour, IEditorOnly
    {
        public List<AnimatorControllerModifyOption> modifierOptions;
    }
}
