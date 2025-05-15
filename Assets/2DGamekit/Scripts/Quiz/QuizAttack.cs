using System.Collections;
using UnityEngine;

namespace Gamekit2D.Quiz
{
    [RequireComponent(typeof(Collider2D))]
    public class QuizAttack : MonoBehaviour
    {
        [Header("Quiz Configuration")]
        public QuizData quizData;
        public float detectionRadius = 3f;
        public LayerMask playerLayer;
        public float quizCooldown = 20f; // Time before quiz can be triggered again
        
        [Header("References")]
        public Damageable enemyDamageable;
        public GameObject quizTriggerIndicator;  // Optional visual indicator when player is in range
        
        private bool canTriggerQuiz = true;
        private bool isQuizActive = false;
        private float cooldownTimer = 0f;
        private PlayerCharacter player;
        private QuizUIController quizUI;
        
        private void Awake()
        {
            // Find references if not assigned
            if (enemyDamageable == null)
                enemyDamageable = GetComponent<Damageable>();
                
            if (quizTriggerIndicator != null)
                quizTriggerIndicator.SetActive(false);
        }
        
        private void Start()
        {
            // Find the quiz UI
            quizUI = FindObjectOfType<QuizUIController>();
            if (quizUI == null)
            {
                Debug.LogError("QuizUIController not found in the scene. Make sure to add it to your UI canvas.");
            }
        }
        
        private void Update()
        {
            if (canTriggerQuiz)
            {
                // Check if player is in range
                CheckForPlayerInRange();
            }
            else if (!isQuizActive)
            {
                // Handle cooldown
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canTriggerQuiz = true;
                }
            }
        }
        
        private void CheckForPlayerInRange()
        {
            // Check for player in detection radius
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            
            if (playerCollider != null)
            {
                player = playerCollider.GetComponent<PlayerCharacter>();
                if (player != null && !isQuizActive)
                {
                    // Show indicator if available
                    if (quizTriggerIndicator != null)
                        quizTriggerIndicator.SetActive(true);
                    
                    // If player presses interaction button, start quiz
                    if (PlayerInput.Instance.Interact.Down)
                    {
                        StartQuiz();
                    }
                }
            }
            else
            {
                // Hide indicator when player leaves area
                if (quizTriggerIndicator != null)
                    quizTriggerIndicator.SetActive(false);
            }
        }
          public void StartQuiz()
        {
            if (!canTriggerQuiz || quizUI == null || quizData == null) 
                return;
                
            isQuizActive = true;
            canTriggerQuiz = false;
            
            // Disable player movement
            player.DisableMovement();
            
            // Start the quiz with both callbacks
            quizUI.StartQuiz(quizData, OnQuizAnswerGiven, EndQuiz);
        }
          private void OnQuizAnswerGiven(bool isCorrect)
        {
            if (isCorrect)
            {
                // Damage the enemy
                if (enemyDamageable != null)
                {
                    // In 2D Gamekit, we use TakeDamage method which takes a Damager and a bool to ignore invincibility
                    if (player.meleeDamager != null)
                    {
                        // Store the original damage value
                        int originalDamage = player.meleeDamager.damage;
                        
                        // Set the damage to our quiz damage value
                        player.meleeDamager.damage = quizData.damageToEnemyOnCorrect;
                        
                        // Apply damage to the enemy
                        enemyDamageable.TakeDamage(player.meleeDamager, false);
                        
                        // Restore the original damage value
                        player.meleeDamager.damage = originalDamage;
                    }
                }
            }
            else
            {
                // Damage the player
                if (player.damageable != null && player.meleeDamager != null)
                {
                    // Create a temporary damager component if we need one
                    Damager tempDamager = GetComponent<Damager>();
                    bool createdTempDamager = false;
                    
                    if (tempDamager == null)
                    {
                        tempDamager = gameObject.AddComponent<Damager>();
                        createdTempDamager = true;
                    }
                    
                    // Store the original damage value
                    int originalDamage = tempDamager.damage;
                    
                    // Configure and use the damager
                    tempDamager.damage = quizData.damageToPlayerOnWrong;
                    player.damageable.TakeDamage(tempDamager, false);
                    
                    // Clean up - either restore original value or remove the component
                    if (createdTempDamager)
                    {
                        Destroy(tempDamager);
                    }
                    else
                    {
                        tempDamager.damage = originalDamage;
                    }
                }
            }
            
            // Check if enemy is dead after damage
            if (enemyDamageable != null && enemyDamageable.CurrentHealth <= 0)
            {
                // If enemy died, end quiz early
                EndQuiz();
            }
        }
        
        private void EndQuiz()
        {
            if (!isQuizActive)
                return;
                
            isQuizActive = false;
            cooldownTimer = quizCooldown;
            
            // Enable player movement
            player.EnableMovement();
            
            // Hide quiz UI if it's still active
            quizUI.EndQuiz();
            
            // Hide indicator
            if (quizTriggerIndicator != null)
                quizTriggerIndicator.SetActive(false);
        }
        
        // Visualization for the detection radius in the editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
