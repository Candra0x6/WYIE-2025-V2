using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Gamekit2D.Quiz
{    public class QuizUIController : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject quizPanel;
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI quizTitleText;
        public Button[] answerButtons;
        public TextMeshProUGUI[] answerTexts;
        
        [Header("Audio")]
        public RandomAudioPlayer correctAnswerAudio;
        public RandomAudioPlayer wrongAnswerAudio;
        
        private QuizData currentQuiz;
        private int currentQuestionIndex;
        private Action<bool> onAnswerSelected;
        private Action onQuizCompleted;
        
        private void Awake()
        {
            // Make sure the quiz panel is hidden at start
            if (quizPanel != null)
                quizPanel.SetActive(false);
        }
          public void StartQuiz(QuizData quizData, Action<bool> answerCallback, Action completedCallback = null)
        {
            currentQuiz = quizData;
            currentQuestionIndex = 0;
            onAnswerSelected = answerCallback;
            onQuizCompleted = completedCallback;
            
            // Set quiz title
            if (quizTitleText != null)
                quizTitleText.text = quizData.quizTitle;
                
            // Show the quiz panel
            quizPanel.SetActive(true);
            
            // Display the first question
            DisplayCurrentQuestion();
        }
        
        private void DisplayCurrentQuestion()
        {
            if (currentQuiz == null || currentQuestionIndex >= currentQuiz.questions.Count)
            {
                Debug.LogError("No more questions available");
                return;
            }
            
            QuizQuestion question = currentQuiz.questions[currentQuestionIndex];
            
            // Set the question text
            questionText.text = question.question;
            
            // Set up answer buttons
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < question.answers.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    answerTexts[i].text = question.answers[i];
                    
                    // Set up button click handler
                    int index = i; // Capture index for lambda
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
        
        private void OnAnswerSelected(int answerIndex)
        {
            QuizQuestion currentQuestion = currentQuiz.questions[currentQuestionIndex];
            bool isCorrect = (answerIndex == currentQuestion.correctAnswerIndex);
            
            // Play appropriate audio
            if (isCorrect)
            {
                if (correctAnswerAudio != null)
                    correctAnswerAudio.PlayRandomSound();
            }
            else
            {
                if (wrongAnswerAudio != null)
                    wrongAnswerAudio.PlayRandomSound();
            }
            
            // Call the callback with result
            onAnswerSelected?.Invoke(isCorrect);
              // Move to next question or end quiz
            currentQuestionIndex++;
            if (currentQuestionIndex < currentQuiz.questions.Count)
            {
                DisplayCurrentQuestion();
            }
            else
            {
                EndQuiz();
            }
        }
        
        public void EndQuiz()
        {
            // Hide the quiz panel
            quizPanel.SetActive(false);
            
            // Notify that the quiz is completed
            onQuizCompleted?.Invoke();
            
            // Reset
            currentQuiz = null;
            currentQuestionIndex = 0;
            onAnswerSelected = null;
            onQuizCompleted = null;
        }
          // Call this to end the quiz early (like if the player or enemy dies during the quiz)
        public void CancelQuiz()
        {
            EndQuiz();
        }
    }
}
