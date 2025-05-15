using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Dialogue
{
    [CustomEditor(typeof(DialogueUIController))]
    public class DialogueUIControllerEditor : Editor
    {
        private SerializedProperty dialoguePanelProp;
        private SerializedProperty speakerNameTextProp;
        private SerializedProperty speakerImageProp;
        private SerializedProperty speakerImageContainerProp;
        private SerializedProperty dialogueTextProp;
        private SerializedProperty optionsPanelProp;
        private SerializedProperty optionButtonPrefabProp;
        private SerializedProperty continuePromptProp;
        private SerializedProperty textDisplaySpeedProp;
        private SerializedProperty delayAfterDialogueProp;
        private SerializedProperty dialogueAudioPlayerProp;
        
        private void OnEnable()
        {
            dialoguePanelProp = serializedObject.FindProperty("dialoguePanel");
            speakerNameTextProp = serializedObject.FindProperty("speakerNameText");
            speakerImageProp = serializedObject.FindProperty("speakerImage");
            speakerImageContainerProp = serializedObject.FindProperty("speakerImageContainer");
            dialogueTextProp = serializedObject.FindProperty("dialogueText");
            optionsPanelProp = serializedObject.FindProperty("optionsPanel");
            optionButtonPrefabProp = serializedObject.FindProperty("optionButtonPrefab");
            continuePromptProp = serializedObject.FindProperty("continuePrompt");
            textDisplaySpeedProp = serializedObject.FindProperty("textDisplaySpeed");
            delayAfterDialogueProp = serializedObject.FindProperty("delayAfterDialogue");
            dialogueAudioPlayerProp = serializedObject.FindProperty("dialogueAudioPlayer");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("UI References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(dialoguePanelProp);
            EditorGUILayout.PropertyField(speakerNameTextProp);
            EditorGUILayout.PropertyField(speakerImageProp);
            EditorGUILayout.PropertyField(speakerImageContainerProp);
            EditorGUILayout.PropertyField(dialogueTextProp);
            EditorGUILayout.PropertyField(optionsPanelProp);
            EditorGUILayout.PropertyField(optionButtonPrefabProp);
            EditorGUILayout.PropertyField(continuePromptProp);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(textDisplaySpeedProp, new GUIContent("Text Display Speed", "The time in seconds between each character being displayed"));
            EditorGUILayout.PropertyField(delayAfterDialogueProp, new GUIContent("Delay After Dialogue", "The time in seconds to wait before hiding dialogue panel"));
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(dialogueAudioPlayerProp);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
