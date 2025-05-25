using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gamekit2D.Dialogue
{
    public class DialogueUIController : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI speakerNameText;
        public Image speakerImage;
        public GameObject speakerImageContainer;
        public TextMeshProUGUI dialogueText;
        public GameObject optionsPanel;
        public Button optionButtonPrefab;
        public GameObject continuePrompt;
        
        [Header("Settings")]
        public float textDisplaySpeed = 0.05f;
        public float delayAfterDialogue = 0.5f;
        
        [Header("Audio")]
        public RandomAudioPlayer dialogueAudioPlayer;
        
        private DialogueData currentDialogue;
        private int currentNodeIndex;
        private PlayerCharacter playerCharacter;
        private Coroutine textDisplayCoroutine;
        private bool isDisplayingText;
        private bool hasCompletedTextDisplay;
        
        private void Awake()
        {
            // Ensure dialogue panel is hidden at start
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
                
            playerCharacter = PlayerCharacter.PlayerInstance;
        }
          private void Update()
        {
            // Double check player can't move during dialogue
            if (dialoguePanel.activeInHierarchy && playerCharacter != null && PlayerInput.Instance != null)
            {
                // Make extra sure movement is disabled during dialogue
                playerCharacter.DisableMovement();
            }
            
            // Skip to end of text if player presses space during text reveal
            if (isDisplayingText && !hasCompletedTextDisplay && 
                (Input.GetKeyDown(KeyCode.Space) || PlayerInput.Instance.Interact.Down))
            {
                SkipToEndOfText();
            }
            
            // Advance dialogue if player presses space after text is fully displayed
            if (hasCompletedTextDisplay && !optionsPanel.activeInHierarchy && 
                (Input.GetKeyDown(KeyCode.Space) || PlayerInput.Instance.Interact.Down))
            {
                AdvanceDialogue();
            }
        }        public void StartDialogue(DialogueData dialogueData)
        {
            if (dialogueData == null || dialogueData.nodes.Count == 0)
                return;
                
            // Store reference to dialogue data
            currentDialogue = dialogueData;
            currentNodeIndex = 0;
            
            // Make sure we have a reference to the player character
            if (playerCharacter == null)
                playerCharacter = PlayerCharacter.PlayerInstance;
            
            // Disable player movement - force this to happen
            if (playerCharacter != null)
            {
                playerCharacter.DisableMovement();
                
                // Double check PlayerInput is also disabled to prevent any movement
                if (PlayerInput.Instance != null)
                {
                    PlayerInput.Instance.ReleaseControl(true);
                }
            }
                
            // Play start dialogue sound
            if (dialogueData.startDialogueSound != null && dialogueAudioPlayer != null)
                dialogueAudioPlayer.PlayRandomSound();
                
            // Show dialogue panel and ensure UI elements are active
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                
                // Ensure speaker name element is active
                if (speakerNameText != null && speakerNameText.transform.parent != null)
                    speakerNameText.transform.parent.gameObject.SetActive(true);
                
                // Ensure dialogue text element is active
                if (dialogueText != null && dialogueText.transform.parent != null)
                    dialogueText.transform.parent.gameObject.SetActive(true);
            }
            
            // Display the first node
            DisplayNode(dialogueData.GetStartNode());
        }
          private void DisplayNode(DialogueNode node)
        {
            if (node == null)
            {
                EndDialogue();
                return;
            }
            
            // Set speaker name (ensure the text component is active)
            if (speakerNameText != null)
            {
                if (speakerNameText.transform.parent != null)
                    speakerNameText.transform.parent.gameObject.SetActive(true);
                speakerNameText.gameObject.SetActive(true);
                speakerNameText.text = node.speakerName;
            }
            
            // Set speaker image if available
            if (node.speakerImage != null)
            {
                if (speakerImageContainer != null)
                    speakerImageContainer.SetActive(true);
                if (speakerImage != null)
                {
                    speakerImage.gameObject.SetActive(true);
                    speakerImage.sprite = node.speakerImage;
                }
            }
            else if (speakerImageContainer != null)
            {
                speakerImageContainer.SetActive(false);
            }
            
            // Clear any existing options
            ClearOptions();
            
            // Hide continue prompt during text display
            continuePrompt.SetActive(false);
            
            // Start displaying text
            if (textDisplayCoroutine != null)
                StopCoroutine(textDisplayCoroutine);
                
            textDisplayCoroutine = StartCoroutine(DisplayTextOverTime(node.text));
        }
        
        private IEnumerator DisplayTextOverTime(string text)
        {
            isDisplayingText = true;
            hasCompletedTextDisplay = false;
            dialogueText.text = "";
            
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textDisplaySpeed);
            }
            
            isDisplayingText = false;
            hasCompletedTextDisplay = true;
            
            // Show continue prompt or dialogue options
            ShowNodeCompletion();
        }
        
        private void SkipToEndOfText()
        {
            if (textDisplayCoroutine != null)
                StopCoroutine(textDisplayCoroutine);
                
            DialogueNode currentNode = currentDialogue.GetNode(currentNodeIndex);
            dialogueText.text = currentNode.text;
            
            isDisplayingText = false;
            hasCompletedTextDisplay = true;
            
            // Show continue prompt or dialogue options
            ShowNodeCompletion();
        }
        
        private void ShowNodeCompletion()
        {
            DialogueNode currentNode = currentDialogue.GetNode(currentNodeIndex);
            
            // If we have dialogue options, show them
            if (currentNode.options != null && currentNode.options.Count > 0)
            {
                DisplayOptions(currentNode);
            }
            else
            {
                // Otherwise show continue prompt
                continuePrompt.SetActive(true);
            }
        }
        
        private void DisplayOptions(DialogueNode node)
        {
            optionsPanel.SetActive(true);
            
            for (int i = 0; i < node.options.Count; i++)
            {
                DialogueOption option = node.options[i];
                Button optionButton = Instantiate(optionButtonPrefab, optionsPanel.transform);
                
                // Set button text
                TextMeshProUGUI buttonText = optionButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = option.text;
                
                // Set button click handler
                int optionIndex = i; // Capture index for lambda
                optionButton.onClick.AddListener(() => OnOptionSelected(optionIndex));
            }
        }
        
        private void ClearOptions()
        {
            // Destroy all child objects in the options panel
            foreach (Transform child in optionsPanel.transform)
            {
                Destroy(child.gameObject);
            }
            
            optionsPanel.SetActive(false);
        }
        
        private void OnOptionSelected(int optionIndex)
        {
            DialogueNode currentNode = currentDialogue.GetNode(currentNodeIndex);
            DialogueOption selectedOption = currentNode.options[optionIndex];
            
            // Play sound if available
            if (currentDialogue.advanceDialogueSound != null && dialogueAudioPlayer != null)
                dialogueAudioPlayer.PlayRandomSound();
                
            // Go to the next node or end dialogue
            if (selectedOption.nextNodeIndex >= 0)
            {
                currentNodeIndex = selectedOption.nextNodeIndex;
                DisplayNode(currentDialogue.GetNode(currentNodeIndex));
            }
            else
            {
                EndDialogue();
            }
        }
        
        private void AdvanceDialogue()
        {
            DialogueNode currentNode = currentDialogue.GetNode(currentNodeIndex);
            
            // Play sound if available
            if (currentDialogue.advanceDialogueSound != null && dialogueAudioPlayer != null)
                dialogueAudioPlayer.PlayRandomSound();
                
            // Go to the next node or end dialogue
            if (currentNode.nextNodeIndex >= 0)
            {
                currentNodeIndex = currentNode.nextNodeIndex;
                DisplayNode(currentDialogue.GetNode(currentNodeIndex));
            }
            else
            {
                EndDialogue();
            }
        }
        
        private void EndDialogue()
        {
            // Hide dialogue panel
            StartCoroutine(EndDialogueCoroutine());
        }
          private IEnumerator EndDialogueCoroutine()
        {
            // Wait a moment before hiding the dialogue
            yield return new WaitForSeconds(delayAfterDialogue);
            
            // Hide dialogue panel
            dialoguePanel.SetActive(false);
            
            // Enable player movement only after dialogue is fully hidden
            if (playerCharacter != null)
            {
                // Make sure we wait one more frame before enabling movement
                yield return null;
                
                // Now it's safe to enable movement
                playerCharacter.EnableMovement();
            }
                
            // Reset state
            currentDialogue = null;
            isDisplayingText = false;
            hasCompletedTextDisplay = false;
        }
    }
}
