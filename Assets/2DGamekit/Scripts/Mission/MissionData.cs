using UnityEngine;

namespace Gamekit2D.Mission
{
    [CreateAssetMenu(fileName = "New Mission", menuName = "Gamekit2D/Mission Data", order = 1)]
    public class MissionData : ScriptableObject
    {
        [Header("Mission Information")]
        public string missionName = "Collection Mission";
        [TextArea(3, 10)]
        public string description = "Collect the required items";
        
        [Header("Mission Requirements")]
        public string targetItemID; // This will match the inventoryKey in InventoryItem
        public int requiredAmount = 1;
        
            [Header("Mission Reward")]
        public float healthBonus = 1f; // Amount to increase player's health by
        public Sprite rewardIcon;
        
        [Header("Dialogue")]
        public Dialogue.DialogueData startDialogue; // Dialogue to show when mission is assigned
        public Dialogue.DialogueData completeDialogue; // Dialogue to show when mission is completed
        
        // Optional: Visual representation of the mission item
        public Sprite itemIcon;
    }
}
