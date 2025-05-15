using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Gamekit2D
{
    #if UNITY_EDITOR
    public static class EnergyIconCreator
    {
        [MenuItem("2D GameKit/Create/UI/Energy Icon Prefab")]
        public static void CreateEnergyIconPrefab()
        {
            // Create the parent game object for our icon
            GameObject energyIcon = new GameObject("EnergyIcon");
            energyIcon.AddComponent<RectTransform>();
            
            // Create the background image
            GameObject background = new GameObject("Background");
            background.transform.SetParent(energyIcon.transform);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            SetStretch(bgRect);
            
            // Create the foreground image (the actual energy icon)
            GameObject foreground = new GameObject("Foreground");
            foreground.transform.SetParent(energyIcon.transform);
            Image fgImage = foreground.AddComponent<Image>();
            fgImage.color = Color.yellow;
            RectTransform fgRect = foreground.GetComponent<RectTransform>();
            SetStretch(fgRect, 0.1f);
            
            // Set icon size
            RectTransform iconRect = energyIcon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(30f, 30f);
            
            // Save the prefab
            string prefabPath = "Assets/2DGamekit/Prefabs/UI/EnergyIcon.prefab";
            
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(energyIcon, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, energyIcon);
            #endif
            
            Debug.Log("Energy icon prefab created at: " + prefabPath);
            
            // Clean up the scene object
            Object.DestroyImmediate(energyIcon);
        }
        
        [MenuItem("2D GameKit/Create/UI/Energy Panel")]
        public static void CreateEnergyPanel()
        {
            // Create the parent panel
            GameObject energyPanel = new GameObject("EnergyPanel");
            RectTransform panelRect = energyPanel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(200f, 80f);
            
            // Add energy UI component
            EnergyUI energyUI = energyPanel.AddComponent<EnergyUI>();
            
            // Create the energy text display
            GameObject textObj = new GameObject("EnergyText");
            textObj.transform.SetParent(energyPanel.transform);
            TextMeshProUGUI energyText = textObj.AddComponent<TextMeshProUGUI>();
            energyText.text = "6/6";
            energyText.fontSize = 24;
            energyText.alignment = TextAlignmentOptions.Center;
            energyText.color = Color.white;
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchoredPosition = new Vector2(0f, 30f);
            textRect.sizeDelta = new Vector2(100f, 30f);
            
            // Create energy bar background
            GameObject barBg = new GameObject("EnergyBarBg");
            barBg.transform.SetParent(energyPanel.transform);
            Image barBgImage = barBg.AddComponent<Image>();
            barBgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform barBgRect = barBg.GetComponent<RectTransform>();
            barBgRect.anchoredPosition = new Vector2(0f, 0f);
            barBgRect.sizeDelta = new Vector2(180f, 20f);
            
            // Create energy bar fill
            GameObject barFill = new GameObject("EnergyBar");
            barFill.transform.SetParent(barBg.transform);
            Image barFillImage = barFill.AddComponent<Image>();
            barFillImage.color = Color.yellow;
            RectTransform barFillRect = barFill.GetComponent<RectTransform>();
            barFillRect.pivot = new Vector2(0f, 0.5f);
            barFillRect.anchorMin = new Vector2(0f, 0f);
            barFillRect.anchorMax = new Vector2(1f, 1f);
            barFillRect.offsetMin = new Vector2(2f, 2f);
            barFillRect.offsetMax = new Vector2(-2f, -2f);
            
            // Create timer display
            GameObject timerObj = new GameObject("TimerDisplay");
            timerObj.transform.SetParent(energyPanel.transform);
            TextMeshProUGUI timerText = timerObj.AddComponent<TextMeshProUGUI>();
            timerText.text = "Next: 03:00";
            timerText.fontSize = 16;
            timerText.alignment = TextAlignmentOptions.Center;
            timerText.color = Color.white;
            RectTransform timerRect = timerObj.GetComponent<RectTransform>();
            timerRect.anchoredPosition = new Vector2(0f, -25f);
            timerRect.sizeDelta = new Vector2(160f, 20f);
            
            // Create icons container
            GameObject iconsContainer = new GameObject("EnergyIcons");
            iconsContainer.transform.SetParent(energyPanel.transform);
            HorizontalLayoutGroup layout = iconsContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            RectTransform iconsRect = iconsContainer.GetComponent<RectTransform>();
            iconsRect.anchoredPosition = new Vector2(0f, -50f);
            iconsRect.sizeDelta = new Vector2(180f, 30f);
            
            // Set references
            energyUI.energyText = energyText;
            energyUI.energyBar = barFillImage;
            energyUI.energyIconsParent = iconsContainer.transform;
            energyUI.timerDisplayObject = timerObj;
            energyUI.timerText = timerText;
            
            // Save the prefab
            string prefabPath = "Assets/2DGamekit/Prefabs/UI/EnergyPanel.prefab";
            
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(energyPanel, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, energyPanel);
            #endif
            
            Debug.Log("Energy panel prefab created at: " + prefabPath);
            
            // Clean up the scene object
            Object.DestroyImmediate(energyPanel);
        }
        
        [MenuItem("2D GameKit/Create/UI/Not Enough Energy Panel")]
        public static void CreateNotEnoughEnergyPanel()
        {
            // Create the parent panel
            GameObject panel = new GameObject("NotEnoughEnergyPanel");
            panel.AddComponent<CanvasGroup>();
            NotEnoughEnergyPanel panelScript = panel.AddComponent<NotEnoughEnergyPanel>();
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(400f, 250f);
            
            // Create the background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(panel.transform);
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            SetStretch(bgRect);
            
            // Create title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panel.transform);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Not Enough Energy";
            titleText.fontSize = 26;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0f, 90f);
            titleRect.sizeDelta = new Vector2(380f, 40f);
            
            // Create message text
            GameObject messageObj = new GameObject("Message");
            messageObj.transform.SetParent(panel.transform);
            TextMeshProUGUI messageText = messageObj.AddComponent<TextMeshProUGUI>();
            messageText.text = "You need 3 energy to enter this level.\nYou currently have 1 energy.";
            messageText.fontSize = 20;
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.color = Color.white;
            RectTransform messageRect = messageObj.GetComponent<RectTransform>();
            messageRect.anchoredPosition = new Vector2(0f, 30f);
            messageRect.sizeDelta = new Vector2(360f, 60f);
            
            // Create timer text
            GameObject timerObj = new GameObject("Timer");
            timerObj.transform.SetParent(panel.transform);
            TextMeshProUGUI timerText = timerObj.AddComponent<TextMeshProUGUI>();
            timerText.text = "Next energy in: 02:45";
            timerText.fontSize = 18;
            timerText.alignment = TextAlignmentOptions.Center;
            timerText.color = Color.yellow;
            RectTransform timerRect = timerObj.GetComponent<RectTransform>();
            timerRect.anchoredPosition = new Vector2(0f, -10f);
            timerRect.sizeDelta = new Vector2(360f, 30f);
            
            // Create close button
            GameObject closeButton = new GameObject("CloseButton");
            closeButton.transform.SetParent(panel.transform);
            Button closeButtonComp = closeButton.AddComponent<Button>();
            Image closeButtonImage = closeButton.AddComponent<Image>();
            closeButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            closeButtonComp.targetGraphic = closeButtonImage;
            RectTransform closeButtonRect = closeButton.GetComponent<RectTransform>();
            closeButtonRect.anchoredPosition = new Vector2(0f, -70f);
            closeButtonRect.sizeDelta = new Vector2(120f, 40f);
            
            // Create button text
            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(closeButton.transform);
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "Close";
            buttonText.fontSize = 20;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            SetStretch(buttonTextRect);
            
            // Set references
            panelScript.messageText = messageText;
            panelScript.timerText = timerText;
            panelScript.closeButton = closeButtonComp;
            
            // Save the prefab
            string prefabPath = "Assets/2DGamekit/Prefabs/UI/NotEnoughEnergyPanel.prefab";
            
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, panel);
            #endif
            
            Debug.Log("Not Enough Energy panel prefab created at: " + prefabPath);
            
            // Clean up the scene object
            Object.DestroyImmediate(panel);
        }
        
        private static void SetStretch(RectTransform rect, float padding = 0f)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }
    }
    #endif
}
