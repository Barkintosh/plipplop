﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NamedPrefabResource))]
[CustomPropertyDrawer(typeof(VisualEffect))]
[CustomPropertyDrawer(typeof(GameEffect))]
[CustomPropertyDrawer(typeof(SerializableEmotionVerb))]
[CustomPropertyDrawer(typeof(Emotions.BubbleSprite))]

public class NamedResourceDrawer : PropertyDrawer
{
    internal bool displayLabels = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var skippedProperties = new List<string>(){ "childCount"};

        EditorGUI.BeginProperty(position, label, property);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var countableProperty = property.Copy();
        countableProperty.Next(true);
        int count = 0;
        bool firstLoop = true;
        while (firstLoop || countableProperty.NextVisible(false)) {
            if (skippedProperties.Contains(countableProperty.name)) continue;
            if (countableProperty.depth != property.depth + 1) break;
            count++;
            firstLoop = false;
        }

        firstLoop = true;

        // Calculate rects
        var spaceBetween = 5f;
        var w = position.width - spaceBetween * 4f;
        var fractionalWidth = 1f/(count * (displayLabels ? 2f : 1f) + (displayLabels ? 1f : 0f));

        var elementRect = new Rect(position.x, position.y, fractionalWidth * w, position.height);
        var prop = property.Copy();
        prop.Next(true);
        while (firstLoop || prop.NextVisible(false)) {
            firstLoop = false;
            if (skippedProperties.Contains(prop.name)) continue;
            if (prop.depth != property.depth + 1) break;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            if (displayLabels) {
                EditorGUI.LabelField(elementRect, prop.displayName);
                elementRect = new Rect(elementRect.x + elementRect.width + spaceBetween, position.y, fractionalWidth * w, position.height);
            }
            EditorGUI.PropertyField(elementRect, prop, GUIContent.none, true);
            elementRect = new Rect(elementRect.x + elementRect.width + spaceBetween, position.y, fractionalWidth * w, position.height);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}