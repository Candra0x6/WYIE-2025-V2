using UnityEngine;
using Gamekit2D.Dialogue;

namespace Gamekit2D.Mission
{
    // Add this component to NPCs that can give and complete missions
    public class MissionNPC : MonoBehaviour
    {
        [Header("Mission Settings")]
        public MissionData availableMission;
        
        [Header("Dialogue References")]
        public DialogueData defaultDialogue; // Shown when no mission is assigned
        
        private DialogueTrigger dialogueTrigger;
        
        private void Awake()
        {
            // Get or add dialogue trigger component
            dialogueTrigger = GetComponent<DialogueTrigger>();
            if (dialogueTrigger == null)
            {
                dialogueTrigger = gameObject.AddComponent<DialogueTrigger>();
            }
            
            // Set default dialogue
            dialogueTrigger.dialogueData = defaultDialogue;
            
            // Listen to dialogue end event
            dialogueTrigger.OnDialogueEnd.AddListener(HandleDialogueEnd);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (dialogueTrigger != null)
            {
                dialogueTrigger.OnDialogueEnd.RemoveListener(HandleDialogueEnd);
            }
        }
        
        // Called when dialogue with this NPC ends
        private void HandleDialogueEnd()
        {
            if (availableMission == null || MissionManager.Instance == null) return;
            
            // Check mission status
            if (MissionManager.Instance.IsMissionActive(availableMission) && 
                MissionManager.Instance.IsMissionCompleted(availableMission))
            {
                // Mission completed - apply rewards
                MissionManager.Instance.CompleteMission(availableMission);
                
                // Reset dialogue to default for next interaction
                dialogueTrigger.dialogueData = defaultDialogue;
            }
            else if (!MissionManager.Instance.IsMissionActive(availableMission))
            {
                // Mission not yet assigned - assign it
                MissionManager.Instance.AssignMission(availableMission);
                
                // Set dialogue for next interaction to the mission's complete dialogue
                if (availableMission.completeDialogue != null)
                {
                    dialogueTrigger.dialogueData = availableMission.completeDialogue;
                }
            }
        }
        
        // Update dialogue based on mission status
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (availableMission == null || MissionManager.Instance == null || dialogueTrigger == null) return;
            
            // Check if it's the player
            if (((1 << other.gameObject.layer) & dialogueTrigger.playerLayer) != 0)
            {
                UpdateDialogueBasedOnMissionStatus();
            }
        }
        
        private void UpdateDialogueBasedOnMissionStatus()
        {
            // Check mission status and update dialogue accordingly
            if (MissionManager.Instance.IsMissionActive(availableMission))
            {
                if (MissionManager.Instance.IsMissionCompleted(availableMission))
                {
                    // Mission completed - show completion dialogue
                    if (availableMission.completeDialogue != null)
                    {
                        dialogueTrigger.dialogueData = availableMission.completeDialogue;
                    }
                }
                else
                {
                    // Mission in progress - show start dialogue again
                    if (availableMission.startDialogue != null)
                    {
                        dialogueTrigger.dialogueData = availableMission.startDialogue;
                    }
                }
            }
            else
            {
                // Mission not yet accepted - show start dialogue
                if (availableMission.startDialogue != null)
                {
                    dialogueTrigger.dialogueData = availableMission.startDialogue;
                }
                else
                {
                    // No special dialogue for mission, use default
                    dialogueTrigger.dialogueData = defaultDialogue;
                }
            }
        }
    }
}
