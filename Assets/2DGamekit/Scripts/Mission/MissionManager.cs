using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit2D.Mission
{
    public class MissionManager : MonoBehaviour, IDataPersister
    {
        public static MissionManager Instance { get; private set; }
        
        [System.Serializable]
        public class MissionInfo
        {
            public MissionData missionData;
            public bool isActive;
            public bool isCompleted;
            public int currentAmount;
        }
        
        [System.Serializable]
        public class MissionEvent : UnityEvent<MissionData, int, int> { } // mission, current, required
        
        [Header("Events")]
        public MissionEvent OnMissionAssigned;
        public MissionEvent OnMissionUpdated;
        public MissionEvent OnMissionCompleted;
        
        [Header("Data Persistence")]
        public DataSettings dataSettings;
        
        // Active missions list
        private List<MissionInfo> activeMissions = new List<MissionInfo>();
        
        // Items collected during missions before returning to NPC
        private Dictionary<string, int> pendingItems = new Dictionary<string, int>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            // Find and subscribe to inventory events
            InventoryController inventoryController = FindObjectOfType<InventoryController>();
            if (inventoryController != null)
            {
                inventoryController.OnInventoryLoaded += OnInventoryLoaded;
            }
        }
        
        private void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
            
            // Unsubscribe from inventory events
            InventoryController inventoryController = FindObjectOfType<InventoryController>();
            if (inventoryController != null)
            {
                inventoryController.OnInventoryLoaded -= OnInventoryLoaded;
            }
        }
        
        private void OnInventoryLoaded()
        {
            // When inventory is loaded, we may need to update mission status
            CheckAllMissionsStatus();
        }
        
        public void AssignMission(MissionData missionData)
        {
            if (missionData == null) return;
            
            // Check if mission is already assigned
            foreach (var mission in activeMissions)
            {
                if (mission.missionData == missionData)
                {
                    // Mission already assigned, just update UI
                    if (OnMissionUpdated != null)
                    {
                        OnMissionUpdated.Invoke(mission.missionData, mission.currentAmount, mission.missionData.requiredAmount);
                    }
                    return;
                }
            }
            
            // Create new mission info
            MissionInfo newMission = new MissionInfo
            {
                missionData = missionData,
                isActive = true,
                isCompleted = false,
                currentAmount = 0
            };
            
            // Add to active missions
            activeMissions.Add(newMission);
            
            // Trigger mission assigned event
            if (OnMissionAssigned != null)
            {
                OnMissionAssigned.Invoke(missionData, 0, missionData.requiredAmount);
            }
            
            // Save mission data
            PersistentDataManager.SetDirty(this);
        }
        
        public void UpdateMissionProgress(string itemID, int amount = 1)
        {
            bool anyMissionUpdated = false;
            
            // Update any mission that requires this item
            foreach (var mission in activeMissions)
            {
                if (mission.isActive && !mission.isCompleted && mission.missionData.targetItemID == itemID)
                {
                    // Update mission progress
                    mission.currentAmount += amount;
                    anyMissionUpdated = true;
                    
                    // Track pending items
                    if (!pendingItems.ContainsKey(itemID))
                    {
                        pendingItems[itemID] = 0;
                    }
                    pendingItems[itemID] += amount;
                    
                    // Check if mission is completed
                    if (mission.currentAmount >= mission.missionData.requiredAmount)
                    {
                        mission.isCompleted = true;
                        
                        // Trigger mission completed event
                        if (OnMissionCompleted != null)
                        {
                            OnMissionCompleted.Invoke(mission.missionData, mission.currentAmount, mission.missionData.requiredAmount);
                        }
                    }
                    else
                    {
                        // Trigger mission updated event
                        if (OnMissionUpdated != null)
                        {
                            OnMissionUpdated.Invoke(mission.missionData, mission.currentAmount, mission.missionData.requiredAmount);
                        }
                    }
                }
            }
            
            // Save mission data if any missions were updated
            if (anyMissionUpdated)
            {
                PersistentDataManager.SetDirty(this);
            }
        }
        
        public void CompleteMission(MissionData missionData)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.missionData == missionData && mission.isCompleted)
                {
                    // Apply reward
                    ApplyReward(mission.missionData);
                    
                    // Clear pending items for this mission
                    if (pendingItems.ContainsKey(mission.missionData.targetItemID))
                    {
                        pendingItems.Remove(mission.missionData.targetItemID);
                    }
                    
                    // Save mission data
                    PersistentDataManager.SetDirty(this);
                    return;
                }
            }
        }
          private void ApplyReward(MissionData missionData)
        {
            // Find player and apply health bonus
            PlayerCharacter player = PlayerCharacter.PlayerInstance;
            if (player != null)
            {
                Damageable damageable = player.GetComponent<Damageable>();
                if (damageable != null)
                {
                    // Increase max health (need to modify startingHealth directly)
                    damageable.startingHealth += Mathf.RoundToInt(missionData.healthBonus);
                    
                    // Heal player to new max health
                    damageable.SetHealth(damageable.startingHealth);
                }
            }
        }
        
        public void CheckAllMissionsStatus()
        {
            // Check inventory for all mission items
            InventoryController inventory = FindObjectOfType<InventoryController>();
            if (inventory == null) return;
            
            foreach (var mission in activeMissions)
            {
                if (mission.isActive && !mission.isCompleted && inventory.HasItem(mission.missionData.targetItemID))
                {
                    // If inventory has the item, update mission progress
                    mission.currentAmount = mission.missionData.requiredAmount;
                    mission.isCompleted = true;
                    
                    // Trigger mission completed event
                    if (OnMissionCompleted != null)
                    {
                        OnMissionCompleted.Invoke(mission.missionData, mission.currentAmount, mission.missionData.requiredAmount);
                    }
                }
            }
        }
        
        // Check if a mission is active
        public bool IsMissionActive(MissionData missionData)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.missionData == missionData && mission.isActive)
                {
                    return true;
                }
            }
            return false;
        }
        
        // Check if a mission is completed but not yet turned in
        public bool IsMissionCompleted(MissionData missionData)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.missionData == missionData && mission.isCompleted)
                {
                    return true;
                }
            }
            return false;
        }
        
        // Get current progress for a mission
        public int GetMissionProgress(MissionData missionData)
        {
            foreach (var mission in activeMissions)
            {
                if (mission.missionData == missionData)
                {
                    return mission.currentAmount;
                }
            }
            return 0;
        }

        #region Data Persistence
        
        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        [System.Serializable]
        private class SavedMission
        {
            public string missionName; // Used to lookup mission data
            public bool isActive;
            public bool isCompleted;
            public int currentAmount;
        }

        public Data SaveData()
        {
            // Convert missions to serializable format
            List<SavedMission> savedMissions = new List<SavedMission>();
            
            foreach (var mission in activeMissions)
            {
                savedMissions.Add(new SavedMission
                {
                    missionName = mission.missionData.name, // ScriptableObject asset name
                    isActive = mission.isActive,
                    isCompleted = mission.isCompleted,
                    currentAmount = mission.currentAmount
                });
            }
            
            return new Data<List<SavedMission>>(savedMissions);
        }

        public void LoadData(Data data)
        {
            Data<List<SavedMission>> missionData = (Data<List<SavedMission>>)data;
            activeMissions.Clear();
            
            if (missionData.value != null)
            {
                // Load all MissionData assets to match with saved missions
                MissionData[] allMissions = Resources.LoadAll<MissionData>("Missions");
                
                foreach (var savedMission in missionData.value)
                {
                    // Find matching mission data asset
                    foreach (var missionAsset in allMissions)
                    {
                        if (missionAsset.name == savedMission.missionName)
                        {
                            // Recreate mission info
                            MissionInfo missionInfo = new MissionInfo
                            {
                                missionData = missionAsset,
                                isActive = savedMission.isActive,
                                isCompleted = savedMission.isCompleted,
                                currentAmount = savedMission.currentAmount
                            };
                            
                            activeMissions.Add(missionInfo);
                            
                            // Update UI for this mission
                            if (savedMission.isCompleted)
                            {
                                if (OnMissionCompleted != null)
                                {
                                    OnMissionCompleted.Invoke(missionAsset, savedMission.currentAmount, missionAsset.requiredAmount);
                                }
                            }
                            else if (savedMission.isActive)
                            {
                                if (OnMissionUpdated != null)
                                {
                                    OnMissionUpdated.Invoke(missionAsset, savedMission.currentAmount, missionAsset.requiredAmount);
                                }
                            }
                            
                            break;
                        }
                    }
                }
            }
        }
        
        #endregion
    }
}
