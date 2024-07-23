// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonTeamsManagerEditor.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Custom inspector for PhotonTeamsManager
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEditor;

namespace Photon.Pun.UtilityScripts
{
    [CustomEditor(typeof(PhotonTeamsManager))]
    public class PhotonTeamsManagerEditor : Editor
    {
        private Dictionary<byte, bool> foldouts = new Dictionary<byte, bool>();
        private PhotonTeamsManager photonTeams;
        private SerializedProperty teamsListSp;
        private SerializedProperty listFoldIsOpenSp;

        private const string proSkinString =
            "iVBORw0KGgoAAAANSUhEUgAAAAgAAAAECAYAAACzzX7wAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACJJREFUeNpi/P//PwM+wHL06FG8KpgYCABGZWVlvCYABBgA7/sHvGw+cz8AAAAASUVORK5CYII=";
        private const string lightSkinString = "iVBORw0KGgoAAAANSUhEUgAAAAgAAAACCAIAAADq9gq6AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAABVJREFUeNpiVFZWZsAGmBhwAIAAAwAURgBt4C03ZwAAAABJRU5ErkJggg==";
        private const string removeTextureName = "removeButton_generated";
        private Texture removeTexture;

        private bool isOpen;

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnEnable()
        {
            photonTeams = target as PhotonTeamsManager;
            teamsListSp = serializedObject.FindProperty("teamsList");
            listFoldIsOpenSp = serializedObject.FindProperty("listFoldIsOpen");
            isOpen = listFoldIsOpenSp.boolValue;
            removeTexture = LoadTexture(removeTextureName, proSkinString, lightSkinString);
        }

        /// <summary>
        /// Read width and height if PNG file in pixels.
        /// </summary>
        /// <param name="imageData">PNG image data.</param>
        /// <param name="width">Width of image in pixels.</param>
        /// <param name="height">Height of image in pixels.</param>
        private static void GetImageSize( byte[] imageData, out int width, out int height )
        {
            width = ReadInt( imageData, 3 + 15 );
            height = ReadInt( imageData, 3 + 15 + 2 + 2 );
        }

        private static int ReadInt( byte[] imageData, int offset )
        {
            return ( imageData[ offset ] << 8 ) | imageData[ offset + 1 ];
        }

        private Texture LoadTexture(string textureName, string proSkin, string lightSkin)
        {
            string skin = EditorGUIUtility.isProSkin ? proSkin : lightSkin;
            // Get image data (PNG) from base64 encoded strings.
            byte[] imageData = Convert.FromBase64String( skin );
            // Gather image size from image data.
            int texWidth, texHeight;
            GetImageSize( imageData, out texWidth, out texHeight );
            // Generate texture asset.
            var tex = new Texture2D( texWidth, texHeight, TextureFormat.ARGB32, false, true );
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.name = textureName;
            tex.filterMode = FilterMode.Point;
            tex.LoadImage( imageData );
            return tex;
        }

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                DrawTeamsList();
                return;
            }
            PhotonTeam[] availableTeams = photonTeams.GetAvailableTeams();
            if (availableTeams != null)
            {
                EditorGUI.indentLevel++;
                foreach (var availableTeam in availableTeams)
                {
                    if (!foldouts.ContainsKey(availableTeam.Code))
                    {
                        foldouts[availableTeam.Code] = true;
                    }
                    Player[] teamMembers;
                    if (photonTeams.TryGetTeamMembers(availableTeam, out teamMembers) && teamMembers != null)
                    {
                        foldouts[availableTeam.Code] = EditorGUILayout.Foldout(foldouts[availableTeam.Code],
                            string.Format("{0} ({1})", availableTeam.Name, teamMembers.Length));
                    }
                    else
                    {
                        foldouts[availableTeam.Code] = EditorGUILayout.Foldout(foldouts[availableTeam.Code],
                            string.Format("{0} (0)", availableTeam.Name));
                    }
                    if (foldouts[availableTeam.Code] && teamMembers != null)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var player in teamMembers)
                        {
                            EditorGUILayout.LabelField(string.Empty, string.Format("{0} {1}", player, player.IsLocal ? " - You -" : string.Empty));
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawTeamsList()
        {
            GUILayout.Space(5);
            HashSet<byte> codes = new HashSet<byte>();
            HashSet<string> names = new HashSet<string>();
            for (int i = 0; i < teamsListSp.arraySize; i++)
            {
                SerializedProperty e = teamsListSp.GetArrayElementAtIndex(i);
                string name = e.FindPropertyRelative("Name").stringValue;
                byte code = (byte)e.FindPropertyRelative("Code").intValue;
                codes.Add(code);
                names.Add(name);
            }
            this.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            isOpen = PhotonGUI.ContainerHeaderFoldout(string.Format("Teams List ({0})", teamsListSp.arraySize), isOpen);
            if (EditorGUI.EndChangeCheck())
            {
                listFoldIsOpenSp.boolValue = isOpen;
            }
            if (isOpen)
            {
                const float containerElementHeight = 22;
                const float propertyHeight = 16;
                const float paddingRight = 29;
                const float paddingLeft = 5;
                const float spacingY = 3;
                float containerHeight = (teamsListSp.arraySize + 1) * containerElementHeight;
                Rect containerRect = PhotonGUI.ContainerBody(containerHeight);
                float propertyWidth = containerRect.width - paddingRight;
                float codePropertyWidth = propertyWidth / 5;
                float namePropertyWidth = 4 * propertyWidth / 5;
                Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin,
                    containerRect.width, containerElementHeight);
                Rect propertyPosition = new Rect(elementRect.xMin + paddingLeft, elementRect.yMin + spacingY,
                    codePropertyWidth, propertyHeight);
                EditorGUI.LabelField(propertyPosition, "Code");
                Rect secondPropertyPosition = new Rect(elementRect.xMin + paddingLeft + codePropertyWidth, elementRect.yMin + spacingY, 
                    namePropertyWidth, propertyHeight);
                EditorGUI.LabelField(secondPropertyPosition, "Name");
                for (int i = 0; i < teamsListSp.arraySize; ++i)
                {
                    elementRect = new Rect(containerRect.xMin, containerRect.yMin + containerElementHeight * (i + 1),
                        containerRect.width, containerElementHeight);
                    propertyPosition = new Rect(elementRect.xMin + paddingLeft, elementRect.yMin + spacingY,
                        codePropertyWidth, propertyHeight);
                    SerializedProperty teamElementSp = teamsListSp.GetArrayElementAtIndex(i);
                    SerializedProperty teamNameSp = teamElementSp.FindPropertyRelative("Name");
                    SerializedProperty teamCodeSp = teamElementSp.FindPropertyRelative("Code");
                    string oldName = teamNameSp.stringValue;
                    byte oldCode = (byte)teamCodeSp.intValue;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(propertyPosition, teamCodeSp, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        byte newCode = (byte)teamCodeSp.intValue;
                        if (codes.Contains(newCode))
                        {
                            Debug.LogWarningFormat("Team with the same code {0} already exists", newCode);
                            teamCodeSp.intValue = oldCode;
                        }
                    }
                    secondPropertyPosition = new Rect(elementRect.xMin + paddingLeft + codePropertyWidth, elementRect.yMin + spacingY, 
                        namePropertyWidth, propertyHeight);
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(secondPropertyPosition, teamNameSp, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        string newName = teamNameSp.stringValue;
                        if (string.IsNullOrEmpty(newName))
                        {
                            Debug.LogWarning("Team name cannot be null or empty");
                            teamNameSp.stringValue = oldName;
                        } 
                        else if (names.Contains(newName))
                        {
                            Debug.LogWarningFormat("Team with the same name \"{0}\" already exists", newName);
                            teamNameSp.stringValue = oldName;
                        }
                    }
                    Rect removeButtonRect = new Rect(
                        elementRect.xMax - PhotonGUI.DefaultRemoveButtonStyle.fixedWidth,
                        elementRect.yMin + 2,
                        PhotonGUI.DefaultRemoveButtonStyle.fixedWidth,
                        PhotonGUI.DefaultRemoveButtonStyle.fixedHeight);
                    if (GUI.Button(removeButtonRect, new GUIContent(removeTexture), PhotonGUI.DefaultRemoveButtonStyle))
                    {
                        teamsListSp.DeleteArrayElementAtIndex(i);
                    }
                    if (i < teamsListSp.arraySize - 1)
                    {
                        Rect texturePosition = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4,
                            1);
                        PhotonGUI.DrawSplitter(texturePosition);
                    }
                }
            }
            if (PhotonGUI.AddButton())
            {
                byte c = 0;
                while (codes.Contains(c) && c < byte.MaxValue)
                {
                    c++;
                }
                this.teamsListSp.arraySize++;
                SerializedProperty teamElementSp = this.teamsListSp.GetArrayElementAtIndex(teamsListSp.arraySize - 1);
                SerializedProperty teamNameSp = teamElementSp.FindPropertyRelative("Name");
                SerializedProperty teamCodeSp = teamElementSp.FindPropertyRelative("Code");
                teamCodeSp.intValue = c;
                string n = "New Team";
                int o = 1;
                while (names.Contains(n))
                {
                    n = string.Format("New Team {0}", o);
                    o++;
                }
                teamNameSp.stringValue = n;
            }
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}