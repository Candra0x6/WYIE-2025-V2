using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Quiz
{
    [CustomEditor(typeof(QuizAttack))]
    public class QuizAttackEditor : Editor
    {
        SerializedProperty quizDataProperty;
        SerializedProperty detectionRadiusProperty;
        SerializedProperty playerLayerProperty;
        SerializedProperty quizCooldownProperty;
        SerializedProperty enemyDamageableProperty;
        SerializedProperty quizTriggerIndicatorProperty;
        
        private void OnEnable()
        {
            quizDataProperty = serializedObject.FindProperty("quizData");
            detectionRadiusProperty = serializedObject.FindProperty("detectionRadius");
            playerLayerProperty = serializedObject.FindProperty("playerLayer");
            quizCooldownProperty = serializedObject.FindProperty("quizCooldown");
            enemyDamageableProperty = serializedObject.FindProperty("enemyDamageable");
            quizTriggerIndicatorProperty = serializedObject.FindProperty("quizTriggerIndicator");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Quiz Attack Configuration", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(quizDataProperty);
            
            if (quizDataProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Quiz Data is required. Create a Quiz Data asset and assign it here.", MessageType.Warning);
            }
            else
            {
                // Show preview of quiz data
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                QuizData quizData = (QuizData)quizDataProperty.objectReferenceValue;
                EditorGUILayout.LabelField("Quiz Title: " + quizData.quizTitle);
                EditorGUILayout.LabelField("Questions: " + quizData.questions.Count);
                EditorGUILayout.LabelField("Damage to Enemy (Correct): " + quizData.damageToEnemyOnCorrect);
                EditorGUILayout.LabelField("Damage to Player (Wrong): " + quizData.damageToPlayerOnWrong);
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Detection Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(detectionRadiusProperty);
            EditorGUILayout.PropertyField(playerLayerProperty);
            EditorGUILayout.PropertyField(quizCooldownProperty);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enemyDamageableProperty);
            
            if (enemyDamageableProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("No Damageable component assigned. The enemy won't take damage when questions are answered correctly.", MessageType.Warning);
            }
            
            EditorGUILayout.PropertyField(quizTriggerIndicatorProperty);
            
            EditorGUILayout.Space();
            
            // Check for QuizUIController in the scene
            QuizUIController quizUI = FindObjectOfType<QuizUIController>();
            if (quizUI == null)
            {
                EditorGUILayout.HelpBox("No QuizUIController found in the scene. Please add the Quiz UI prefab to your scene.", MessageType.Error);
                
                if (GUILayout.Button("Create Default Quiz UI"))
                {
                    // This button would ideally instantiate a prefab, but since we're not creating that here,
                    // we'll just give instructions
                    EditorUtility.DisplayDialog("Quiz UI Setup",
                        "Please follow these steps to create a Quiz UI:\n\n" +
                        "1. Create a Canvas in your scene\n" +
                        "2. Add a Panel as child (this will be the quiz panel)\n" +
                        "3. Add TextMeshProUGUI components for title and question\n" +
                        "4. Add buttons for answers\n" +
                        "5. Add the QuizUIController component to the Canvas\n" +
                        "6. Assign all references in the QuizUIController\n\n" +
                        "See the QuizAttackSetupGuide.md file for detailed instructions.",
                        "OK");
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
