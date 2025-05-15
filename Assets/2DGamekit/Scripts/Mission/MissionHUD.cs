using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gamekit2D.Mission
{
    public class MissionHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject missionHudPanel;
        public TextMeshProUGUI missionNameText;
        public TextMeshProUGUI progressText;
        public Image progressBar;
        public Image itemIcon;
        
        private MissionData currentMission;
        
        private void Awake()
        {
            // Hide HUD at start if no active missions
            if (missionHudPanel != null)
                missionHudPanel.SetActive(false);
        }
        
        private void OnEnable()
        {
            // Subscribe to mission events
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.OnMissionAssigned.AddListener(HandleMissionAssigned);
                MissionManager.Instance.OnMissionUpdated.AddListener(HandleMissionUpdated);
                MissionManager.Instance.OnMissionCompleted.AddListener(HandleMissionCompleted);
            }
        }
        
        private void OnDisable()
        {
            // Unsubscribe from mission events
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.OnMissionAssigned.RemoveListener(HandleMissionAssigned);
                MissionManager.Instance.OnMissionUpdated.RemoveListener(HandleMissionUpdated);
                MissionManager.Instance.OnMissionCompleted.RemoveListener(HandleMissionCompleted);
            }
        }
        
        // Show mission in HUD when assigned
        public void HandleMissionAssigned(MissionData missionData, int current, int required)
        {
            currentMission = missionData;
            
            // Show HUD panel
            if (missionHudPanel != null)
                missionHudPanel.SetActive(true);
            
            // Update UI elements
            UpdateHUD(missionData, current, required);
        }
        
        // Update progress display when mission progress changes
        public void HandleMissionUpdated(MissionData missionData, int current, int required)
        {
            // Only update if this is the active mission
            if (currentMission != missionData)
                return;
                
            // Update UI elements
            UpdateHUD(missionData, current, required);
        }
        
        // Update when mission is completed
        public void HandleMissionCompleted(MissionData missionData, int current, int required)
        {
            if (currentMission != missionData)
                return;
                
            // Update UI to show mission completed
            UpdateHUD(missionData, required, required);
        }
        
        private void UpdateHUD(MissionData missionData, int current, int required)
        {
            if (missionNameText != null)
                missionNameText.text = missionData.missionName;
                
            if (progressText != null)
                progressText.text = $"{current}/{required}";
                
            if (progressBar != null)
                progressBar.fillAmount = (float)current / required;
                
            if (itemIcon != null && missionData.itemIcon != null)
                itemIcon.sprite = missionData.itemIcon;
        }
    }
}
