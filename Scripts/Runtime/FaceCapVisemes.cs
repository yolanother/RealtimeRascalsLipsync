using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts;
using _Project.Scripts.Visemes;
using UnityEngine;

public class FaceCapVisemes : MonoBehaviour
{
    [SerializeField] private BlendShapeConfig blendShapeConfig;
    [SerializeField] private VisemeDefinition[] visemes;
    [SerializeField] private List<int> blendShapeMappings = new List<int>();

    public VisemeDefinition[] Visemes => visemes;
    public List<int> BlendShapeMappings => blendShapeMappings;
    public BlendShapeConfig BlendShapeConfig => blendShapeConfig;

    private List<SkinnedMeshRenderer> skinnedMeshes;
    public List<SkinnedMeshRenderer> SkinnedMeshes
    {
        get
        {
            if (null == skinnedMeshes || skinnedMeshes.Count == 0)
            {
                skinnedMeshes = new List<SkinnedMeshRenderer>();
                skinnedMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
            }

            return skinnedMeshes;
        }
    }
    
    public float this[int blendShapeIndex]
    {
        get
        {
            if (!blendShapeConfig) return 0;
            if (blendShapeIndex >= blendShapeConfig.blendShapes.Length) return 0;
            var blendShape = blendShapeConfig.blendShapes[blendShapeMappings[blendShapeIndex]];
            return VisemeToBlendShapeIndex.TryGetValue(blendShape, out var weights) && weights.Count > 0
                ? weights[0].Weight
                : 0;
        }
        set
        {
            if (blendShapeIndex >= blendShapeMappings.Count) return;
            
            var blendShape = blendShapeConfig.blendShapes[blendShapeMappings[blendShapeIndex]];
            if (VisemeToBlendShapeIndex.TryGetValue(blendShape, out var weights))
            {
                for (int i = 0; i < weights.Count; i++)
                {
                    weights[i].Weight = value;
                }
            }
        }
    }

    public class BlendShapeReference
    {
        public string name;
        public SkinnedMeshRenderer skinnedMesh;
        public int index;

        public float Weight
        {
            get => skinnedMesh.GetBlendShapeWeight(index);
            set => skinnedMesh.SetBlendShapeWeight(index, value);
        }
    }

    private List<BlendShapeReference> _blendShapeReferences;

    public List<BlendShapeReference> BlendShapeReferences
    {
        get
        {
            if (null == _blendShapeReferences || _visemeToBlendShapeIndex.Count == 0) RefreshVisemeIndex();

            return _blendShapeReferences;
        }
    }
    private Dictionary<string, List<BlendShapeReference>> _visemeToBlendShapeIndex;
    public Dictionary<string, List<BlendShapeReference>> VisemeToBlendShapeIndex
    {
        get
        {
            if (null == _visemeToBlendShapeIndex || _visemeToBlendShapeIndex.Count == 0) RefreshVisemeIndex();
 
            return _visemeToBlendShapeIndex;
        }
    }

    private void RefreshVisemeIndex()
    {
        _visemeToBlendShapeIndex = new Dictionary<string, List<BlendShapeReference>>();
        _blendShapeReferences = new List<BlendShapeReference>();
        foreach (var skinnedMesh in SkinnedMeshes)
        {
            var blendShapes = skinnedMesh.sharedMesh.blendShapeCount;
            for (var i = 0; i < blendShapes; i++)
            {
                var name = skinnedMesh.sharedMesh.GetBlendShapeName(i).Split(".").Last();
                
                var weight = skinnedMesh.GetBlendShapeWeight(i);
                if(!_visemeToBlendShapeIndex.TryGetValue(name, out var list))
                {
                    list = new List<BlendShapeReference>();
                    _visemeToBlendShapeIndex.Add(name, list);
                }

                var reference = new BlendShapeReference()
                {
                    name = name,
                    skinnedMesh = skinnedMesh,
                    index = i
                };
                list.Add(reference);
                _blendShapeReferences.Add(reference);
            }
        }
    }


    public void ResetFace()
    {
        for (int i = 0; i < BlendShapeReferences.Count; i++)
        {
            BlendShapeReferences[i].Weight = 0;
        }
    }

    public void LerpToViseme(VisemeDefinition viseme, float stepSize)
    {
        for (int i = 0; i < BlendShapeReferences.Count; i++)
        {
            var targetWeight = viseme[BlendShapeReferences[i].name];
            BlendShapeReferences[i].Weight = Mathf.Lerp(BlendShapeReferences[i].Weight, targetWeight, Time.deltaTime * stepSize);
            BlendShapeReferences[i].Weight = 0;
        }
    }
    
    public void SetViseme(VisemeDefinition viseme)
    {
        ResetFace();
        for (int i = 0; i < viseme.blendShapeWeights.Count; i++)
        {
            SetBlendshape(viseme.blendShapeWeights[i].name, viseme.blendShapeWeights[i].weight);
        }
    }

    public void SetBlendshape(BlendShapeWeight weight)
    {
        SetBlendshape(weight.name, weight.weight);
    }

    public void SetBlendshape(string name, float weight)
    {
        if(VisemeToBlendShapeIndex.TryGetValue(name, out var list))
        {
            foreach (var reference in list)
            {
                reference.Weight = weight;
            }
        }
    }

    public void SetBlendShapeWeight(int visemeToBlendTarget, float frameViseme)
    {
        if (visemeToBlendTarget >= blendShapeMappings.Count || visemeToBlendTarget < 0)
        {
            Debug.LogWarning($"Invalid viseme target: {visemeToBlendTarget}");
            return;
        }

        var blendShapeIndex = blendShapeMappings[visemeToBlendTarget];
        if (blendShapeIndex >= blendShapeMappings.Count || blendShapeIndex < 0)
        {
            Debug.LogWarning($"Invalid blend shape index: {blendShapeIndex}");
            return;
        }
        
        SetBlendshape(blendShapeConfig.blendShapes[blendShapeIndex], frameViseme);
    }
}

[Serializable]
public class BlendShapeWeight
{
    public string name;
    public float weight;
}
