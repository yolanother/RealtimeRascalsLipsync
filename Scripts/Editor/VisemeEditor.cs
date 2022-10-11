using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Visemes;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Scripts.Visemes.Editor
{
    public class VisemeEditor : EditorWindow
    {
        [SerializeField] private FaceCapVisemes target;
        [HideInInspector]
        [SerializeField] private int visemeIndex = -1;

        private Dictionary<string, int> weightIndex;
        private Vector2 scrollView;
        private bool blendshapeFoldout;
        private bool onlyVisemes;

        [MenuItem("Tools/Visemes/Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<VisemeEditor>();
            window.titleContent = new GUIContent("Viseme Editor");
            window.Show();
        }

        private void OnSelectionChange()
        {
            if (!Selection.activeGameObject) return;
            var faceController = Selection.activeGameObject.GetComponent<FaceCapVisemes>();
            if (faceController)
            {
                target = faceController;
                Repaint();
            }
        }

        private void OnGUI()
        {
            target = EditorGUILayout.ObjectField("Target", target, typeof(FaceCapVisemes), true) as FaceCapVisemes;
            if(!target)
            {
                GUILayout.Label("Select a target to use for previews");
                return;
            }
            if (!target.BlendShapeConfig)
            {
                EditorGUILayout.HelpBox($"No blendshape config found on {target.name}", MessageType.Info);
                return;
            }
            
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            blendshapeFoldout = EditorGUILayout.Foldout(blendshapeFoldout, "Blendshapes");
            GUILayout.FlexibleSpace();
            //onlyVisemes = EditorGUILayout.Toggle(onlyVisemes);
            if(blendshapeFoldout && GUILayout.Button("+", GUILayout.Width(24))) target.BlendShapeMappings.Add(0);
            GUILayout.EndHorizontal();

            var blendshapesNames = target.BlendShapeConfig.blendShapes;
            /*if (onlyVisemes)
            {
                blendshapesNames = target.BlendShapeConfig.blendShapes.Where(x => x.Length <= 3).ToArray();
            }*/
            
            if (blendshapeFoldout)
            {
                for (int i = 0; i < target.BlendShapeMappings.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    target.BlendShapeMappings[i] = EditorGUILayout.Popup(target.BlendShapeMappings[i],
                        blendshapesNames);
                    if (GUILayout.Button("-", GUILayout.Width(24)))
                    {
                        target.BlendShapeMappings.RemoveAt(i);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(4);
            GUILayout.EndVertical();
            if (target.Visemes.Length == 0)
            {
                EditorGUILayout.HelpBox($"No visemes found on {target.name}", MessageType.Info);
                return;
            }
            
            // get names from target.Visemes

            var index = EditorGUILayout.Popup(visemeIndex, target.Visemes.Select(v => v.name).ToArray());
            if(index != visemeIndex)
            {
                visemeIndex = index;
                target.SetViseme(target.Visemes[visemeIndex]);
            }
            var viseme = target.Visemes[visemeIndex];
            if (!viseme)
            {
                GUILayout.Label("Select a viseme to edit");
                return;
            }
            if (!viseme.blendShapeConfig)
            {
                GUILayout.Label("Viseme does not have a blend shape config.");
                return;
            }
            
            
            scrollView = EditorGUILayout.BeginScrollView(scrollView);
            

            if (GUILayout.Button("Reset"))
            {
                target.ResetFace();
                viseme.ClearWeights();
            }
            
            foreach (var blendShape in viseme.blendShapeConfig.blendShapes)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(blendShape);
                var weight = viseme[blendShape];
                var newWeight = EditorGUILayout.Slider(weight, 0, 100);
                if (Math.Abs(newWeight - weight) > .0001f)
                {
                    viseme[blendShape] = newWeight;
                    target.SetBlendshape(blendShape, newWeight);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}