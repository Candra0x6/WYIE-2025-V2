using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Quiz
{
    [CustomEditor(typeof(QuizDataJsonLoader))]
    public class QuizDataJsonLoaderEditor : Editor
    {
        SerializedProperty jsonQuizFileProperty;
        SerializedProperty targetQuizDataProperty;
        
        private void OnEnable()
        {
            jsonQuizFileProperty = serializedObject.FindProperty("jsonQuizFile");
            targetQuizDataProperty = serializedObject.FindProperty("targetQuizData");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(jsonQuizFileProperty);
            EditorGUILayout.PropertyField(targetQuizDataProperty);
            
            serializedObject.ApplyModifiedProperties();
            
            // Add button to load data
            if (GUILayout.Button("Load Quiz Data from JSON"))
            {
                QuizDataJsonLoader loader = (QuizDataJsonLoader)target;
                
                if (loader.jsonQuizFile == null)
                {
                    EditorUtility.DisplayDialog("Error", "Please assign a JSON file first.", "OK");
                    return;
                }
                
                if (loader.targetQuizData == null)
                {
                    EditorUtility.DisplayDialog("Error", "Please assign a target QuizData object first.", "OK");
                    return;
                }
                
                if (EditorUtility.DisplayDialog("Load Quiz Data", 
                    "This will overwrite the current data in the target QuizData asset. Continue?", 
                    "Yes", "No"))
                {
                    loader.LoadFromJson();
                    EditorUtility.SetDirty(loader.targetQuizData);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("Success", "Quiz data loaded from JSON file.", "OK");
                }
            }
        }
    }
}
