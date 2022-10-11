using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Visemes
{
    [CreateAssetMenu(fileName = "Viseme", menuName = "Tools/Visemes/Viseme", order = 0)]
    public class VisemeDefinition : ScriptableObject
    {
        public BlendShapeConfig blendShapeConfig;
        public List<BlendShapeWeight> blendShapeWeights = new List<BlendShapeWeight>();

        private Dictionary<string, BlendShapeWeight> _blendShapeWeightsDictionary;

        public Dictionary<string, BlendShapeWeight> BlendShapeWeights
        {
            get
            {
                if (null == _blendShapeWeightsDictionary)
                {
                    _blendShapeWeightsDictionary = new Dictionary<string, BlendShapeWeight>();
                    foreach (var blendShapeWeight in blendShapeWeights)
                    {
                        _blendShapeWeightsDictionary.Add(blendShapeWeight.name, blendShapeWeight);
                    }
                }

                return _blendShapeWeightsDictionary;
            }
        }

        public void ResetWeightDictionary() => _blendShapeWeightsDictionary = null;
        
        public float this[string viseme]
        {
            get
            {
                return BlendShapeWeights.TryGetValue(viseme, out var blendShapeWeight) ? blendShapeWeight.weight : 0;
            }
            set
            {
                if (value > 0)
                {
                    if (!BlendShapeWeights.TryGetValue(viseme, out var blendShapeWeight))
                    {
                        blendShapeWeight = new BlendShapeWeight();
                        blendShapeWeight.name = viseme;
                        BlendShapeWeights[viseme] = blendShapeWeight;
                        blendShapeWeights.Add(blendShapeWeight);
                    }

                    blendShapeWeight.weight = value;
                }
                else if(BlendShapeWeights.TryGetValue(viseme, out var blendShapeWeight))
                {
                    BlendShapeWeights.Remove(viseme);
                    blendShapeWeights.Remove(blendShapeWeight);
                }
            }
        }

        public void ClearWeights()
        {
            ResetWeightDictionary();
            blendShapeWeights.Clear();
        }
    }
}