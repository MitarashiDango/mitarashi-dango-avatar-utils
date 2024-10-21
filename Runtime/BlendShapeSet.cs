using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CreateAssetMenu(fileName = "New BlendShape Set", menuName = "MitarashiDango AvatarUtils/BlendShape Set", order = 1)]
    public class BlendShapeSet : ScriptableObject
    {
        [SerializeField]
        private List<BlendShape> _blendShapes = new List<BlendShape>();

        public string[] GetKeys()
        {
            return _blendShapes.Select(blendShape => blendShape.name).ToArray();
        }

        public BlendShape Get(string name)
        {
            return _blendShapes.Find(BlendShape => BlendShape.name == name);
        }

        public void Set(string name, float weight)
        {
            var index = _blendShapes.FindIndex(blendShape => blendShape.name == name);
            if (index >= 0)
            {
                _blendShapes[index].weight = weight;
            }
            else
            {
                _blendShapes.Add(new BlendShape(name, weight));
            }
        }

        public void Delete(string name)
        {
            var index = _blendShapes.FindIndex(blendShape => blendShape.name == name);
            if (index >= 0)
            {
                _blendShapes.RemoveAt(index);
            }
        }

        public bool Exists(string name)
        {
            return _blendShapes.Exists(blendShapes => blendShapes.name == name);
        }
    }
}