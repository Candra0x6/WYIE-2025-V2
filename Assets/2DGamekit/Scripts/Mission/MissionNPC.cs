using UnityEngine;
using Gamekit2D.Dialogue;

namespace Gamekit2D.Mission
{    // Add this component to NPCs that can give and complete missions
    public class MissionNPC : MonoBehaviour
    {
        [Header("Mission Settings")]
        public MissionData[] availableMissions; // Array to support multiple missions
        public bool assignMissionsSequentially = true; // If true, missions will be assigned one after another
        
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
            if (availableMissions == null || availableMissions.Length == 0 || MissionManager.Instance == null) return;
            
            // Check for active missions first (handle completion)
            for (int i = 0; i < availableMissions.Length; i++)
            {
                MissionData mission = availableMissions[i];
                if (mission == null) continue;
                
                if (MissionManager.Instance.IsMissionActive(mission) && 
                    MissionManager.Instance.IsMissionCompleted(mission))
                {
                    // Mission completed - apply rewards
                    MissionManager.Instance.CompleteMission(mission);
                    
                    // Check if there's a next mission to assign
                    if (assignMissionsSequentially && i < availableMissions.Length - 1)
                    {
                        // Set dialogue for next mission
                        if (availableMissions[i + 1].startDialogue != null)
                        {
                            dialogueTrigger.dialogueData = availableMissions[i + 1].startDialogue;
                        }
                        else
                        {
                            dialogueTrigger.dialogueData = defaultDialogue;
                        }
                    }
                    else
                    {
                        // Reset dialogue to default for next interaction
                        dialogueTrigger.dialogueData = defaultDialogue;
                    }
                    
                    return; // Only handle one mission completion at a time
                }
            }
            
            // Check if we need to assign a new mission
            if (assignMissionsSequentially)
            {
                // Assign missions in order
                for (int i = 0; i < availableMissions.Length; i++)
                {
                    MissionData mission = availableMissions[i];
                    if (mission == null) continue;
                    
                    if (!MissionManager.Instance.IsMissionActive(mission))
                    {
                        // Mission not yet assigned - assign it
                        MissionManager.Instance.AssignMission(mission);
                        
                        // Set dialogue for next interaction to the mission's complete dialogue
                        if (mission.completeDialogue != null)
                        {
                            dialogueTrigger.dialogueData = mission.completeDialogue;
                        }
                        
                        return; // Only assign one mission at a time
                    }
                }
            }
            else
            {
                // Assign all available missions at once
                bool assignedAny = false;
                
                foreach (MissionData mission in availableMissions)
                {
                    if (mission == null) continue;
                    
                    if (!MissionManager.Instance.IsMissionActive(mission))
                    {
                        // Mission not yet assigned - assign it
                        MissionManager.Instance.AssignMission(mission);
                        assignedAny = true;
                        
                        // Don't set dialogue here as we might assign multiple missions
                    }
                }
                
                // If we assigned any missions, set dialogue for next interaction
                if (assignedAny && availableMissions[0].completeDialogue != null)
                {
                    dialogueTrigger.dialogueData = availableMissions[0].completeDialogue;
                }
            }
        }
          // Update dialogue based on mission status
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (availableMissions == null || availableMissions.Length == 0 || MissionManager.Instance == null || dialogueTrigger == null) return;
            
            // Check if it's the player
            if (((1 << other.gameObject.layer) & dialogueTrigger.playerLayer) != 0)
            {
                UpdateDialogueBasedOnMissionStatus();
            }
        }
          private void UpdateDialogueBasedOnMissionStatus()
        {
            if (availableMissions == null || availableMissions.Length == 0 || MissionManager.Instance == null || dialogueTrigger == null) return;
            
            // First check for active and completed missions (highest priority)
            foreach (MissionData mission in availableMissions)
            {
                if (mission == null) continue;
                
                if (MissionManager.Instance.IsMissionActive(mission) && 
                    MissionManager.Instance.IsMissionCompleted(mission))
                {
                    // Mission completed - show completion dialogue
                    if (mission.completeDialogue != null)
                    {
                        dialogueTrigger.dialogueData = mission.completeDialogue;
                        return;
                    }
                }
            }
            
            // Then check for active but not completed missions
            foreach (MissionData mission in availableMissions)
            {
                if (mission == null) continue;
                
                if (MissionManager.Instance.IsMissionActive(mission) && 
                    !MissionManager.Instance.IsMissionCompleted(mission))
                {
                    // Mission in progress - show start dialogue again
                    if (mission.startDialogue != null)
                    {
                        dialogueTrigger.dialogueData = mission.startDialogue;
                        return;
                    }
                }
            }
            
            // If we get here, no missions are active or we're ready to assign the next mission
            if (assignMissionsSequentially)
            {
                // Find the first unassigned mission
                foreach (MissionData mission in availableMissions)
                {
                    if (mission == null) continue;
                    
                    if (!MissionManager.Instance.IsMissionActive(mission))
                    {
                        // Mission not yet accepted - show start dialogue
                        if (mission.startDialogue != null)
                        {
                            dialogueTrigger.dialogueData = mission.startDialogue;
                            return;
                        }
                    }
                }
            }
            else if (availableMissions.Length > 0 && availableMissions[0] != null)
            {
                // Not sequential, show the first mission's start dialogue
                if (availableMissions[0].startDialogue != null)
                {
                    dialogueTrigger.dialogueData = availableMissions[0].startDialogue;
                    return;
                }
            }
            
            // No special dialogue for missions, use default
            dialogueTrigger.dialogueData = defaultDialogue;
        }
    }
}
