using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D.Dialogue
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        public DialogueData dialogueData;
        public float interactionRadius = 2f;
        public LayerMask playerLayer;
        public Sprite dialogueIndicatorSprite;
        public GameObject dialogueIndicator;
        
        [Header("Events")]
        public UnityEvent OnDialogueStart;
        public UnityEvent OnDialogueEnd;
        
        private bool playerInRange = false;
        private SpriteRenderer indicatorRenderer;
        private DialogueUIController dialogueUI;
        private bool dialogueActive = false;
        
        private void Awake()
        {
            if (dialogueIndicator == null && dialogueIndicatorSprite != null)
            {
                dialogueIndicator = new GameObject("DialogueIndicator");
                dialogueIndicator.transform.parent = transform;
                dialogueIndicator.transform.localPosition = new Vector3(0, 1.5f, 0);
                
                indicatorRenderer = dialogueIndicator.AddComponent<SpriteRenderer>();
                indicatorRenderer.sprite = dialogueIndicatorSprite;
                indicatorRenderer.sortingOrder = 5;
            }
            else if (dialogueIndicator != null)
            {
                indicatorRenderer = dialogueIndicator.GetComponent<SpriteRenderer>();
                if (indicatorRenderer == null)
                    indicatorRenderer = dialogueIndicator.AddComponent<SpriteRenderer>();
            }
            
            // Hide indicator at start
            if (dialogueIndicator != null)
                dialogueIndicator.SetActive(false);
        }
          private void Start()
        {
            // Find dialogue UI
            dialogueUI = FindObjectOfType<DialogueUIController>();
            if (dialogueUI == null)
                Debug.LogWarning("No DialogueUIController found in scene. Dialogue will not work.", this);
                
            // Handle scene load edge case - if dialogue was active when scene changed
            if (dialogueActive)
            {
                dialogueActive = false;
                
                // Make sure player can move in case dialogue was interrupted
                PlayerCharacter player = PlayerCharacter.PlayerInstance;
                if (player != null)
                    player.EnableMovement();
            }
        }
        
        private void Update()
        {
            if (playerInRange && !dialogueActive)
            {
                // Show indicator that dialogue is available
                if (dialogueIndicator != null)
                    dialogueIndicator.SetActive(true);
                
                // Check for interaction key (E)
                if (PlayerInput.Instance.Interact.Down)
                {
                    StartDialogue();
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                playerInRange = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                playerInRange = false;
                
                // Hide indicator
                if (dialogueIndicator != null)
                    dialogueIndicator.SetActive(false);
            }
        }
        
        public void StartDialogue()
        {
            if (dialogueData == null || dialogueUI == null || dialogueActive)
                return;
                
            // Hide indicator
            if (dialogueIndicator != null)
                dialogueIndicator.SetActive(false);
                
            // Set dialogue active state
            dialogueActive = true;
            
            // Invoke event
            OnDialogueStart.Invoke();
            
            // Start dialogue in UI controller
            dialogueUI.StartDialogue(dialogueData);
            
            // Subscribe to UI controller events
            StartCoroutine(WaitForDialogueToEnd());
        }
          private IEnumerator WaitForDialogueToEnd()
        {
            // Make extra sure player can't move during dialogue
            PlayerCharacter player = PlayerCharacter.PlayerInstance;
            if (player != null)
                player.DisableMovement();
                
            // Wait until the dialogue UI is no longer active
            while (dialogueUI.gameObject.activeInHierarchy && dialogueUI.dialoguePanel.activeInHierarchy)
            {
                // Continuously ensure player can't move while dialogue is active
                if (player != null && PlayerInput.Instance != null && PlayerInput.Instance.HaveControl)
                {
                    player.DisableMovement();
                }
                yield return null;
            }
            
            // Wait one more frame to ensure synchronization with DialogueUIController
            yield return null;
            
            // Dialogue has ended
            dialogueActive = false;
            
            // Double check that movement is re-enabled (failsafe)
            if (player != null && !player.IsMovementEnabled())
            {
                player.EnableMovement();
            }
            
            // Invoke event
            OnDialogueEnd.Invoke();
        }
        
        // Show radius in Scene view
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
