using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gamekit2D
{
    public class NotEnoughEnergyPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI messageText;
        public TextMeshProUGUI timerText;
        public Button closeButton;
        [SerializeField] private Button getEnergyButton; // Optional button for energy purchases or rewards
        
        [Header("Settings")]
        [SerializeField] private string messageTemplate = "You need {0} energy to enter this level. You currently have {1} energy.";
        
        private EnergyManager energyManager;
        
        private void Awake()
        {
            // Find the energy manager
            energyManager = FindObjectOfType<EnergyManager>();
            
            if (energyManager == null)
            {
                Debug.LogError("NotEnoughEnergyPanel: No EnergyManager found in the scene!");
                return;
            }
            
            // Set up close button
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => 
                {
                    gameObject.SetActive(false);
                });
            }
            
            // Set up get energy button (if implemented)
            if (getEnergyButton != null)
            {
                getEnergyButton.onClick.AddListener(OnGetEnergyButtonClicked);
            }
            
            // Hide panel on start
            gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            if (energyManager != null)
            {
                UpdateUI();
            }
        }
        
        private void Update()
        {
            // Update the timer if the panel is active
            if (gameObject.activeSelf && timerText != null && energyManager != null)
            {
                float timeRemaining = energyManager.GetTimeUntilNextEnergy();
                System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeRemaining);
                
                // Format: MM:SS
                timerText.text = $"Next energy in: {timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
        }
        
        private void UpdateUI()
        {
            if (messageText != null && energyManager != null)
            {
                // Update message with energy requirements
                messageText.text = string.Format(messageTemplate, 
                    energyManager.energyCostPerLevel, 
                    energyManager.GetCurrentEnergy());
            }
        }
        
        private void OnGetEnergyButtonClicked()
        {
            // This could be implemented to show ads, IAP, or other ways to get energy
            Debug.Log("Get Energy button clicked - implement your energy acquisition logic here");
            
            // For testing, add some energy
            if (energyManager != null)
            {
                energyManager.RegenerateEnergy(1);
                UpdateUI();
            }
        }
    }
}
