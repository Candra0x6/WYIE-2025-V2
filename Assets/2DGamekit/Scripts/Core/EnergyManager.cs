using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Gamekit2D
{    
    public class EnergyManager : MonoBehaviour, IDataPersister
    {
        public static EnergyManager Instance { get; private set; }

        [Header("Energy Settings")]
        [SerializeField] private int maxEnergy = 6;
        [SerializeField] private int currentEnergy = 6;
        [SerializeField] public int energyCostPerLevel = 3;
        [SerializeField] private float regenerationTimeMinutes = 3f;
        
        [Header("UI References")]
        [SerializeField] private GameObject notEnoughEnergyPanel;
        
        [Header("Data Persistence")]
        [SerializeField] private DataSettings dataSettings;
        
        [System.Serializable]
        public class EnergyEvent : UnityEvent<int, int> { } // current, max
        
        public EnergyEvent OnEnergyChanged;
        public EnergyEvent OnEnergyRegenerated;
        
        private float _regenerationTimeSeconds;
        private float _timeSinceLastRegeneration = 0f;
        private DateTime _lastSessionTime;
        
        private const string ENERGY_PREFS_KEY = "PlayerEnergyAmount";
        private const string LAST_TIME_PREFS_KEY = "PlayerEnergyLastTime";

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Register with the persistence system
                PersistentDataManager.RegisterPersister(this);
                
                InitializeEnergy();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            // Convert minutes to seconds for easier calculation
            _regenerationTimeSeconds = regenerationTimeMinutes * 60f;
            
            // Initialize UI reference
            if (notEnoughEnergyPanel != null)
                notEnoughEnergyPanel.SetActive(false);
        }

        private void Start()
        {
            // Register to scene loading events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Unregister from scene loading events
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // Unregister from the persistence system
            PersistentDataManager.UnregisterPersister(this);
            
            // Save energy when the game is closed
            SaveEnergyData();
        }

        private void Update()
        {
            // Handle energy regeneration
            if (currentEnergy < maxEnergy)
            {
                _timeSinceLastRegeneration += Time.deltaTime;
                
                if (_timeSinceLastRegeneration >= _regenerationTimeSeconds)
                {
                    RegenerateEnergy(1);
                    _timeSinceLastRegeneration = 0f;
                }
            }
        }
        
        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                // App is being paused, save the current time
                SaveEnergyData();
            }
            else
            {
                // App is being resumed, calculate offline energy regeneration
                CalculateOfflineRegeneration();
            }
        }

        private void InitializeEnergy()
        {
            LoadEnergyData();
            
            // Notify any listeners about the current energy
            if (OnEnergyChanged != null)
                OnEnergyChanged.Invoke(currentEnergy, maxEnergy);
        }
        
        public bool TrySpendEnergy(int amount)
        {
            if (currentEnergy >= amount)
            {
                currentEnergy -= amount;
                
                if (OnEnergyChanged != null)
                    OnEnergyChanged.Invoke(currentEnergy, maxEnergy);
                
                SaveEnergyData();
                return true;
            }
            
            return false;
        }
        
        public void RegenerateEnergy(int amount)
        {
            int previousEnergy = currentEnergy;
            currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
            
            if (previousEnergy != currentEnergy)
            {
                if (OnEnergyChanged != null)
                    OnEnergyChanged.Invoke(currentEnergy, maxEnergy);
                
                if (OnEnergyRegenerated != null)
                    OnEnergyRegenerated.Invoke(currentEnergy, maxEnergy);
                
                SaveEnergyData();
            }
        }
        
        public bool CanEnterLevel()
        {
            return currentEnergy >= energyCostPerLevel;
        }
        
        public void LoadLevel(string sceneName)
        {
            if (CanEnterLevel())
            {
                TrySpendEnergy(energyCostPerLevel);
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                ShowNotEnoughEnergyPanel();
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Hide the not enough energy panel when a new scene is loaded
            if (notEnoughEnergyPanel != null)
                notEnoughEnergyPanel.SetActive(false);
        }
        
        public void ShowNotEnoughEnergyPanel()
        {
            if (notEnoughEnergyPanel != null)
                notEnoughEnergyPanel.SetActive(true);
        }
        
        public void CloseNotEnoughEnergyPanel()
        {
            if (notEnoughEnergyPanel != null)
                notEnoughEnergyPanel.SetActive(false);
        }
        
        public int GetCurrentEnergy()
        {
            return currentEnergy;
        }
        
        public int GetMaxEnergy()
        {
            return maxEnergy;
        }
        
        public float GetTimeUntilNextEnergy()
        {
            if (currentEnergy >= maxEnergy)
                return 0f;
                
            return _regenerationTimeSeconds - _timeSinceLastRegeneration;
        }
        
        // Save energy data to PlayerPrefs
        private void SaveEnergyData()
        {
            PlayerPrefs.SetInt(ENERGY_PREFS_KEY, currentEnergy);
            
            // Save current time as a string
            string currentTime = DateTime.UtcNow.ToString("o"); // ISO 8601 format
            PlayerPrefs.SetString(LAST_TIME_PREFS_KEY, currentTime);
            
            PlayerPrefs.Save();
        }
        
        // Load energy data from PlayerPrefs
        private void LoadEnergyData()
        {
            if (PlayerPrefs.HasKey(ENERGY_PREFS_KEY))
            {
                currentEnergy = PlayerPrefs.GetInt(ENERGY_PREFS_KEY);
            }
            else
            {
                // First time playing, set to max energy
                currentEnergy = maxEnergy;
            }
            
            // Cap energy at the maximum value (in case maxEnergy was changed in settings)
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
        
        // Calculate energy regeneration that happened while the game was closed
        private void CalculateOfflineRegeneration()
        {
            if (PlayerPrefs.HasKey(LAST_TIME_PREFS_KEY))
            {
                string lastTimeStr = PlayerPrefs.GetString(LAST_TIME_PREFS_KEY);
                
                try
                {
                    DateTime lastTime = DateTime.Parse(lastTimeStr);
                    TimeSpan timeDifference = DateTime.UtcNow - lastTime;
                    
                    // Calculate how many energy points should have regenerated
                    int energyToRegenerate = Mathf.FloorToInt((float)timeDifference.TotalSeconds / _regenerationTimeSeconds);
                    
                    if (energyToRegenerate > 0)
                    {
                        RegenerateEnergy(energyToRegenerate);
                    }
                    
                    // Set the timer for partial regeneration
                    float remainingSeconds = (float)timeDifference.TotalSeconds % _regenerationTimeSeconds;
                    _timeSinceLastRegeneration = remainingSeconds;
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing last session time: " + e.Message);
                    _timeSinceLastRegeneration = 0f;
                }
            }
        }
        
        #region IDataPersister implementation
        
        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<int, float>(currentEnergy, _timeSinceLastRegeneration);
        }

        public void LoadData(Data data)
        {
            var energyData = data as Data<int, float>;
            if (energyData != null)
            {
                currentEnergy = Mathf.Min(energyData.value0, maxEnergy);
                _timeSinceLastRegeneration = energyData.value1;
                
                // Notify any listeners about the loaded energy value
                if (OnEnergyChanged != null)
                    OnEnergyChanged.Invoke(currentEnergy, maxEnergy);
            }
        }
        
        #endregion
    }
}
