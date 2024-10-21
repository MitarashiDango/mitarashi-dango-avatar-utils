
using System.Collections.Generic;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CreateAssetMenu(fileName = "New BlendShape Set", menuName = "MitarashiDango AvatarUtils/BlendShape Set", order = 1)]
    public class BlendShapeSet : ScriptableObject
    {
        public List<BlendShape> blendShapes = new List<BlendShape>();

        public BlendShape Get(string name)
        {
            return blendShapes.Find(BlendShape => BlendShape.name == name);
        }

        public void Set(string name, float weight)
        {
            var index = blendShapes.FindIndex(blendShape => blendShape.name == name);
            if (index >= 0)
            {
                blendShapes[index].weight = weight;
            }
            else
            {
                blendShapes.Add(new BlendShape(name, weight));
            }
        }

        public void Delete(string name)
        {
            var index = blendShapes.FindIndex(blendShape => blendShape.name == name);
            if (index >= 0)
            {
                blendShapes.RemoveAt(index);
            }
        }

        public bool Exists(string name)
        {
            return blendShapes.Exists(blendShapes => blendShapes.name == name);
        }
    }
}