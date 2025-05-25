using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace Gamekit2D.Mission
{
#if UNITY_EDITOR
    /// <summary>
    /// Editor utility to automatically ~set up the Mission UI system
    /// </summary>
    public class MissionUISetupWizard : EditorWindow
    {        private bool useExistingCanvas = true;
        private Canvas existingCanvas;
        private string newCanvasName = "MissionCanvas";
        private bool createMissionListUI = true;
        private bool createMissionNotificationUI = true;
        private bool createMissionEntryPrefab = true;
        
        // Mission List Settings
        private bool keepOpenAfterToggle = true;
        private bool showNotificationOnNewMission = true;
        
        // Default positioning
        private Vector2 missionListPosition = new Vector2(10, 10);
        private Vector2 missionListSize = new Vector2(300, 400);
        private Vector2 notificationPosition = new Vector2(0, -100);
        private Vector2 notificationSize = new Vector2(500, 150);
        private KeyCode toggleKey = KeyCode.J;
        
        [MenuItem("2D Game Kit/Setup/Mission UI Setup Wizard")]
        public static void ShowWindow()
        {
            GetWindow<MissionUISetupWizard>("Mission UI Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Mission UI Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("This wizard will create all necessary UI elements for the mission system.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            
            // Canvas options
            EditorGUILayout.LabelField("Canvas Settings", EditorStyles.boldLabel);
            useExistingCanvas = EditorGUILayout.Toggle("Use Existing Canvas", useExistingCanvas);
            
            if (useExistingCanvas)
            {
                existingCanvas = EditorGUILayout.ObjectField("Canvas", existingCanvas, typeof(Canvas), true) as Canvas;
            }
            else
            {
                newCanvasName = EditorGUILayout.TextField("New Canvas Name", newCanvasName);
            }
            
            EditorGUILayout.Space();
            
            // UI Component options
            EditorGUILayout.LabelField("UI Components", EditorStyles.boldLabel);
            createMissionListUI = EditorGUILayout.Toggle("Create Mission List UI", createMissionListUI);
            createMissionNotificationUI = EditorGUILayout.Toggle("Create Mission Notification UI", createMissionNotificationUI);
            createMissionEntryPrefab = EditorGUILayout.Toggle("Create Mission Entry Prefab", createMissionEntryPrefab);
              if (createMissionListUI)
            {
                EditorGUI.indentLevel++;
                missionListPosition = EditorGUILayout.Vector2Field("Position", missionListPosition);
                missionListSize = EditorGUILayout.Vector2Field("Size", missionListSize);
                toggleKey = (KeyCode)EditorGUILayout.EnumPopup("Toggle Key", toggleKey);
                keepOpenAfterToggle = EditorGUILayout.Toggle("Keep Open After Toggle", keepOpenAfterToggle);
                showNotificationOnNewMission = EditorGUILayout.Toggle("Show Notification On New Mission", showNotificationOnNewMission);
                EditorGUI.indentLevel--;
            }
            
            if (createMissionNotificationUI)
            {
                EditorGUI.indentLevel++;
                notificationPosition = EditorGUILayout.Vector2Field("Position", notificationPosition);
                notificationSize = EditorGUILayout.Vector2Field("Size", notificationSize);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Setup button
            if (GUILayout.Button("Create Mission UI"))
            {
                CreateMissionUI();
            }
        }
        
        private void CreateMissionUI()
        {
            Canvas canvas = existingCanvas;
            GameObject canvasObj;
            
            // Create or get canvas
            if (!useExistingCanvas || existingCanvas == null)
            {
                // Create new canvas
                canvasObj = new GameObject(newCanvasName);
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                // Add required components
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                
                // Create EventSystem if it doesn't exist
                if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    Debug.Log("Created EventSystem");
                }
                
                Debug.Log($"Created new canvas: {newCanvasName}");
            }
            else
            {
                canvasObj = canvas.gameObject;
                Debug.Log($"Using existing canvas: {canvasObj.name}");
            }
              // Add the MissionListUI component to the canvas
            MissionListUI missionListUI = canvasObj.AddComponent<MissionListUI>();
            missionListUI.toggleKey = toggleKey;
            missionListUI.keepOpenAfterToggle = keepOpenAfterToggle;
            missionListUI.showNotificationOnNewMission = showNotificationOnNewMission;
            
            // Create mission entry prefab
            GameObject missionEntryPrefab = null;
            if (createMissionEntryPrefab)
            {
                missionEntryPrefab = CreateMissionEntryPrefab();
            }
            
            // Create mission list UI
            GameObject missionListPanel = null;
            if (createMissionListUI)
            {
                missionListPanel = CreateMissionListPanel(canvasObj.transform, missionEntryPrefab);
                missionListUI.missionListPanel = missionListPanel;
                missionListUI.missionEntryPrefab = missionEntryPrefab;
                
                // Get content container reference
                Transform contentTransform = missionListPanel.transform.Find("Scroll View/Viewport/Content");
                if (contentTransform != null)
                {
                    missionListUI.missionEntryContainer = contentTransform;
                }
                
                // Get no missions text reference
                Transform noMissionsTextTransform = missionListPanel.transform.Find("NoMissionsText");
                if (noMissionsTextTransform != null)
                {
                    missionListUI.noMissionsText = noMissionsTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }
            
            // Create mission notification UI
            if (createMissionNotificationUI)
            {
                GameObject notificationPanel = CreateMissionNotificationPanel(canvasObj.transform);
                
                // Add MissionUIController component if it doesn't exist
                MissionUIController controller = canvasObj.GetComponent<MissionUIController>();
                if (controller == null)
                {
                    controller = canvasObj.AddComponent<MissionUIController>();
                }
                
                // Set references
                controller.missionPanel = notificationPanel;
                controller.canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
                
                // Get UI element references
                Transform nameTextTransform = notificationPanel.transform.Find("Header");
                if (nameTextTransform != null)
                {
                    controller.missionNameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
                }
                
                Transform descriptionTextTransform = notificationPanel.transform.Find("Description");
                if (descriptionTextTransform != null)
                {
                    controller.missionDescriptionText = descriptionTextTransform.GetComponent<TextMeshProUGUI>();
                }
                
                Transform progressTextTransform = notificationPanel.transform.Find("ProgressText");
                if (progressTextTransform != null)
                {
                    controller.progressText = progressTextTransform.GetComponent<TextMeshProUGUI>();
                }
                
                Transform progressBarTransform = notificationPanel.transform.Find("ProgressBar");
                if (progressBarTransform != null)
                {
                    controller.progressBar = progressBarTransform.GetComponent<Image>();
                }
                
                Transform iconTransform = notificationPanel.transform.Find("Icon");
                if (iconTransform != null)
                {
                    controller.missionIcon = iconTransform.GetComponent<Image>();
                }
            }
            
            // Add MissionHUD to the canvas
            if (canvasObj.GetComponent<MissionHUD>() == null)
            {
                MissionHUD missionHUD = canvasObj.AddComponent<MissionHUD>();
                
                // Create a simple HUD if needed
                if (missionHUD.missionHudPanel == null)
                {
                    GameObject hudPanel = new GameObject("MissionHUDPanel");
                    hudPanel.transform.SetParent(canvasObj.transform, false);
                    
                    RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
                    hudRect.anchorMin = new Vector2(0, 1);
                    hudRect.anchorMax = new Vector2(0, 1);
                    hudRect.pivot = new Vector2(0, 1);
                    hudRect.anchoredPosition = new Vector2(10, -10);
                    hudRect.sizeDelta = new Vector2(200, 80);
                    
                    // Add background image
                    Image bg = hudPanel.AddComponent<Image>();
                    bg.color = new Color(0, 0, 0, 0.5f);
                    
                    // Add mission name text
                    GameObject nameObj = new GameObject("MissionName");
                    nameObj.transform.SetParent(hudPanel.transform, false);
                    RectTransform nameRect = nameObj.AddComponent<RectTransform>();
                    nameRect.anchorMin = new Vector2(0, 1);
                    nameRect.anchorMax = new Vector2(1, 1);
                    nameRect.pivot = new Vector2(0.5f, 1);
                    nameRect.anchoredPosition = Vector2.zero;
                    nameRect.sizeDelta = new Vector2(0, 25);
                    
                    TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
                    nameText.fontSize = 14;
                    nameText.alignment = TextAlignmentOptions.Center;
                    nameText.text = "Mission Name";
                    
                    // Add progress text
                    GameObject progressObj = new GameObject("Progress");
                    progressObj.transform.SetParent(hudPanel.transform, false);
                    RectTransform progressRect = progressObj.AddComponent<RectTransform>();
                    progressRect.anchorMin = new Vector2(1, 0);
                    progressRect.anchorMax = new Vector2(1, 0);
                    progressRect.pivot = new Vector2(1, 0);
                    progressRect.anchoredPosition = new Vector2(-5, 5);
                    progressRect.sizeDelta = new Vector2(50, 25);
                    
                    TextMeshProUGUI progressText = progressObj.AddComponent<TextMeshProUGUI>();
                    progressText.fontSize = 14;
                    progressText.alignment = TextAlignmentOptions.Right;
                    progressText.text = "0/5";
                    
                    // Add progress bar
                    GameObject barObj = new GameObject("ProgressBar");
                    barObj.transform.SetParent(hudPanel.transform, false);
                    RectTransform barRect = barObj.AddComponent<RectTransform>();
                    barRect.anchorMin = new Vector2(0, 0);
                    barRect.anchorMax = new Vector2(1, 0);
                    barRect.pivot = new Vector2(0, 0);
                    barRect.anchoredPosition = new Vector2(0, 5);
                    barRect.sizeDelta = new Vector2(-60, 15);
                    
                    Image barBg = barObj.AddComponent<Image>();
                    barBg.color = new Color(0.2f, 0.2f, 0.2f, 1);
                    
                    GameObject fillObj = new GameObject("Fill");
                    fillObj.transform.SetParent(barObj.transform, false);
                    RectTransform fillRect = fillObj.AddComponent<RectTransform>();
                    fillRect.anchorMin = Vector2.zero;
                    fillRect.anchorMax = Vector2.one;
                    fillRect.pivot = new Vector2(0, 0.5f);
                    fillRect.sizeDelta = Vector2.zero;
                    
                    Image fillImage = fillObj.AddComponent<Image>();
                    fillImage.color = new Color(0.8f, 0.8f, 0.2f, 1);
                    
                    // Add icon image
                    GameObject iconObj = new GameObject("Icon");
                    iconObj.transform.SetParent(hudPanel.transform, false);
                    RectTransform iconRect = iconObj.AddComponent<RectTransform>();
                    iconRect.anchorMin = new Vector2(0, 0.5f);
                    iconRect.anchorMax = new Vector2(0, 0.5f);
                    iconRect.pivot = new Vector2(0, 0.5f);
                    iconRect.anchoredPosition = new Vector2(5, 0);
                    iconRect.sizeDelta = new Vector2(30, 30);
                    
                    Image iconImage = iconObj.AddComponent<Image>();
                    iconImage.color = Color.white;
                    
                    // Set references in MissionHUD
                    missionHUD.missionHudPanel = hudPanel;
                    missionHUD.missionNameText = nameText;
                    missionHUD.progressText = progressText;
                    missionHUD.progressBar = fillImage;
                    missionHUD.itemIcon = iconImage;
                    
                    // Hide the panel at start
                    hudPanel.SetActive(false);
                }
            }
            
            Debug.Log("Mission UI setup complete!");
            Selection.activeGameObject = canvasObj;
            
            // Focus on the created objects in hierarchy
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        }
        
        private GameObject CreateMissionEntryPrefab()
        {
            // Create prefab folder if it doesn't exist
            string prefabFolderPath = "Assets/2DGamekit/Prefabs/UI";
            if (!Directory.Exists(prefabFolderPath))
            {
                Directory.CreateDirectory(prefabFolderPath);
            }
            
            // Check if prefab already exists
            string prefabPath = $"{prefabFolderPath}/MissionEntryPrefab.prefab";
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existingPrefab != null)
            {
                Debug.Log($"Using existing mission entry prefab at {prefabPath}");
                return existingPrefab;
            }
            
            // Create mission entry prefab
            GameObject entryObj = new GameObject("MissionEntryPrefab");
            RectTransform rect = entryObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280, 60);
            
            // Add background image
            Image bg = entryObj.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
            
            // Add MissionEntryUI component
            MissionEntryUI entryUI = entryObj.AddComponent<MissionEntryUI>();
            
            // Add mission name text
            GameObject nameObj = new GameObject("MissionName");
            nameObj.transform.SetParent(entryObj.transform, false);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.pivot = new Vector2(0.5f, 1);
            nameRect.anchoredPosition = new Vector2(10, -5);
            nameRect.sizeDelta = new Vector2(-20, 25);
            
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.fontSize = 14;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.text = "Mission Name";
            
            // Add progress text
            GameObject progressObj = new GameObject("Progress");
            progressObj.transform.SetParent(entryObj.transform, false);
            RectTransform progressRect = progressObj.AddComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(1, 0);
            progressRect.anchorMax = new Vector2(1, 0);
            progressRect.pivot = new Vector2(1, 0);
            progressRect.anchoredPosition = new Vector2(-5, 5);
            progressRect.sizeDelta = new Vector2(50, 25);
            
            TextMeshProUGUI progressText = progressObj.AddComponent<TextMeshProUGUI>();
            progressText.fontSize = 14;
            progressText.alignment = TextAlignmentOptions.Right;
            progressText.text = "0/5";
            
            // Add progress bar
            GameObject barObj = new GameObject("ProgressBar");
            barObj.transform.SetParent(entryObj.transform, false);
            RectTransform barRect = barObj.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(1, 0);
            barRect.pivot = new Vector2(0, 0);
            barRect.anchoredPosition = new Vector2(40, 5);
            barRect.sizeDelta = new Vector2(-90, 15);
            
            Image barBg = barObj.AddComponent<Image>();
            barBg.color = new Color(0.2f, 0.2f, 0.2f, 1);
            
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(barObj.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.pivot = new Vector2(0, 0.5f);
            fillRect.sizeDelta = Vector2.zero;
            
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.8f, 0.8f, 0.2f, 1);
            
            // Add status icon
            GameObject statusObj = new GameObject("StatusIcon");
            statusObj.transform.SetParent(entryObj.transform, false);
            RectTransform statusRect = statusObj.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0, 0.5f);
            statusRect.anchorMax = new Vector2(0, 0.5f);
            statusRect.pivot = new Vector2(0.5f, 0.5f);
            statusRect.anchoredPosition = new Vector2(15, 0);
            statusRect.sizeDelta = new Vector2(20, 20);
            
            Image statusImage = statusObj.AddComponent<Image>();
            statusImage.color = new Color(0.8f, 0.8f, 0.2f, 1);
            
            // Add mission icon
            GameObject iconObj = new GameObject("MissionIcon");
            iconObj.transform.SetParent(entryObj.transform, false);
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = new Vector2(280, 0);
            iconRect.sizeDelta = new Vector2(30, 30);
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.white;
            
            // Set references in MissionEntryUI component
            entryUI.missionNameText = nameText;
            entryUI.progressText = progressText;
            entryUI.progressBar = fillImage;
            entryUI.statusIcon = statusImage;
            entryUI.missionIcon = iconImage;
            
            // Create the prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(entryObj, prefabPath);
            DestroyImmediate(entryObj);
            
            Debug.Log($"Created mission entry prefab at {prefabPath}");
            
            return prefab;
        }
        
        private GameObject CreateMissionListPanel(Transform parent, GameObject entryPrefab)
        {
            // Create mission list panel
            GameObject panel = new GameObject("MissionListPanel");
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = missionListPosition;
            rect.sizeDelta = missionListSize;
            
            // Add background image
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);
            
            // Add panel header
            GameObject headerObj = new GameObject("Header");
            headerObj.transform.SetParent(panel.transform, false);
            RectTransform headerRect = headerObj.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 40);
            
            Image headerBg = headerObj.AddComponent<Image>();
            headerBg.color = new Color(0.3f, 0.3f, 0.3f, 1);
            
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(headerObj.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = Vector2.zero;
            titleRect.anchorMax = Vector2.one;
            titleRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.fontSize = 18;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.text = "Active Missions";
            
            // Add scroll view
            GameObject scrollObj = new GameObject("Scroll View");
            scrollObj.transform.SetParent(panel.transform, false);
            RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.pivot = new Vector2(0.5f, 0.5f);
            scrollRect.anchoredPosition = new Vector2(0, -20);
            scrollRect.sizeDelta = new Vector2(0, -40);
            
            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            
            // Add viewport
            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollObj.transform, false);
            RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.pivot = new Vector2(0, 1);
            viewportRect.sizeDelta = Vector2.zero;
            
            Image viewportImage = viewportObj.AddComponent<Image>();
            viewportImage.color = new Color(1, 1, 1, 0.01f);
            
            Mask viewportMask = viewportObj.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            // Add content container
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);
            
            // Add a vertical layout group to content
            VerticalLayoutGroup layout = contentObj.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            
            // Add a content size fitter
            ContentSizeFitter fitter = contentObj.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Set up the scroll rect
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.scrollSensitivity = 10f;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            
            // Add "No Missions" text
            GameObject noMissionsObj = new GameObject("NoMissionsText");
            noMissionsObj.transform.SetParent(panel.transform, false);
            RectTransform noMissionsRect = noMissionsObj.AddComponent<RectTransform>();
            noMissionsRect.anchorMin = new Vector2(0, 0.5f);
            noMissionsRect.anchorMax = new Vector2(1, 0.5f);
            noMissionsRect.sizeDelta = new Vector2(0, 40);
            
            TextMeshProUGUI noMissionsText = noMissionsObj.AddComponent<TextMeshProUGUI>();
            noMissionsText.fontSize = 16;
            noMissionsText.alignment = TextAlignmentOptions.Center;
            noMissionsText.text = "No active missions";
            noMissionsText.color = new Color(0.7f, 0.7f, 0.7f, 1);
            
            // Hide panel at start
            panel.SetActive(false);
            
            return panel;
        }
        
        private GameObject CreateMissionNotificationPanel(Transform parent)
        {
            // Create notification panel
            GameObject panel = new GameObject("MissionNotificationPanel");
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = notificationPosition;
            rect.sizeDelta = notificationSize;
            
            // Add background image
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);
            
            // Add canvas group for fading
            CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
            
            // Add header text
            GameObject headerObj = new GameObject("Header");
            headerObj.transform.SetParent(panel.transform, false);
            RectTransform headerRect = headerObj.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 40);
            
            TextMeshProUGUI headerText = headerObj.AddComponent<TextMeshProUGUI>();
            headerText.fontSize = 24;
            headerText.fontStyle = FontStyles.Bold;
            headerText.alignment = TextAlignmentOptions.Center;
            headerText.text = "New Mission: Mission Name";
            
            // Add description text
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(panel.transform, false);
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.5f);
            descRect.anchorMax = new Vector2(1, 0.5f);
            descRect.pivot = new Vector2(0.5f, 0.5f);
            descRect.anchoredPosition = new Vector2(0, 0);
            descRect.sizeDelta = new Vector2(-20, 60);
            
            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.fontSize = 16;
            descText.alignment = TextAlignmentOptions.Center;
            descText.text = "Mission description goes here. Collect items, defeat enemies, etc.";
            
            // Add progress text
            GameObject progressObj = new GameObject("ProgressText");
            progressObj.transform.SetParent(panel.transform, false);
            RectTransform progressRect = progressObj.AddComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(1, 0);
            progressRect.anchorMax = new Vector2(1, 0);
            progressRect.pivot = new Vector2(1, 0);
            progressRect.anchoredPosition = new Vector2(-10, 20);
            progressRect.sizeDelta = new Vector2(50, 25);
            
            TextMeshProUGUI progressText = progressObj.AddComponent<TextMeshProUGUI>();
            progressText.fontSize = 14;
            progressText.alignment = TextAlignmentOptions.Right;
            progressText.text = "0/5";
            
            // Add progress bar
            GameObject barObj = new GameObject("ProgressBar");
            barObj.transform.SetParent(panel.transform, false);
            RectTransform barRect = barObj.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(1, 0);
            barRect.pivot = new Vector2(0, 0);
            barRect.anchoredPosition = new Vector2(0, 10);
            barRect.sizeDelta = new Vector2(-80, 15);
            
            Image barBg = barObj.AddComponent<Image>();
            barBg.color = new Color(0.2f, 0.2f, 0.2f, 1);
            
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(barObj.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.pivot = new Vector2(0, 0.5f);
            fillRect.sizeDelta = Vector2.zero;
            
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.8f, 0.8f, 0.2f, 1);
            
            // Add icon
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(panel.transform, false);
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0);
            iconRect.anchorMax = new Vector2(0, 0);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = new Vector2(30, 20);
            iconRect.sizeDelta = new Vector2(40, 40);
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.white;
            
            // Hide panel at start
            panel.SetActive(false);
            
            return panel;
        }
    }
#endif
}
