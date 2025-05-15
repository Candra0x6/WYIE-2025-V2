using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit2D.Mission
{
    // Utility class to create UI prefabs for the mission system
    public static class MissionUISetup
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/2D GameKit/Mission/Create Mission UI", false, 10)]
        public static void CreateMissionUI()
        {
            // Check if Canvas exists, if not create one
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // Create Canvas
                GameObject canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                // Add Canvas Scaler
                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                // Add Graphic Raycaster
                canvasObject.AddComponent<GraphicRaycaster>();
            }
            
            // Create Mission UI parent
            GameObject missionUIObject = new GameObject("MissionUI");
            missionUIObject.transform.SetParent(canvas.transform, false);
            MissionUIController missionUIController = missionUIObject.AddComponent<MissionUIController>();
            
            // Create notification panel
            GameObject notificationPanel = CreatePanel("NotificationPanel", missionUIObject.transform);
            RectTransform notificationRect = notificationPanel.GetComponent<RectTransform>();
            notificationRect.anchorMin = new Vector2(0.5f, 1);
            notificationRect.anchorMax = new Vector2(0.5f, 1);
            notificationRect.pivot = new Vector2(0.5f, 1);
            notificationRect.anchoredPosition = new Vector2(0, -100);
            notificationRect.sizeDelta = new Vector2(600, 200);
            
            // Add components to notification panel
            CanvasGroup canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            Image panelImage = notificationPanel.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            
            // Create header
            GameObject headerObject = CreateTextObject("Header", notificationPanel.transform, "New Mission");
            RectTransform headerRect = headerObject.GetComponent<RectTransform>();
            headerRect.anchoredPosition = new Vector2(0, -30);
            headerRect.sizeDelta = new Vector2(580, 50);
            TextMeshProUGUI headerText = headerObject.GetComponent<TextMeshProUGUI>();
            headerText.fontSize = 24;
            headerText.alignment = TextAlignmentOptions.Center;
            headerText.fontStyle = FontStyles.Bold;
            headerText.color = Color.yellow;
            
            // Create description
            GameObject descriptionObject = CreateTextObject("Description", notificationPanel.transform, "Collect the required items");
            RectTransform descriptionRect = descriptionObject.GetComponent<RectTransform>();
            descriptionRect.anchoredPosition = new Vector2(0, -80);
            descriptionRect.sizeDelta = new Vector2(580, 60);
            TextMeshProUGUI descriptionText = descriptionObject.GetComponent<TextMeshProUGUI>();
            descriptionText.fontSize = 18;
            descriptionText.alignment = TextAlignmentOptions.Center;
            
            // Create progress container
            GameObject progressContainer = CreatePanel("ProgressContainer", notificationPanel.transform);
            RectTransform progressContainerRect = progressContainer.GetComponent<RectTransform>();
            progressContainerRect.anchoredPosition = new Vector2(0, -150);
            progressContainerRect.sizeDelta = new Vector2(300, 40);
            Image progressContainerImage = progressContainer.GetComponent<Image>();
            progressContainerImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Create progress bar
            GameObject progressBar = CreatePanel("ProgressBar", progressContainer.transform);
            RectTransform progressBarRect = progressBar.GetComponent<RectTransform>();
            progressBarRect.anchorMin = Vector2.zero;
            progressBarRect.anchorMax = new Vector2(0.3f, 1);
            progressBarRect.pivot = new Vector2(0, 0.5f);
            progressBarRect.offsetMin = Vector2.zero;
            progressBarRect.offsetMax = Vector2.zero;
            Image progressBarImage = progressBar.GetComponent<Image>();
            progressBarImage.color = Color.green;
            
            // Create progress text
            GameObject progressTextObject = CreateTextObject("ProgressText", progressContainer.transform, "0/5");
            RectTransform progressTextRect = progressTextObject.GetComponent<RectTransform>();
            progressTextRect.anchoredPosition = Vector2.zero;
            progressTextRect.sizeDelta = new Vector2(200, 30);
            TextMeshProUGUI progressText = progressTextObject.GetComponent<TextMeshProUGUI>();
            progressText.fontSize = 18;
            progressText.alignment = TextAlignmentOptions.Center;
            
            // Create mission icon
            GameObject iconObject = new GameObject("MissionIcon", typeof(RectTransform));
            iconObject.transform.SetParent(notificationPanel.transform, false);
            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.anchoredPosition = new Vector2(-250, -80);
            iconRect.sizeDelta = new Vector2(64, 64);
            Image iconImage = iconObject.AddComponent<Image>();
            iconImage.preserveAspect = true;
            
            // Create HUD panel
            GameObject hudPanel = CreatePanel("MissionHUD", missionUIObject.transform);
            RectTransform hudRect = hudPanel.GetComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 1);
            hudRect.anchorMax = new Vector2(0, 1);
            hudRect.pivot = new Vector2(0, 1);
            hudRect.anchoredPosition = new Vector2(20, -20);
            hudRect.sizeDelta = new Vector2(300, 80);
            Image hudPanelImage = hudPanel.GetComponent<Image>();
            hudPanelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Create HUD Components
            GameObject hudHeaderObject = CreateTextObject("HUDTitle", hudPanel.transform, "Current Mission");
            RectTransform hudHeaderRect = hudHeaderObject.GetComponent<RectTransform>();
            hudHeaderRect.anchoredPosition = new Vector2(100, -20);
            hudHeaderRect.sizeDelta = new Vector2(200, 30);
            TextMeshProUGUI hudHeaderText = hudHeaderObject.GetComponent<TextMeshProUGUI>();
            hudHeaderText.fontSize = 16;
            hudHeaderText.fontStyle = FontStyles.Bold;
            
            // Create HUD Progress
            GameObject hudProgressObject = CreateTextObject("HUDProgress", hudPanel.transform, "0/5");
            RectTransform hudProgressRect = hudProgressObject.GetComponent<RectTransform>();
            hudProgressRect.anchoredPosition = new Vector2(100, -50);
            hudProgressRect.sizeDelta = new Vector2(200, 20);
            TextMeshProUGUI hudProgressText = hudProgressObject.GetComponent<TextMeshProUGUI>();
            hudProgressText.fontSize = 14;
            
            // Create HUD Progress Bar
            GameObject hudProgressContainer = CreatePanel("HUDProgressContainer", hudPanel.transform);
            RectTransform hudProgressContainerRect = hudProgressContainer.GetComponent<RectTransform>();
            hudProgressContainerRect.anchoredPosition = new Vector2(180, -50);
            hudProgressContainerRect.sizeDelta = new Vector2(100, 10);
            Image hudProgressContainerImage = hudProgressContainer.GetComponent<Image>();
            hudProgressContainerImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            GameObject hudProgressBar = CreatePanel("HUDProgressBar", hudProgressContainer.transform);
            RectTransform hudProgressBarRect = hudProgressBar.GetComponent<RectTransform>();
            hudProgressBarRect.anchorMin = Vector2.zero;
            hudProgressBarRect.anchorMax = new Vector2(0.5f, 1);
            hudProgressBarRect.pivot = new Vector2(0, 0.5f);
            hudProgressBarRect.offsetMin = Vector2.zero;
            hudProgressBarRect.offsetMax = Vector2.zero;
            Image hudProgressBarImage = hudProgressBar.GetComponent<Image>();
            hudProgressBarImage.color = Color.green;
            
            // Create HUD Icon
            GameObject hudIconObject = new GameObject("HUDMissionIcon", typeof(RectTransform));
            hudIconObject.transform.SetParent(hudPanel.transform, false);
            RectTransform hudIconRect = hudIconObject.GetComponent<RectTransform>();
            hudIconRect.anchoredPosition = new Vector2(35, -40);
            hudIconRect.sizeDelta = new Vector2(50, 50);
            Image hudIconImage = hudIconObject.AddComponent<Image>();
            hudIconImage.preserveAspect = true;
            
            // Set up MissionUIController references
            missionUIController.missionPanel = notificationPanel;
            missionUIController.missionNameText = headerText;
            missionUIController.missionDescriptionText = descriptionText;
            missionUIController.progressText = progressText;
            missionUIController.progressBar = progressBarImage;
            missionUIController.missionIcon = iconImage;
            missionUIController.canvasGroup = canvasGroup;
            
            // Set up MissionHUD component
            MissionHUD missionHUD = missionUIObject.AddComponent<MissionHUD>();
            missionHUD.missionHudPanel = hudPanel;
            missionHUD.missionNameText = hudHeaderText;
            missionHUD.progressText = hudProgressText;
            missionHUD.progressBar = hudProgressBarImage;
            missionHUD.itemIcon = hudIconImage;
            
            // Hide notification panel
            notificationPanel.SetActive(false);
            
            // Select the created object
            Selection.activeGameObject = missionUIObject;
        }
        
        [MenuItem("GameObject/2D GameKit/Mission/Create Mission Manager", false, 10)]
        public static void CreateMissionManager()
        {
            // Create mission manager object
            GameObject managerObject = new GameObject("MissionManager");
            MissionManager manager = managerObject.AddComponent<MissionManager>();
            
            // Set up data settings
            manager.dataSettings = new DataSettings
            {
                dataTag = "mission",
                persistenceType = DataSettings.PersistenceType.ReadWrite
            };
            
            // Initialize events
            manager.OnMissionAssigned = new MissionManager.MissionEvent();
            manager.OnMissionUpdated = new MissionManager.MissionEvent();
            manager.OnMissionCompleted = new MissionManager.MissionEvent();
            
            // Select the created object
            Selection.activeGameObject = managerObject;
        }
        
        // Utility method to create a panel with Image component
        private static GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            Image image = panel.AddComponent<Image>();
            return panel;
        }
        
        // Utility method to create a text object with TextMeshPro
        private static GameObject CreateTextObject(string name, Transform parent, string text)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.color = Color.white;
            return textObject;
        }
#endif
    }
}
