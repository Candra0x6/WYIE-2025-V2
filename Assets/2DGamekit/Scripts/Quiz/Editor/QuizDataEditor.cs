using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gamekit2D.Quiz
{
    [CustomEditor(typeof(QuizData))]
    public class QuizDataEditor : Editor
    {
        private SerializedProperty quizTitleProperty;
        private SerializedProperty questionsProperty;
        private SerializedProperty damageToEnemyProperty;
        private SerializedProperty damageToPlayerProperty;
        
        private void OnEnable()
        {
            quizTitleProperty = serializedObject.FindProperty("quizTitle");
            questionsProperty = serializedObject.FindProperty("questions");
            damageToEnemyProperty = serializedObject.FindProperty("damageToEnemyOnCorrect");
            damageToPlayerProperty = serializedObject.FindProperty("damageToPlayerOnWrong");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(quizTitleProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Damage Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(damageToEnemyProperty, new GUIContent("Damage To Enemy (Correct Answer)"));
            EditorGUILayout.PropertyField(damageToPlayerProperty, new GUIContent("Damage To Player (Wrong Answer)"));
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quiz Questions", EditorStyles.boldLabel);
            
            // Quiz questions list
            EditorGUI.indentLevel++;
            
            int questionCount = questionsProperty.arraySize;
            int newQuestionCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Questions", questionCount));
            
            // Resize array if needed
            if (newQuestionCount != questionCount)
            {
                while (questionsProperty.arraySize < newQuestionCount)
                {
                    questionsProperty.InsertArrayElementAtIndex(questionsProperty.arraySize);
                }
                while (questionsProperty.arraySize > newQuestionCount)
                {
                    questionsProperty.DeleteArrayElementAtIndex(questionsProperty.arraySize - 1);
                }
            }
            
            // Question details
            for (int i = 0; i < questionsProperty.arraySize; i++)
            {
                SerializedProperty questionProperty = questionsProperty.GetArrayElementAtIndex(i);
                SerializedProperty questionTextProperty = questionProperty.FindPropertyRelative("question");
                SerializedProperty answersProperty = questionProperty.FindPropertyRelative("answers");
                SerializedProperty correctAnswerIndexProperty = questionProperty.FindPropertyRelative("correctAnswerIndex");
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Question {i + 1}", EditorStyles.boldLabel);
                
                EditorGUILayout.PropertyField(questionTextProperty, new GUIContent("Question"));
                
                // Answer choices
                EditorGUILayout.LabelField("Answers");
                
                int answerCount = answersProperty.arraySize;
                // Always ensure we have at least 2 answers
                int newAnswerCount = Mathf.Max(2, EditorGUILayout.IntField("Number of Answers", answerCount));
                
                // Resize answers array if needed
                if (newAnswerCount != answerCount)
                {
                    while (answersProperty.arraySize < newAnswerCount)
                    {
                        answersProperty.InsertArrayElementAtIndex(answersProperty.arraySize);
                    }
                    while (answersProperty.arraySize > newAnswerCount)
                    {
                        answersProperty.DeleteArrayElementAtIndex(answersProperty.arraySize - 1);
                    }
                    
                    // Make sure correct answer index is valid
                    correctAnswerIndexProperty.intValue = Mathf.Clamp(correctAnswerIndexProperty.intValue, 0, newAnswerCount - 1);
                }
                
                // Display answers
                EditorGUI.indentLevel++;
                for (int j = 0; j < answersProperty.arraySize; j++)
                {
                    SerializedProperty answer = answersProperty.GetArrayElementAtIndex(j);
                    
                    // Create label with indicator if it's the correct answer
                    string label = (j == correctAnswerIndexProperty.intValue) ? $"Answer {j + 1} (Correct)" : $"Answer {j + 1}";
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(answer, new GUIContent(label));
                    
                    // Set this as correct answer button
                    if (GUILayout.Button("Set as Correct", GUILayout.Width(120)))
                    {
                        correctAnswerIndexProperty.intValue = j;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
            }
            
            EditorGUI.indentLevel--;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
