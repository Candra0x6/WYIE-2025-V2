using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gamekit2D.Mission
{
    // Editor utility for mission development and testing
    public class MissionDebugger : MonoBehaviour
    {
        [Header("Mission Testing")]
        public MissionData[] testMissions;
        public int selectedMissionIndex = 0;
        public string testItemID;
          [ContextMenu("Assign Test Mission")]
        public void AssignTestMission()
        {
            if (testMissions == null || testMissions.Length == 0 || MissionManager.Instance == null) return;
            
            // Make sure index is valid
            selectedMissionIndex = Mathf.Clamp(selectedMissionIndex, 0, testMissions.Length - 1);
            
            MissionData selectedMission = testMissions[selectedMissionIndex];
            if (selectedMission != null)
            {
                MissionManager.Instance.AssignMission(selectedMission);
                Debug.Log($"Assigned mission: {selectedMission.name}");
            }
        }
        
        [ContextMenu("Complete Test Mission")]
        public void CompleteTestMission()
        {
            if (testMissions == null || testMissions.Length == 0 || MissionManager.Instance == null) return;
            
            // Make sure index is valid
            selectedMissionIndex = Mathf.Clamp(selectedMissionIndex, 0, testMissions.Length - 1);
            
            MissionData selectedMission = testMissions[selectedMissionIndex];
            if (selectedMission != null)
            {
                int requiredAmount = selectedMission.requiredAmount;
                for (int i = 0; i < requiredAmount; i++)
                {
                    MissionManager.Instance.UpdateMissionProgress(selectedMission.targetItemID);
                }
                Debug.Log($"Completed mission: {selectedMission.name}");
            }
        }
        
        [ContextMenu("Assign All Test Missions")]
        public void AssignAllTestMissions()
        {
            if (testMissions == null || testMissions.Length == 0 || MissionManager.Instance == null) return;
            
            foreach (var mission in testMissions)
            {
                if (mission != null)
                {
                    MissionManager.Instance.AssignMission(mission);
                    Debug.Log($"Assigned mission: {mission.name}");
                }
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
            if (missionNPC != null && missionNPC.availableMissions != null && missionNPC.availableMissions.Length > 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 1f);
                
                Handles.color = Color.yellow;
                
                string missionNames = "";
                for (int i = 0; i < Mathf.Min(missionNPC.availableMissions.Length, 3); i++)
                {
                    if (missionNPC.availableMissions[i] != null)
                    {
                        if (i > 0) missionNames += ", ";
                        missionNames += missionNPC.availableMissions[i].missionName;
                    }
                }
                
                if (missionNPC.availableMissions.Length > 3)
                {
                    missionNames += "...";
                }
                
                Handles.Label(transform.position + Vector3.up * 1.5f, $"Missions: {missionNames}");
            }
        }
#endif
    }
}
