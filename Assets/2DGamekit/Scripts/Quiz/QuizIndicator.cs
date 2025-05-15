using UnityEngine;

namespace Gamekit2D.Quiz
{
    // This component will create a visual indicator that the player can interact
    // with the QuizAttack when in range
    public class QuizIndicator : MonoBehaviour
    {
        [Header("Visual Settings")]
        public float bobAmount = 0.5f;
        public float bobSpeed = 2f;
        public Transform visualElement;
        
        private Vector3 startPosition;
        private float bobTimer;
        
        private void Awake()
        {
            if (visualElement == null)
                visualElement = transform;
                
            startPosition = visualElement.localPosition;
        }
        
        private void OnEnable()
        {
            bobTimer = 0f;
        }
        
        private void Update()
        {
            // Simple up and down bobbing animation
            bobTimer += Time.deltaTime * bobSpeed;
            
            Vector3 newPosition = startPosition;
            newPosition.y += Mathf.Sin(bobTimer) * bobAmount;
            
            visualElement.localPosition = newPosition;
        }
    }
}
