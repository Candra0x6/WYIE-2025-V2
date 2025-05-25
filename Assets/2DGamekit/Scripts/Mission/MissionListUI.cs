using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Gamekit2D.Mission
{    /// <summary>
    /// UI component to display a list of all active missions
    /// </summary>
    public class MissionListUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject missionListPanel;        // The main panel containing all missions
        public GameObject missionEntryPrefab;      // Prefab for each mission entry (contains name, progress, etc.)
        public Transform missionEntryContainer;    // Container to parent mission entries to
        public TextMeshProUGUI noMissionsText;     // Text to show when no missions are active
        
        [Header("Settings")]
        public KeyCode toggleKey = KeyCode.J;      // Key to toggle mission list visibility
        public bool keepOpenAfterToggle = true;    // If true, mission list stays open until manually closed
        public bool showNotificationOnNewMission = true; // Whether to briefly show the mission list when a new mission is assigned
        
        // Dictionary to track UI entries for each active mission
        private Dictionary<MissionData, GameObject> missionEntries = new Dictionary<MissionData, GameObject>();
        private Coroutine notificationCoroutine;   // Reference to the active notification coroutine
        
        private void Awake()
        {
            // Hide panel at start
            if (missionListPanel != null)
                missionListPanel.SetActive(false);
                
            // Hide "no missions" text if there are active missions
            if (noMissionsText != null)
                noMissionsText.gameObject.SetActive(false);
        }
          private void OnEnable()
        {
            // Subscribe to mission events
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.OnMissionAssigned.AddListener(HandleMissionAssigned);
                MissionManager.Instance.OnMissionUpdated.AddListener(HandleMissionUpdated);
                MissionManager.Instance.OnMissionCompleted.AddListener(HandleMissionCompleted);
                MissionManager.Instance.OnMissionRemoved.AddListener(RemoveMissionEntry);
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
                MissionManager.Instance.OnMissionRemoved.RemoveListener(RemoveMissionEntry);
            }
        }
        
        private void Update()
        {
            // Toggle mission list visibility with key press
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleMissionList();
            }
        }
        
        // Toggle mission list visibility
        public void ToggleMissionList()
        {
            if (missionListPanel != null)
            {
                missionListPanel.SetActive(!missionListPanel.activeSelf);
            }
        }        // Add new mission to the list when assigned
        public void HandleMissionAssigned(MissionData missionData, int current, int required)
        {
            // Create mission entry if it doesn't exist
            if (!missionEntries.ContainsKey(missionData))
            {
                CreateMissionEntry(missionData, current, required);
            }
            
            // Show a notification that a new mission was assigned
            if (missionListPanel != null && showNotificationOnNewMission)
            {
                // Stop existing notification if there is one
                if (notificationCoroutine != null)
                {
                    StopCoroutine(notificationCoroutine);
                }
                
                // Start new notification
                notificationCoroutine = StartCoroutine(ShowMissionNotification(3f));
            }
            
            // Hide "no missions" text when we have missions
            if (noMissionsText != null)
                noMissionsText.gameObject.SetActive(false);
        }
        
        // Update mission progress
        public void HandleMissionUpdated(MissionData missionData, int current, int required)
        {
            UpdateMissionEntry(missionData, current, required);
        }
        
        // Update mission as completed
        public void HandleMissionCompleted(MissionData missionData, int current, int required)
        {
            UpdateMissionEntry(missionData, required, required, true);
        }
          // Create a new UI entry for a mission
        private void CreateMissionEntry(MissionData missionData, int current, int required)
        {
            if (missionEntryPrefab == null || missionEntryContainer == null) return;
            
            // Instantiate mission entry prefab
            GameObject entryObj = Instantiate(missionEntryPrefab, missionEntryContainer);
            
            // Store reference to the entry
            missionEntries[missionData] = entryObj;
            
            // Try to get MissionEntryUI component first
            MissionEntryUI entryUI = entryObj.GetComponent<MissionEntryUI>();
            if (entryUI != null)
            {
                // Use the component's setup method
                entryUI.Setup(missionData, current, required, false);
            }
            else
            {
                // Fall back to manual update
                UpdateMissionEntryUI(entryObj, missionData, current, required);
            }
        }
          // Update an existing mission entry
        private void UpdateMissionEntry(MissionData missionData, int current, int required, bool completed = false)
        {
            if (!missionEntries.TryGetValue(missionData, out GameObject entryObj)) return;
            
            // Try to get MissionEntryUI component first
            MissionEntryUI entryUI = entryObj.GetComponent<MissionEntryUI>();
            if (entryUI != null)
            {
                // Use the component's setup method
                entryUI.Setup(missionData, current, required, completed);
            }
            else
            {
                // Fall back to manual update
                UpdateMissionEntryUI(entryObj, missionData, current, required, completed);
            }
        }
        
        // Update UI elements in a mission entry
        private void UpdateMissionEntryUI(GameObject entryObj, MissionData missionData, int current, int required, bool completed = false)
        {
            // Find UI components in the entry prefab
            TextMeshProUGUI nameText = entryObj.transform.Find("MissionName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI progressText = entryObj.transform.Find("Progress")?.GetComponent<TextMeshProUGUI>();
            Image progressBar = entryObj.transform.Find("ProgressBar")?.GetComponent<Image>();
            Image statusIcon = entryObj.transform.Find("StatusIcon")?.GetComponent<Image>();
            
            // Update mission name
            if (nameText != null)
            {
                nameText.text = missionData.missionName;
                
                // Optionally change color if completed
                if (completed)
                    nameText.color = Color.green;
            }
            
            // Update progress text
            if (progressText != null)
                progressText.text = $"{current}/{required}";
                
            // Update progress bar
            if (progressBar != null)
                progressBar.fillAmount = (float)current / required;
                
            // Update status icon if available (can show different icons for active vs completed)
            if (statusIcon != null && statusIcon.GetComponent<Image>() != null)
            {
                // You could change the icon based on status
                // statusIcon.sprite = completed ? completedSprite : activeSprite;
                
                // Or just change the color
                statusIcon.color = completed ? Color.green : Color.yellow;
            }
        }
        
        // Remove a mission entry when the mission is turned in
        public void RemoveMissionEntry(MissionData missionData)
        {
            if (missionEntries.TryGetValue(missionData, out GameObject entryObj))
            {
                // Remove entry from dictionary
                missionEntries.Remove(missionData);
                
                // Destroy the UI object
                Destroy(entryObj);
                
                // Show "no missions" text if there are no more missions
                if (missionEntries.Count == 0 && noMissionsText != null)
                {
                    noMissionsText.gameObject.SetActive(true);
                }
            }
        }
          // Show panel for a brief time
        private System.Collections.IEnumerator ShowPanelBriefly(float duration)
        {
            missionListPanel.SetActive(true);
            yield return new WaitForSeconds(duration);
            missionListPanel.SetActive(false);
        }
        
        // Show a notification that a new mission was assigned
        private System.Collections.IEnumerator ShowMissionNotification(float duration)
        {
            // First activate the panel to show it
            bool wasActive = missionListPanel.activeSelf;
            missionListPanel.SetActive(true);
            
            // Create a notification overlay or highlight new mission
            // (For now, we just keep the panel open to show the mission)
            
            // Wait for the notification duration
            yield return new WaitForSeconds(duration);
            
            // Only hide the panel if it wasn't active before
            if (!wasActive)
            {
                missionListPanel.SetActive(false);
            }
            // Otherwise leave it visible since the user had it open already
        }
    }
}
