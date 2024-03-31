using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapDebugger))] public class MapDebuggerEditor : Editor {

    public override void OnInspectorGUI() {
        MapDebugger mapDebugger = (MapDebugger) target;
        if (DrawDefaultInspector() || GUILayout.Button("Generate")) mapDebugger.GenerateMap();
    }
}
