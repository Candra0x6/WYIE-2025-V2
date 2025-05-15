using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Mission
{
    // Add menu item to easily create mission data
    public static class MissionDataCreator
    {
        [MenuItem("Assets/Create/Gamekit2D/Mission Data", false, 10)]
        public static void CreateMissionData()
        {
            // Create a new MissionData asset
            MissionData asset = ScriptableObject.CreateInstance<MissionData>();
            
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Missions"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder("Assets/Resources", "Missions");
            }
            
            // Save the asset
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Missions/New Mission.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            
            // Select the created asset
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }

    [CustomEditor(typeof(MissionData))]
    public class MissionDataEditor : Editor
    {
        private SerializedProperty missionNameProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty targetItemIDProperty;
        private SerializedProperty requiredAmountProperty;
        private SerializedProperty healthBonusProperty;
        private SerializedProperty rewardIconProperty;
        private SerializedProperty startDialogueProperty;
        private SerializedProperty completeDialogueProperty;
        private SerializedProperty itemIconProperty;
        
        private void OnEnable()
        {
            missionNameProperty = serializedObject.FindProperty("missionName");
            descriptionProperty = serializedObject.FindProperty("description");
            targetItemIDProperty = serializedObject.FindProperty("targetItemID");
            requiredAmountProperty = serializedObject.FindProperty("requiredAmount");
            healthBonusProperty = serializedObject.FindProperty("healthBonus");
            rewardIconProperty = serializedObject.FindProperty("rewardIcon");
            startDialogueProperty = serializedObject.FindProperty("startDialogue");
            completeDialogueProperty = serializedObject.FindProperty("completeDialogue");
            itemIconProperty = serializedObject.FindProperty("itemIcon");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Mission Information", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(missionNameProperty);
            EditorGUILayout.PropertyField(descriptionProperty, GUILayout.Height(60));
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mission Requirements", EditorStyles.boldLabel);
            
            // Use a special dropdown for targetItemID if possible
            EditorGUILayout.PropertyField(targetItemIDProperty);
            
            // Show available inventory keys in the project
            if (GUILayout.Button("Find Available Item IDs"))
            {
                ShowAvailableItemIDs();
            }
            
            EditorGUILayout.PropertyField(requiredAmountProperty);
            EditorGUILayout.PropertyField(itemIconProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mission Reward", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(healthBonusProperty);
            EditorGUILayout.PropertyField(rewardIconProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dialogue References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(startDialogueProperty);
            EditorGUILayout.PropertyField(completeDialogueProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void ShowAvailableItemIDs()
        {
            // Find all InventoryItem prefabs and list their keys
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            EditorGUILayout.HelpBox("Available Item IDs:", MessageType.Info);
            
            int found = 0;
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    InventoryItem item = prefab.GetComponent<InventoryItem>();
                    if (item != null && !string.IsNullOrEmpty(item.inventoryKey))
                    {
                        EditorGUILayout.LabelField($"• {item.inventoryKey} (from {prefab.name})");
                        found++;
                    }
                }
            }
            
            if (found == 0)
            {
                EditorGUILayout.HelpBox("No inventory items found in project. Create InventoryItem components with inventoryKey values.", MessageType.Warning);
            }
        }
    }
    
    [CustomEditor(typeof(MissionNPC))]
    public class MissionNPCEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            MissionNPC missionNPC = (MissionNPC)target;
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Mission Data Asset"))
            {
                CreateNewMissionData();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Make sure your NPC has a CollisionBox2D with 'Is Trigger' enabled and a DialogueTrigger component.", MessageType.Info);
        }
          private void CreateNewMissionData()
        {
            MissionDataCreator.CreateMissionData();
        }
    }
}
