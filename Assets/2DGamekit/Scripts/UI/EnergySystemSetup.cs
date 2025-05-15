using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Gamekit2D
{
    #if UNITY_EDITOR
    public class EnergySystemSetup : EditorWindow
    {
        private bool setupEnergyManager = true;
        private bool createUIPrefabs = true;
        private bool modifyTransitionPoints = true;

        private int maxEnergy = 6;
        private int energyCostPerLevel = 3;
        private float regenerationTimeMinutes = 3f;

        [MenuItem("2D GameKit/Energy System/Setup")]
        static void Init()
        {
            EnergySystemSetup window = (EnergySystemSetup)EditorWindow.GetWindow(typeof(EnergySystemSetup));
            window.titleContent = new GUIContent("Energy System Setup");
            window.minSize = new Vector2(350, 350);
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Energy System Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("This wizard will set up the energy system in your game.", MessageType.Info);
            EditorGUILayout.Space();

            setupEnergyManager = EditorGUILayout.Toggle("Create Energy Manager", setupEnergyManager);
            createUIPrefabs = EditorGUILayout.Toggle("Create UI Prefabs", createUIPrefabs);
            modifyTransitionPoints = EditorGUILayout.Toggle("Update Transition Points", modifyTransitionPoints);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            maxEnergy = EditorGUILayout.IntSlider("Max Energy", maxEnergy, 1, 20);
            energyCostPerLevel = EditorGUILayout.IntSlider("Energy Cost Per Level", energyCostPerLevel, 1, 10);
            regenerationTimeMinutes = EditorGUILayout.Slider("Regeneration Time (minutes)", regenerationTimeMinutes, 0.5f, 30f);
            EditorGUILayout.Space();

            if (GUILayout.Button("Set Up Energy System"))
            {
                SetupEnergySystem();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Note: This will create prefabs in your project. Make sure to add the EnergyManager to your scene and configure the UI references manually.", MessageType.Warning);
        }

        private void SetupEnergySystem()
        {
            // Create required directories
            if (!AssetDatabase.IsValidFolder("Assets/2DGamekit/Prefabs/UI"))
            {
                AssetDatabase.CreateFolder("Assets/2DGamekit/Prefabs", "UI");
            }

            // Create Energy Manager GameObject if requested
            if (setupEnergyManager)
            {
                CreateEnergyManager();
            }

            // Create UI Prefabs
            if (createUIPrefabs)
            {
                CreateUIPrefabs();
            }

            // Update Transition Points
            if (modifyTransitionPoints)
            {
                UpdateTransitionPoints();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Setup Complete", 
                "Energy system setup complete!\n\n" +
                "1. Add the EnergyManager prefab to your start scene\n" +
                "2. Assign the NotEnoughEnergyPanel to the EnergyManager\n" +
                "3. Add the EnergyPanel to your UI canvas", "OK");
        }

        private void CreateEnergyManager()
        {
            // Create Energy Manager GameObject
            GameObject managerObj = new GameObject("EnergyManager");
            EnergyManager manager = managerObj.AddComponent<EnergyManager>();

            // Get the field using reflection to set the DataSettings since it's private
            var dataSettingsField = typeof(EnergyManager).GetField("dataSettings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (dataSettingsField != null)
            {
                var dataSettings = new DataSettings();
                dataSettings.dataTag = "EnergyData";
                dataSettings.persistenceType = DataSettings.PersistenceType.ReadWrite;
                dataSettingsField.SetValue(manager, dataSettings);
            }

            // Set properties via serialized object
            SerializedObject serializedObj = new SerializedObject(manager);
            serializedObj.FindProperty("maxEnergy").intValue = maxEnergy;
            serializedObj.FindProperty("currentEnergy").intValue = maxEnergy;
            serializedObj.FindProperty("energyCostPerLevel").intValue = energyCostPerLevel;
            serializedObj.FindProperty("regenerationTimeMinutes").floatValue = regenerationTimeMinutes;
            serializedObj.ApplyModifiedProperties();

            // Save prefab
            string prefabPath = "Assets/2DGamekit/Prefabs/UI/EnergyManager.prefab";
            
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(managerObj, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, managerObj);
            #endif
            
            Object.DestroyImmediate(managerObj);
            Debug.Log("Energy Manager prefab created: " + prefabPath);
        }

        private void CreateUIPrefabs()
        {
            // Create the Energy Icon prefab
            EnergyIconCreator.CreateEnergyIconPrefab();
            
            // Create the Energy Panel prefab
            EnergyIconCreator.CreateEnergyPanel();
            
            // Create the Not Enough Energy Panel prefab
            EnergyIconCreator.CreateNotEnoughEnergyPanel();
        }

        private void UpdateTransitionPoints()
        {
            // Find all transition points in scenes
            TransitionPoint[] transitionPoints = FindObjectsOfType<TransitionPoint>();
            
            if (transitionPoints.Length > 0)
            {
                int modified = 0;
                foreach (TransitionPoint tp in transitionPoints)
                {
                    // Only modify transition points that transition to a different zone
                    if (tp.transitionType == TransitionPoint.TransitionType.DifferentZone)
                    {
                        SerializedObject serializedObj = new SerializedObject(tp);
                        serializedObj.FindProperty("requiresEnergy").boolValue = true;
                        serializedObj.ApplyModifiedProperties();
                        modified++;
                    }
                }
                
                Debug.Log($"Updated {modified} of {transitionPoints.Length} transition points to require energy");
            }
            else
            {
                Debug.Log("No transition points found in open scenes");
            }
        }
    }
    #endif
}
