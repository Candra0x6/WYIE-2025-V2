using UnityEngine;
using UnityEditor;

namespace Gamekit2D.Mission
{
    public class MissionSystemSetup : EditorWindow
    {
        [MenuItem("2D GameKit/Setup Mission System", false, 100)]
        public static void ShowWindow()
        {
            GetWindow<MissionSystemSetup>("Mission System Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Mission System Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("This will set up the complete mission system in your scene.", MessageType.Info);
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Setup Complete Mission System"))
            {
                SetupMissionSystem();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Individual Components", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Mission Manager"))
            {
                MissionUISetup.CreateMissionManager();
            }
            
            if (GUILayout.Button("Add Mission UI"))
            {
                MissionUISetup.CreateMissionUI();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Sample Mission Data"))
            {
                CreateSampleMission();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Make sure to place mission data assets in Resources/Missions folder for proper loading.", MessageType.Warning);
        }
        
        private void SetupMissionSystem()
        {
            // Create mission manager
            MissionUISetup.CreateMissionManager();
            
            // Create mission UI
            MissionUISetup.CreateMissionUI();
            
            // Create resources folder if needed
            CreateResourcesFolder();
            
            // Create sample mission data
            CreateSampleMission();
            
            Debug.Log("Mission system setup complete!");
        }
        
        private void CreateResourcesFolder()
        {
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Missions"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Missions");
            }
        }
        
        private void CreateSampleMission()
        {
            // Create mission data asset
            MissionData sampleMission = ScriptableObject.CreateInstance<MissionData>();
            sampleMission.missionName = "Collect the Gems";
            sampleMission.description = "Find and collect 5 gems scattered around the level.";
            sampleMission.targetItemID = "gem";
            sampleMission.requiredAmount = 5;
            sampleMission.healthBonus = 2f;
            
            // Create resources folder if needed
            CreateResourcesFolder();
            
            // Save the asset
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Missions/CollectGems.asset");
            AssetDatabase.CreateAsset(sampleMission, path);
            AssetDatabase.SaveAssets();
            
            // Select the created asset
            Selection.activeObject = sampleMission;
            EditorGUIUtility.PingObject(sampleMission);
            
            Debug.Log("Sample mission created at: " + path);
        }
    }
}
