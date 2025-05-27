# Mission Persistence Between Scenes Guide

This guide explains how to save and load mission state data between different scenes in your Unity project.

## Understanding Mission Persistence

The Mission System in this project is designed to track player progress in missions across scene transitions. This is achieved through:

1. **DontDestroyOnLoad** - The MissionManager instance persists between scenes
2. **Data Persistence** - Mission data is saved and loaded using a serialization system
3. **ScriptableObjects** - Mission definitions are stored as ScriptableObjects

## Setup Instructions

### 1. Ensure Mission Resources are Properly Organized

Place all MissionData ScriptableObjects in a `Resources/Missions` folder:

```
Assets/
  Resources/
    Missions/
      MissionA.asset
      MissionB.asset
      MissionC.asset
```

This allows the MissionManager to find and load mission definitions from any scene.

### 2. Configure Scene Transitions

When changing scenes, you have two options:

#### Option A: Using SceneManager.LoadScene (Recommended)

The MissionManager object has `DontDestroyOnLoad` set in its Awake method. This means it will persist naturally between scenes when you use:

```csharp
using UnityEngine.SceneManagement;

// Load a new scene
SceneManager.LoadScene("YourSceneName");
```

#### Option B: Manual Data Saving Before Scene Transitions

If you need to ensure data is explicitly saved before a scene change:

```csharp
// Before changing scenes, force a save
PersistentDataManager.SetDirty(MissionManager.Instance);
PersistentDataManager.SaveAllData();

// Then load the scene
SceneManager.LoadScene("YourSceneName");
```

### 3. Handling Mission Data Between Scenes

For any scene that needs access to mission data:

1. Ensure the scene has a reference to the MissionManager (it should persist from the previous scene)
2. Use MissionManager.Instance to access mission data
3. Connect UI elements to the MissionManager events (OnMissionAssigned, OnMissionUpdated, etc.)

Example:

```csharp
void Start()
{
    // Connect to mission events
    MissionManager.Instance.OnMissionUpdated.AddListener(HandleMissionUpdated);
    MissionManager.Instance.OnMissionCompleted.AddListener(HandleMissionCompleted);
    
    // Update UI with current mission state
    UpdateMissionUI();
}

void UpdateMissionUI()
{
    // Get active missions from MissionManager.Instance.activeMissions
    // Update your UI accordingly
}
```

## Advanced: Creating a Mission State Manager

For more complex games, you might want to create a dedicated MissionStateManager class that handles scene-specific mission states:

```csharp
using UnityEngine;
using Gamekit2D.Mission;

public class MissionStateManager : MonoBehaviour
{
    public static MissionStateManager Instance { get; private set; }
    
    // Track scene-specific mission states
    private Dictionary<string, Dictionary<string, object>> sceneSpecificMissionStates = new Dictionary<string, Dictionary<string, object>>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Save scene-specific mission state
    public void SaveSceneMissionState(string sceneName, string missionId, object state)
    {
        if (!sceneSpecificMissionStates.ContainsKey(sceneName))
        {
            sceneSpecificMissionStates[sceneName] = new Dictionary<string, object>();
        }
        
        sceneSpecificMissionStates[sceneName][missionId] = state;
    }
    
    // Get scene-specific mission state
    public T GetSceneMissionState<T>(string sceneName, string missionId)
    {
        if (sceneSpecificMissionStates.ContainsKey(sceneName) && 
            sceneSpecificMissionStates[sceneName].ContainsKey(missionId))
        {
            return (T)sceneSpecificMissionStates[sceneName][missionId];
        }
        
        return default(T);
    }
}
```

## Common Issues and Solutions

### Issue: Mission data is lost between scenes

**Solution:** 
- Verify that MissionManager has DontDestroyOnLoad set
- Check that mission data is properly saved before scene transitions
- Ensure mission assets are in Resources/Missions folder

### Issue: Multiple MissionManagers exist after scene transitions

**Solution:**
- Check that MissionManager.Awake() properly destroys duplicate instances
- Use a proper Singleton pattern

### Issue: Mission UI doesn't update in new scenes

**Solution:**
- Connect UI elements to MissionManager events in each scene
- Call MissionManager.Instance methods to refresh UI when the scene loads

## Best Practices

1. **Use ScriptableObjects for mission definitions** - Keeps mission data separate from scene-specific code
2. **Centralize mission logic in MissionManager** - Avoid duplicating mission logic in different scenes
3. **Use events for UI updates** - The MissionManager events provide a clean way to update UI elements
4. **Save explicitly before critical scene transitions** - Use PersistentDataManager.SaveAllData() before important scene changes

By following this guide, your mission data will persist properly between scenes, allowing for a seamless player experience throughout your game.
