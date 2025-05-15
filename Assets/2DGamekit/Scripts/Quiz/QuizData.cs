using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D.Quiz
{
    [CreateAssetMenu(fileName = "QuizData", menuName = "Gamekit2D/Quiz Data", order = 1)]
    public class QuizData : ScriptableObject
    {
        public string quizTitle = "Enemy Quiz";
        public List<QuizQuestion> questions = new List<QuizQuestion>();
        
        [Header("Damage Settings")]
        public int damageToEnemyOnCorrect = 1;
        public int damageToPlayerOnWrong = 1;
    }
}
