using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralShapeFactory))]
public class HexSphereEditor : Editor
{
    private ProceduralShapeFactory _factory;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button($"Generate Shape"))
        {
            switch (_factory.shape)
            {
                case ProceduralShapeFactory.Shape.Icosphere:
                    _factory.CreateNewIcosphereObject();
                    break;
                case ProceduralShapeFactory.Shape.Cube:
                    _factory.CreateNewCubeObject();
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnEnable() { _factory = (ProceduralShapeFactory)target; }
}