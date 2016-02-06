/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PathologicalGames;


// Only compile if not using Unity iPhone
[CustomEditor(typeof(PreRuntimePoolItem))]
public class PreRuntimePoolItemInspector : Editor
{
    public override void OnInspectorGUI()
	{
        var script = (PreRuntimePoolItem)target;

        EditorGUI.indentLevel = 0;
        PGEditorUtils.LookLikeControls();

        script.poolName = EditorGUILayout.TextField("Pool Name", script.poolName);
        script.prefabName = EditorGUILayout.TextField("Prefab Name", script.prefabName);
        script.despawnOnStart = EditorGUILayout.Toggle("Despawn On Start", script.despawnOnStart);
        script.doNotReparent = EditorGUILayout.Toggle("Do Not Reparent", script.doNotReparent);

        // Flag Unity to save the changes to to the prefab to disk
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}


