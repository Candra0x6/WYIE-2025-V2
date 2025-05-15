using UnityEngine;

namespace Gamekit2D.Quiz
{
    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea(3, 5)]
        public string question;
        public string[] answers;
        public int correctAnswerIndex;
    }
}
