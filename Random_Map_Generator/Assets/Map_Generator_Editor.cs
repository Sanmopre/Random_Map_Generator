using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Map_Generator))]
public class Map_Generator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        Map_Generator mapGen = (Map_Generator)target;

        if (DrawDefaultInspector()) {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate")) {
            mapGen.DrawMapInEditor();
        }
    }
}
