using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Mission
{
    public class MissionDataCreationWindow : EditorWindow
    {
        [MenuItem("Window/2D GameKit/Create Mission Data", false, 100)]
        public static void ShowWindow()
        {
            MissionDataCreationWindow window = GetWindow<MissionDataCreationWindow>("Create Mission Data");
            window.minSize = new Vector2(300, 200);
            window.Show();
        }

        private string missionName = "New Mission";
        private string description = "Collect the required items";
        private string targetItemID = "";
        private int requiredAmount = 5;
        private float healthBonus = 1f;

        private void OnGUI()
        {
            GUILayout.Label("Create New Mission", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            missionName = EditorGUILayout.TextField("Mission Name", missionName);
            
            GUILayout.Label("Description");
            description = EditorGUILayout.TextArea(description, GUILayout.Height(60));
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mission Requirements", EditorStyles.boldLabel);
            
            targetItemID = EditorGUILayout.TextField("Target Item ID", targetItemID);
            requiredAmount = EditorGUILayout.IntField("Required Amount", requiredAmount);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Reward", EditorStyles.boldLabel);
            
            healthBonus = EditorGUILayout.FloatField("Health Bonus", healthBonus);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("List Available Item IDs"))
            {
                ShowAvailableItemIDs();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Mission Data"))
            {
                CreateMissionData();
            }
        }
        
        private void CreateMissionData()
        {
            // Create a new MissionData asset
            MissionData asset = ScriptableObject.CreateInstance<MissionData>();
            
            // Set properties
            asset.missionName = missionName;
            asset.description = description;
            asset.targetItemID = targetItemID;
            asset.requiredAmount = requiredAmount;
            asset.healthBonus = healthBonus;
            
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Missions"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder("Assets/Resources", "Missions");
            }
            
            // Save the asset with the specified name
            string sanitizedName = string.IsNullOrEmpty(missionName) ? "New Mission" : missionName;
            sanitizedName = string.Join("_", sanitizedName.Split(System.IO.Path.GetInvalidFileNameChars()));
            
            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Resources/Missions/{sanitizedName}.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            
            // Select the created asset
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            
            // Show confirmation
            EditorUtility.DisplayDialog("Mission Created", $"Mission '{missionName}' created at {path}", "OK");
            
            // Close window
            Close();
        }
        
        private void ShowAvailableItemIDs()
        {
            // Create a new window to show available item IDs
            ItemIDListWindow window = GetWindow<ItemIDListWindow>("Available Item IDs");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
    }
    
    public class ItemIDListWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        
        private void OnGUI()
        {
            GUILayout.Label("Available Item IDs", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These are the inventory items found in your project. Click an ID to copy it.", MessageType.Info);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Find all prefabs with InventoryItem component
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            int found = 0;
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    Gamekit2D.InventoryItem item = prefab.GetComponent<Gamekit2D.InventoryItem>();
                    if (item != null && !string.IsNullOrEmpty(item.inventoryKey))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"• {item.inventoryKey}");
                        GUILayout.Label($"(from {prefab.name})", EditorStyles.miniLabel);
                        
                        if (GUILayout.Button("Use This ID", GUILayout.Width(100)))
                        {
                            // Find the parent window and set the ID
                            MissionDataCreationWindow parentWindow = GetWindow<MissionDataCreationWindow>();
                            if (parentWindow != null)
                            {
                                // Use reflection to set the targetItemID field
                                System.Reflection.FieldInfo field = typeof(MissionDataCreationWindow).GetField("targetItemID", 
                                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                
                                if (field != null)
                                    field.SetValue(parentWindow, item.inventoryKey);
                                
                                parentWindow.Repaint();
                            }
                            else
                            {
                                // If we can't find the parent window, just copy to clipboard
                                EditorGUIUtility.systemCopyBuffer = item.inventoryKey;
                                Debug.Log($"Copied '{item.inventoryKey}' to clipboard.");
                            }
                            
                            Close();
                        }
                        GUILayout.EndHorizontal();
                        found++;
                    }
                }
            }
            
            if (found == 0)
            {
                EditorGUILayout.HelpBox("No inventory items found in project. Create InventoryItem components with inventoryKey values.", MessageType.Warning);
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
}
