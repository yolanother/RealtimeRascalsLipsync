using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    [CreateAssetMenu(fileName = "BlendShapeconfig", menuName = "Tools/Blendshapes", order = 0)]
    [Serializable]
    public class BlendShapeConfig : ScriptableObject
    {
        [SerializeField] public string[] blendShapes;
        
        [Button]
        public void Load(GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            HashSet<string> addedShapes = new HashSet<string>();
            List<string> blendShapeList = new List<string>();
            foreach (var renderer in renderers)
            {
                var blendShapes = renderer.sharedMesh.blendShapeCount;
                for (var i = 0; i < blendShapes; i++)
                {
                    var name = renderer.sharedMesh.GetBlendShapeName(i).Split(".").Last();
                    var weight = renderer.GetBlendShapeWeight(i);
                    Debug.Log($"{name} {weight}");
                    if (!addedShapes.Contains(name))
                    {
                        addedShapes.Add(name);
                        blendShapeList.Add(name);
                    }
                }

                this.blendShapes = blendShapeList.ToArray();
            }
        }
    }
}