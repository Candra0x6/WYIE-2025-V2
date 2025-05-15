using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Gamekit2D.Mission
{
    public class MissionUIController : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject missionPanel;
        public TextMeshProUGUI missionNameText;
        public TextMeshProUGUI missionDescriptionText;
        public TextMeshProUGUI progressText;
        public Image progressBar;
        public Image missionIcon;
        
        [Header("Animation")]
        public float showDuration = 5f;
        public float fadeDuration = 0.5f;
        public CanvasGroup canvasGroup;
        
        private Coroutine showCoroutine;
        private MissionData currentMission;
        
        private void Awake()
        {
            // Hide mission panel at start
            if (missionPanel != null)
                missionPanel.SetActive(false);
                
            if (canvasGroup == null)
                canvasGroup = missionPanel.GetComponent<CanvasGroup>();
                
            // Add a CanvasGroup if one doesn't exist
            if (canvasGroup == null && missionPanel != null)
                canvasGroup = missionPanel.AddComponent<CanvasGroup>();
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
        
        public void HandleMissionAssigned(MissionData missionData, int current, int required)
        {
            currentMission = missionData;
            
            // Update UI elements
            missionNameText.text = missionData.missionName;
            missionDescriptionText.text = missionData.description;
            progressText.text = $"{current}/{required}";
            progressBar.fillAmount = (float)current / required;
            
            if (missionData.itemIcon != null)
                missionIcon.sprite = missionData.itemIcon;
            
            // Show the mission panel
            ShowMissionPanel("New Mission");
        }
        
        public void HandleMissionUpdated(MissionData missionData, int current, int required)
        {
            // Only update if this is the current mission being displayed
            if (currentMission != missionData)
                return;
            
            // Update progress text and bar
            progressText.text = $"{current}/{required}";
            progressBar.fillAmount = (float)current / required;
            
            // Show mission panel with updated info
            ShowMissionPanel("Mission Updated");
        }
        
        public void HandleMissionCompleted(MissionData missionData, int current, int required)
        {
            // Update the UI
            progressText.text = $"{required}/{required}";
            progressBar.fillAmount = 1f;
            
            // Show mission panel with completed info
            ShowMissionPanel("Mission Complete!");
        }
        
        public void ShowMissionPanel(string status = "")
        {
            // Stop any existing fade coroutine
            if (showCoroutine != null)
                StopCoroutine(showCoroutine);
            
            // Start new show coroutine
            showCoroutine = StartCoroutine(ShowMissionPanelCoroutine(status));
        }
        
        private IEnumerator ShowMissionPanelCoroutine(string status)
        {
            // Update the panel title if status is provided
            if (!string.IsNullOrEmpty(status) && missionNameText != null)
            {
                missionNameText.text = $"{status}: {currentMission.missionName}";
            }
            
            // Make sure panel is active
            missionPanel.SetActive(true);
            
            // Fade in
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                float timer = 0f;
                
                while (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
                    yield return null;
                }
                
                canvasGroup.alpha = 1f;
            }
            
            // Wait for display duration
            yield return new WaitForSeconds(showDuration);
            
            // Fade out
            if (canvasGroup != null)
            {
                float timer = 0f;
                
                while (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = 1f - Mathf.Clamp01(timer / fadeDuration);
                    yield return null;
                }
                
                canvasGroup.alpha = 0f;
            }
            
            // Hide panel
            missionPanel.SetActive(false);
        }
    }
}
