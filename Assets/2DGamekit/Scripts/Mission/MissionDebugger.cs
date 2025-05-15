using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Mission
{
    // Editor utility for mission development and testing
    public class MissionDebugger : MonoBehaviour
    {
        [Header("Mission Testing")]
        public MissionData testMission;
        public string testItemID;
        
        [ContextMenu("Assign Test Mission")]
        public void AssignTestMission()
        {
            if (testMission != null && MissionManager.Instance != null)
            {
                MissionManager.Instance.AssignMission(testMission);
                Debug.Log($"Assigned mission: {testMission.name}");
            }
        }
        
        [ContextMenu("Complete Test Mission")]
        public void CompleteTestMission()
        {
            if (testMission != null && MissionManager.Instance != null)
            {
                int requiredAmount = testMission.requiredAmount;
                for (int i = 0; i < requiredAmount; i++)
                {
                    MissionManager.Instance.UpdateMissionProgress(testMission.targetItemID);
                }
                Debug.Log($"Completed mission: {testMission.name}");
            }
        }
        
        [ContextMenu("Simulate Item Collection")]
        public void SimulateItemCollection()
        {
            if (!string.IsNullOrEmpty(testItemID) && MissionManager.Instance != null)
            {
                MissionManager.Instance.UpdateMissionProgress(testItemID);
                Debug.Log($"Collected item: {testItemID}");
            }
        }
        
#if UNITY_EDITOR
        // Draw mission gizmos for NPCs with missions
        private void OnDrawGizmosSelected()
        {
            // Draw an icon for NPCs with missions
            MissionNPC missionNPC = GetComponent<MissionNPC>();
            if (missionNPC != null && missionNPC.availableMission != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 1f);
                
                Handles.color = Color.yellow;
                Handles.Label(transform.position + Vector3.up * 1.5f, $"Mission: {missionNPC.availableMission.missionName}");
            }
        }
#endif
    }
}
