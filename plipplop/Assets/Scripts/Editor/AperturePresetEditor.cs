﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AperturePreset), true)]
[CanEditMultipleObjects]
public class AperturePresetEditor : Editor
{
    readonly float headerSeparatorHeight = 30f;
    float overrideColumnWidth = 60f;

    GUIStyle title;
    GUIStyle box;
    GUIStyle boxToggle;
    GUIStyle pressedControl;
    GUIStyle normalControl;
    GUIStyle centeredPressedControl;
    GUIStyle centeredNormalControl;
    GUIStyle toggle;

    Dictionary<string, InheritableProperties> inheritableProperties = new Dictionary<string, InheritableProperties>();

    class InheritableProperty
    {
        public bool inheritDefault = true;
        public string name;
    }

    class InheritableProperties : List<InheritableProperty>
    {
        public InheritableProperties(Dictionary<string, bool> overrides) { this.overrides = overrides; }

        public Dictionary<string, bool> overrides;

        public void Add(string propName)
        {
            if (Find(o=>o.name == propName) == null) {
                Add(new InheritableProperty() { 
                    name = propName,
                    inheritDefault = !overrides.ContainsKey(propName) || (overrides.ContainsKey(propName) && !overrides[propName])
                });
            }
        }
    }

    public override void OnInspectorGUI()
    {
        MakeStyles();
        LoadProperties();
        serializedObject.Update();
        bool isDefault = target.name == "DefaultAperture";

        EditorGUI.BeginDisabledGroup(isDefault);
        EditorGUILayout.BeginVertical(title);
        GUILayout.Label("default aperture".ToUpper(), title, GUILayout.Height(headerSeparatorHeight), GUILayout.ExpandWidth(true));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fallback"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUI.EndDisabledGroup();

        // Someone dragged me as my own fallback!
        if (serializedObject.FindProperty("fallback").objectReferenceValue == target) {
            serializedObject.FindProperty("fallback").objectReferenceValue = null;
        }

        // No fallback selected
        var fb = ((AperturePreset)target).fallback;
        if (!fb && !isDefault) {
            GUILayout.Label("Please pick a fallback for this aperture");
            serializedObject.ApplyModifiedProperties();
            return;
        }

        SerializedObject serializedDefault = null;
        if (fb) {
            serializedDefault = new SerializedObject(fb);
        }
        /*
        var currentOverrides = serializedObject.FindProperty("overrides").Copy();
        currentOverrides.Next(true); // Generic field
        currentOverrides.Next(true); // Array size
        currentOverrides.Next(true); // Advance to first element;

    */
        foreach (var category in inheritableProperties.Keys) {

            EditorGUILayout.BeginVertical(title);
            GUILayout.Label(category.ToUpper(), title, GUILayout.Height(headerSeparatorHeight), GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Override", EditorStyles.boldLabel, GUILayout.Width(overrideColumnWidth));
            EditorGUILayout.LabelField("Property", EditorStyles.boldLabel, GUILayout.ExpandWidth(true), GUILayout.MinWidth(Screen.width * 0.8f));
            EditorGUILayout.EndHorizontal();


            foreach (var ip in inheritableProperties[category]) {
                var defaultProperty = fb == null ? null : serializedDefault.FindProperty(ip.name);
                var property = serializedObject.FindProperty(ip.name);
                

                EditorGUILayout.BeginHorizontal();

                // Override Switch
                if (isDefault) ip.inheritDefault = false;
                //Debug.Log(currentOverrides);
                //currentOverrides.Next(false);

                if (ip.inheritDefault){
                    if (GUILayout.Button("AUTO", normalControl, GUILayout.Width(overrideColumnWidth))) {
                        ip.inheritDefault = false;
                    }
                }
                else{
                    if (GUILayout.Button("OVERRIDE", pressedControl, GUILayout.Width(overrideColumnWidth))) {
                        ip.inheritDefault = true;
                        serializedObject.CopyFromSerializedProperty(serializedDefault.FindProperty(ip.name));
                    }
                }

                inheritableProperties[category].overrides[ip.name] = !ip.inheritDefault;

                // Field
                EditorGUI.BeginDisabledGroup(ip.inheritDefault);
                EditorGUILayout.PropertyField(property, new GUIContent(property.displayName));

                
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        //serializedObject.FindProperty("overrides").objectReferenceValue = currentOverrides;

        serializedObject.ApplyModifiedProperties();
    }

    void LoadProperties()
    {
        string switches = "switches";
        string basicParameters = "basic parameters";
        string interpolations = "interpolations";
        string speedFX = "speed effects";
        string advanced = "advanced";

        var overrides = ((AperturePreset)target).overrides;

        if (!inheritableProperties.ContainsKey(switches)) inheritableProperties[switches] = new InheritableProperties(overrides);
        if (!inheritableProperties.ContainsKey(basicParameters)) inheritableProperties[basicParameters] = new InheritableProperties(overrides);
        if (!inheritableProperties.ContainsKey(interpolations)) inheritableProperties[interpolations] = new InheritableProperties(overrides);
        if (!inheritableProperties.ContainsKey(speedFX)) inheritableProperties[speedFX] = new InheritableProperties(overrides);
        if (!inheritableProperties.ContainsKey(advanced)) inheritableProperties[advanced] = new InheritableProperties(overrides);

        inheritableProperties[switches].Add("canBeControlled");

        inheritableProperties[basicParameters].Add("fieldOfView");
        inheritableProperties[basicParameters].Add("heightOffset");
        inheritableProperties[basicParameters].Add("additionalAngle");
        inheritableProperties[basicParameters].Add("distance");

        inheritableProperties[interpolations].Add("fovLerp");
        inheritableProperties[interpolations].Add("lateralFollowLerp");
        inheritableProperties[interpolations].Add("longitudinalFollowLerp");
        inheritableProperties[interpolations].Add("verticalFollowLerp");
        inheritableProperties[interpolations].Add("rotationSpeed");
        inheritableProperties[interpolations].Add("lookAtLerp");
        
        inheritableProperties[speedFX].Add("speedEffectMultiplier");
        inheritableProperties[speedFX].Add("catchUpSpeedMultiplier");
        inheritableProperties[speedFX].Add("angleIncrementOnSpeed");

        inheritableProperties[advanced].Add("maximumCatchUpSpeed");
        inheritableProperties[advanced].Add("cameraRotateAroundSpeed");
        inheritableProperties[advanced].Add("absoluteBoundaries");
        inheritableProperties[advanced].Add("constraintToTarget");
        inheritableProperties[advanced].Add("targetConstraintLocalOffset");
    }

    void MakeStyles()
    {
        title = new GUIStyle(GUI.skin.box);
        title.fontSize = 12;
        title.alignment = TextAnchor.MiddleCenter;
        title.normal.textColor = Color.white;

        box = new GUIStyle(GUI.skin.box);
        box.normal.textColor = Color.white;
        box.alignment = TextAnchor.MiddleCenter;

        boxToggle = new GUIStyle(box);
        boxToggle.active.background = GUI.skin.toggle.active.background;
        boxToggle.normal.background = GUI.skin.toggle.normal.background;

        toggle = new GUIStyle(GUI.skin.toggle);
        toggle.alignment = TextAnchor.MiddleCenter;

        normalControl = new GUIStyle(GUI.skin.button);
        normalControl.fontSize = 8;
        normalControl.normal.textColor = Color.Lerp(Color.white, Color.blue, 0.5f);
        normalControl.alignment = TextAnchor.MiddleCenter;

        pressedControl = new GUIStyle(GUI.skin.button);
        pressedControl.normal = GUI.skin.button.active;
        pressedControl.fontSize = 8;
        pressedControl.normal.textColor = Color.Lerp(Color.white, Color.green, 0.5f);
        pressedControl.alignment = TextAnchor.MiddleCenter;

        centeredNormalControl = new GUIStyle(normalControl);
        centeredNormalControl.fontStyle = FontStyle.Bold;
        centeredNormalControl.alignment = TextAnchor.MiddleCenter;

        centeredPressedControl = new GUIStyle(pressedControl);
        centeredPressedControl.fontStyle = FontStyle.Bold;
        centeredPressedControl.alignment = TextAnchor.MiddleCenter;


    }
}