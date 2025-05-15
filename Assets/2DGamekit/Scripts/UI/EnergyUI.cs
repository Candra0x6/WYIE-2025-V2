using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gamekit2D
{
    public class EnergyUI : MonoBehaviour
    {        [Header("Energy Display")]
        public TextMeshProUGUI energyText;
        public Image energyBar;
        
        [Header("Energy Icons")]
        public GameObject energyIconPrefab;
        public Transform energyIconsParent;
        [SerializeField] private Color activeEnergyColor = Color.yellow;
        [SerializeField] private Color inactiveEnergyColor = Color.gray;
        
        [Header("Regeneration Timer")]
        public GameObject timerDisplayObject;
        public TextMeshProUGUI timerText;
        
        private Image[] energyIcons;
        private EnergyManager energyManager;
        
        private void Awake()
        {
            // Find the energy manager
            energyManager = FindObjectOfType<EnergyManager>();
            
            if (energyManager == null)
            {
                Debug.LogError("EnergyUI: No EnergyManager found in the scene!");
                return;
            }
            
            // Setup energy icons if not using a bar
            if (energyIconPrefab != null && energyIconsParent != null)
            {
                SetupEnergyIcons();
            }
        }
        
        private void OnEnable()
        {
            if (energyManager != null)
            {
                // Subscribe to energy events
                energyManager.OnEnergyChanged.AddListener(UpdateEnergyDisplay);
                
                // Initial update
                UpdateEnergyDisplay(energyManager.GetCurrentEnergy(), energyManager.GetMaxEnergy());
            }
        }
        
        private void OnDisable()
        {
            if (energyManager != null)
            {
                // Unsubscribe from events
                energyManager.OnEnergyChanged.RemoveListener(UpdateEnergyDisplay);
            }
        }
        
        private void Update()
        {
            // Update timer display if energy is not at maximum
            if (energyManager != null && timerDisplayObject != null && timerText != null)
            {
                if (energyManager.GetCurrentEnergy() < energyManager.GetMaxEnergy())
                {
                    timerDisplayObject.SetActive(true);
                    
                    float timeRemaining = energyManager.GetTimeUntilNextEnergy();
                    TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
                    
                    // Format: MM:SS
                    timerText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                else
                {
                    timerDisplayObject.SetActive(false);
                }
            }
        }
        
        private void SetupEnergyIcons()
        {
            // Clear existing icons
            foreach (Transform child in energyIconsParent)
            {
                Destroy(child.gameObject);
            }
            
            int maxEnergy = energyManager.GetMaxEnergy();
            energyIcons = new Image[maxEnergy];
            
            // Create new icons
            for (int i = 0; i < maxEnergy; i++)
            {
                GameObject iconObj = Instantiate(energyIconPrefab, energyIconsParent);
                iconObj.name = $"EnergyIcon_{i}";
                
                Image iconImage = iconObj.GetComponent<Image>();
                if (iconImage != null)
                {
                    energyIcons[i] = iconImage;
                }
            }
        }
        
        public void UpdateEnergyDisplay(int currentEnergy, int maxEnergy)
        {
            // Update text display
            if (energyText != null)
            {
                energyText.text = $"{currentEnergy}/{maxEnergy}";
            }
            
            // Update bar display
            if (energyBar != null)
            {
                energyBar.fillAmount = (float)currentEnergy / maxEnergy;
            }
            
            // Update individual energy icons
            if (energyIcons != null)
            {
                for (int i = 0; i < energyIcons.Length; i++)
                {
                    if (energyIcons[i] != null)
                    {
                        // Icons up to currentEnergy are active, the rest are inactive
                        energyIcons[i].color = i < currentEnergy ? activeEnergyColor : inactiveEnergyColor;
                    }
                }
            }
        }
    }
}
