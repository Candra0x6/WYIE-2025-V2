# Cross-Scene Mission Data Guide

This guide explains how to use the mission system to save and transfer mission data between different scenes in your Unity project.

## Table of Contents

1. [Overview](#overview)
2. [Basic Scene Transitions](#basic-scene-transitions)
3. [Advanced Mission State Persistence](#advanced-mission-state-persistence)
4. [Implementation Examples](#implementation-examples)
5. [Troubleshooting](#troubleshooting)

## Overview

The mission system consists of two main components:

1. **MissionManager**: Handles core mission functionality (assignment, progress tracking, completion)
2. **MissionStatePersistence**: Handles additional mission state data between scenes

Together, these systems allow missions to persist as players move between different scenes in your game.

## Basic Scene Transitions

### Setting Up Scene Transitions

The MissionManager already uses `DontDestroyOnLoad` to persist between scenes. To ensure your mission data transfers correctly:

1. **Initialize MissionManager in your starting scene**:
   ```csharp
   // Place this in your game initialization
   if (MissionManager.Instance == null)
   {
       GameObject missionManagerObj = new GameObject("MissionManager");
       missionManagerObj.AddComponent<MissionManager>();
   }
   ```

2. **Use standard scene loading**:
   ```csharp
   // Load a new scene
   SceneManager.LoadScene("YourNextScene");
   ```

3. **Access mission data in the new scene**:
   ```csharp
   void Start()
   {
       // The MissionManager persists from the previous scene
       if (MissionManager.Instance != null)
       {
           // Access mission data
           bool hasMission = MissionManager.Instance.IsMissionActive(yourMissionData);
       }
   }
   ```

## Advanced Mission State Persistence

The MissionStatePersistence class allows you to store additional data related to missions that the standard MissionManager doesn't track.

### Setup

1. **Add MissionStatePersistence to your scene**:
   ```csharp
   // Add alongside MissionManager during initialization
   if (MissionStatePersistence.Instance == null)
   {
       GameObject obj = new GameObject("MissionStatePersistence");
       obj.AddComponent<MissionStatePersistence>();
   }
   ```

2. **Configure Data Settings in the Inspector**:
   - Set a unique `dataTag` for your mission persistence data
   - Choose the appropriate `persistenceType` (usually SaveToPlayerPrefs)

### Saving Custom Mission State

```csharp
// Save mission-specific data
MissionStatePersistence.Instance.SetMissionStateValue("CollectGems", "gemLocations", "1,3,5,7");
MissionStatePersistence.Instance.SetMissionStateValue("DefeatBoss", "bossHealth", 500);
MissionStatePersistence.Instance.SetMissionStateValue("OpenGate", "isGateUnlocked", true);

// Save scene-specific mission data
MissionStatePersistence.Instance.SetMissionStateValue("Forest", "CollectHerbs", "herbsFound", "2,4");
```

### Loading Custom Mission State

```csharp
// Load different data types
string gemLocations = MissionStatePersistence.Instance.GetMissionStateValue("CollectGems", "gemLocations");
int bossHealth = MissionStatePersistence.Instance.GetMissionStateValueInt("DefeatBoss", "bossHealth");
bool isGateUnlocked = MissionStatePersistence.Instance.GetMissionStateValueBool("OpenGate", "isGateUnlocked");

// Load scene-specific data
string herbsFound = MissionStatePersistence.Instance.GetMissionStateValue("Forest", "CollectHerbs", "herbsFound");
```

### Forcing Save Before Important Transitions

```csharp
// Save all mission state data before a critical scene transition
MissionStatePersistence.Instance.SaveAllMissionStates();
SceneManager.LoadScene("BossLevel");
```

## Implementation Examples

### Example 1: Item Collection Mission with Scene Changes

```csharp
// In SceneA
public class GemCollector : MonoBehaviour
{
    public MissionData collectGemsData;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Update mission progress
            MissionManager.Instance.UpdateMissionProgress("gem", 1);
            
            // Save which gem was collected
            string gemId = gameObject.name;
            string collectedGems = MissionStatePersistence.Instance.GetMissionStateValue(collectGemsData.name, "collectedGems", "");
            
            if (!string.IsNullOrEmpty(collectedGems))
            {
                collectedGems += ",";
            }
            collectedGems += gemId;
            
            MissionStatePersistence.Instance.SetMissionStateValue(collectGemsData.name, "collectedGems", collectedGems);
            
            // Destroy the gem
            Destroy(gameObject);
        }
    }
}
```

```csharp
// In SceneB
public class GemRestorer : MonoBehaviour
{
    public MissionData collectGemsData;
    public GameObject[] gemObjects;
    
    void Start()
    {
        // Check which gems were already collected
        string collectedGems = MissionStatePersistence.Instance.GetMissionStateValue(collectGemsData.name, "collectedGems", "");
        
        if (!string.IsNullOrEmpty(collectedGems))
        {
            string[] gemIds = collectedGems.Split(',');
            
            // Disable already collected gems
            foreach (string gemId in gemIds)
            {
                foreach (GameObject gem in gemObjects)
                {
                    if (gem.name == gemId)
                    {
                        gem.SetActive(false);
                        break;
                    }
                }
            }
        }
    }
}
```

### Example 2: Boss Fight Mission with Persistent Health

```csharp
// Boss script that maintains health between scene reloads
public class BossController : MonoBehaviour
{
    public MissionData bossData;
    public int maxHealth = 1000;
    private int currentHealth;
    
    void Start()
    {
        // Load boss health from persistence
        currentHealth = MissionStatePersistence.Instance.GetMissionStateValueInt(bossData.name, "bossHealth", maxHealth);
        
        // Update health bar UI
        UpdateHealthUI();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Save current health
        MissionStatePersistence.Instance.SetMissionStateValue(bossData.name, "bossHealth", currentHealth);
        
        if (currentHealth <= 0)
        {
            // Complete mission
            MissionManager.Instance.UpdateMissionProgress(bossData.targetItemID, 1);
            
            // Boss defeated
            Destroy(gameObject);
        }
        
        // Update health bar UI
        UpdateHealthUI();
    }
    
    void UpdateHealthUI()
    {
        // Update your health bar UI based on currentHealth/maxHealth
    }
}
```

## Troubleshooting

### Common Issues

1. **Mission data doesn't persist between scenes**
   - Check that MissionManager and MissionStatePersistence have DontDestroyOnLoad set
   - Verify that DataSettings are properly configured
   - Ensure Resources/Missions folder contains all mission data assets

2. **Multiple instances of managers exist**
   - Check your singleton implementation in Awake() methods
   - Use GameObject.FindObjectOfType to find existing instances before creating new ones

3. **Serialization errors**
   - The MissionStatePersistence class converts complex data to strings
   - For more complex data, consider using JSON serialization within the string values

4. **Scene-specific objects don't update properly**
   - Subscribe to SceneManager.sceneLoaded events
   - Use OnEnable to check mission state when objects are activated

### Best Practices

1. **Use descriptive identifiers** - Make mission IDs and state keys clear and descriptive
2. **Save after important changes** - Call SaveAllMissionStates() after critical mission updates
3. **Validate loaded data** - Always check for null or default values when loading mission state
4. **Document your state keys** - Keep track of what state keys each mission uses
5. **Test scene transitions thoroughly** - Create test scenes to verify data persistence
