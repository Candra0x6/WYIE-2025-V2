using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Dialogue
{
    [CustomEditor(typeof(DialogueTrigger))]
    public class DialogueTriggerEditor : Editor
    {
        private SerializedProperty dialogueDataProp;
        private SerializedProperty interactionRadiusProp;
        private SerializedProperty playerLayerProp;
        private SerializedProperty dialogueIndicatorSpriteProp;
        private SerializedProperty dialogueIndicatorProp;
        private SerializedProperty onDialogueStartProp;
        private SerializedProperty onDialogueEndProp;
        
        private void OnEnable()
        {
            dialogueDataProp = serializedObject.FindProperty("dialogueData");
            interactionRadiusProp = serializedObject.FindProperty("interactionRadius");
            playerLayerProp = serializedObject.FindProperty("playerLayer");
            dialogueIndicatorSpriteProp = serializedObject.FindProperty("dialogueIndicatorSprite");
            dialogueIndicatorProp = serializedObject.FindProperty("dialogueIndicator");
            onDialogueStartProp = serializedObject.FindProperty("OnDialogueStart");
            onDialogueEndProp = serializedObject.FindProperty("OnDialogueEnd");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Dialogue Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(dialogueDataProp);
            
            DialogueData dialogueData = dialogueDataProp.objectReferenceValue as DialogueData;
            if (dialogueData != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField($"Contains {dialogueData.nodes.Count} dialogue nodes");
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.PropertyField(interactionRadiusProp);
            EditorGUILayout.PropertyField(playerLayerProp);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Visual Indicator", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(dialogueIndicatorSpriteProp);
            EditorGUILayout.PropertyField(dialogueIndicatorProp);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(onDialogueStartProp);
            EditorGUILayout.PropertyField(onDialogueEndProp);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnSceneGUI()
        {
            DialogueTrigger trigger = (DialogueTrigger)target;
            
            // Draw the interaction radius
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(trigger.transform.position, Vector3.forward, trigger.interactionRadius);
            
            // Allow the user to adjust the radius with handles
            EditorGUI.BeginChangeCheck();
            float newRadius = Handles.RadiusHandle(Quaternion.identity, trigger.transform.position, trigger.interactionRadius);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(trigger, "Change Dialogue Trigger Radius");
                trigger.interactionRadius = newRadius;
                EditorUtility.SetDirty(trigger);
            }
        }
    }
}
