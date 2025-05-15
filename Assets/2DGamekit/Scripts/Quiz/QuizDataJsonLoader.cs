using UnityEngine;

namespace Gamekit2D.Quiz
{
    // Simple example of a quiz data loader that allows loading from JSON at runtime
    public class QuizDataJsonLoader : MonoBehaviour
    {
        [Header("References")]
        public TextAsset jsonQuizFile;
        public QuizData targetQuizData;
        
        [System.Serializable]
        private class SerializableQuestion
        {
            public string question;
            public string[] answers;
            public int correctAnswerIndex;
        }
        
        [System.Serializable]
        private class SerializableQuiz
        {
            public string quizTitle;
            public SerializableQuestion[] questions;
            public int damageToEnemyOnCorrect;
            public int damageToPlayerOnWrong;
        }
        
        // Call this to load data from JSON into the QuizData scriptable object
        public void LoadFromJson()
        {
            if (jsonQuizFile == null || targetQuizData == null)
                return;
                
            // Parse JSON
            SerializableQuiz loadedQuiz = JsonUtility.FromJson<SerializableQuiz>(jsonQuizFile.text);
            
            // Update the target QuizData
            targetQuizData.quizTitle = loadedQuiz.quizTitle;
            targetQuizData.damageToEnemyOnCorrect = loadedQuiz.damageToEnemyOnCorrect;
            targetQuizData.damageToPlayerOnWrong = loadedQuiz.damageToPlayerOnWrong;
            
            // Clear and fill questions
            targetQuizData.questions.Clear();
            foreach (SerializableQuestion loadedQuestion in loadedQuiz.questions)
            {
                QuizQuestion question = new QuizQuestion();
                question.question = loadedQuestion.question;
                question.answers = loadedQuestion.answers;
                question.correctAnswerIndex = loadedQuestion.correctAnswerIndex;
                targetQuizData.questions.Add(question);
            }
        }
        
        // Example of how your JSON file should look:
        /*
        {
            "quizTitle": "Enemy Quiz",
            "questions": [
                {
                    "question": "What is 2+2?",
                    "answers": ["3", "4", "5", "6"],
                    "correctAnswerIndex": 1
                },
                {
                    "question": "Which color is not in the rainbow?",
                    "answers": ["Red", "Brown", "Blue", "Green"],
                    "correctAnswerIndex": 1
                }
            ],
            "damageToEnemyOnCorrect": 1,
            "damageToPlayerOnWrong": 1
        }
        */
    }
}
