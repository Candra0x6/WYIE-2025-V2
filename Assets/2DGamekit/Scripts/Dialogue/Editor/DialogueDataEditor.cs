using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gamekit2D.Dialogue
{
    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : Editor
    {
        private bool[] foldouts;
        
        private void OnEnable()
        {
            DialogueData dialogueData = (DialogueData)target;
            InitializeFoldouts(dialogueData);
        }
        
        private void InitializeFoldouts(DialogueData dialogueData)
        {
            if (dialogueData.nodes == null)
                dialogueData.nodes = new List<DialogueNode>();
                
            foldouts = new bool[dialogueData.nodes.Count];
            for (int i = 0; i < foldouts.Length; i++)
                foldouts[i] = false;
        }
        
        public override void OnInspectorGUI()
        {
            DialogueData dialogueData = (DialogueData)target;
            
            serializedObject.Update();
            
            // Audio settings
            EditorGUILayout.LabelField("Audio Settings", EditorStyles.boldLabel);
            dialogueData.startDialogueSound = (AudioClip)EditorGUILayout.ObjectField("Start Dialogue Sound", dialogueData.startDialogueSound, typeof(AudioClip), false);
            dialogueData.advanceDialogueSound = (AudioClip)EditorGUILayout.ObjectField("Advance Dialogue Sound", dialogueData.advanceDialogueSound, typeof(AudioClip), false);
            
            EditorGUILayout.Space();
            
            // Dialogue nodes
            EditorGUILayout.LabelField("Dialogue Nodes", EditorStyles.boldLabel);
            
            if (dialogueData.nodes == null)
                dialogueData.nodes = new List<DialogueNode>();
            
            if (foldouts == null || foldouts.Length != dialogueData.nodes.Count)
                InitializeFoldouts(dialogueData);
            
            for (int i = 0; i < dialogueData.nodes.Count; i++)
            {
                DialogueNode node = dialogueData.nodes[i];
                
                EditorGUILayout.BeginVertical(GUI.skin.box);
                
                // Node header with delete button
                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Node {i}: {(string.IsNullOrEmpty(node.speakerName) ? "No Speaker" : node.speakerName)}");
                
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Delete Node", "Are you sure you want to delete this dialogue node?", "Yes", "No"))
                    {
                        // Update references to this node in other nodes
                        for (int j = 0; j < dialogueData.nodes.Count; j++)
                        {
                            var otherNode = dialogueData.nodes[j];
                            
                            // Fix nextNodeIndex references
                            if (otherNode.nextNodeIndex > i)
                                otherNode.nextNodeIndex--;
                            else if (otherNode.nextNodeIndex == i)
                                otherNode.nextNodeIndex = -1;
                                
                            // Fix option nextNodeIndex references
                            if (otherNode.options != null)
                            {
                                foreach (var option in otherNode.options)
                                {
                                    if (option.nextNodeIndex > i)
                                        option.nextNodeIndex--;
                                    else if (option.nextNodeIndex == i)
                                        option.nextNodeIndex = -1;
                                }
                            }
                        }
                        
                        // Remove the node
                        dialogueData.nodes.RemoveAt(i);
                        ArrayUtility.RemoveAt(ref foldouts, i);
                        
                        // Force the inspector to redraw
                        EditorUtility.SetDirty(target);
                        
                        // Exit early to avoid accessing deleted node
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        continue;
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (foldouts[i])
                {
                    // Speaker name and image
                    EditorGUILayout.LabelField("Speaker Settings", EditorStyles.boldLabel);
                    node.speakerName = EditorGUILayout.TextField("Speaker Name", node.speakerName);
                    node.speakerImage = (Sprite)EditorGUILayout.ObjectField("Speaker Image", node.speakerImage, typeof(Sprite), false);
                    
                    // Dialogue text
                    EditorGUILayout.LabelField("Dialogue Text", EditorStyles.boldLabel);
                    node.text = EditorGUILayout.TextArea(node.text, GUILayout.Height(60));
                    
                    EditorGUILayout.Space();
                    
                    // Node connections
                    EditorGUILayout.LabelField("Node Connections", EditorStyles.boldLabel);
                    
                    // Options
                    if (node.options == null)
                        node.options = new List<DialogueOption>();
                        
                    if (node.options.Count == 0)
                    {
                        // No options, so display basic next node field
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Next Node:", GUILayout.Width(70));
                        
                        // Create a dropdown list of available nodes, plus "End Dialogue" option
                        string[] nodeOptions = new string[dialogueData.nodes.Count + 1];
                        nodeOptions[0] = "End Dialogue";
                        for (int j = 0; j < dialogueData.nodes.Count; j++)
                        {
                            nodeOptions[j + 1] = "Node " + j;
                        }
                        
                        int selectedIndex = node.nextNodeIndex + 1; // +1 because "End Dialogue" is at index 0
                        selectedIndex = EditorGUILayout.Popup(selectedIndex, nodeOptions);
                        node.nextNodeIndex = selectedIndex - 1; // -1 to match our internal indexing
                        
                        EditorGUILayout.EndHorizontal();
                        
                        if (GUILayout.Button("Add Response Option"))
                        {
                            node.options.Add(new DialogueOption { text = "New response", nextNodeIndex = -1 });
                        }
                    }
                    else
                    {
                        // Has options, display them
                        EditorGUILayout.LabelField("Response Options:", EditorStyles.boldLabel);
                        
                        for (int j = 0; j < node.options.Count; j++)
                        {
                            DialogueOption option = node.options[j];
                            
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField($"Option {j+1}:", GUILayout.Width(60));
                            
                            if (GUILayout.Button("×", GUILayout.Width(20)))
                            {
                                node.options.RemoveAt(j);
                                j--;
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                                continue;
                            }
                            EditorGUILayout.EndHorizontal();
                            
                            option.text = EditorGUILayout.TextField("Text:", option.text);
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Leads to:", GUILayout.Width(70));
                            
                            // Create a dropdown list of available nodes, plus "End Dialogue" option
                            string[] nodeOptions = new string[dialogueData.nodes.Count + 1];
                            nodeOptions[0] = "End Dialogue";
                            for (int k = 0; k < dialogueData.nodes.Count; k++)
                            {
                                nodeOptions[k + 1] = "Node " + k;
                            }
                            
                            int selectedIndex = option.nextNodeIndex + 1; // +1 because "End Dialogue" is at index 0
                            selectedIndex = EditorGUILayout.Popup(selectedIndex, nodeOptions);
                            option.nextNodeIndex = selectedIndex - 1; // -1 to match our internal indexing
                            
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            
                            EditorGUILayout.Space();
                        }
                        
                        if (GUILayout.Button("Add Response Option"))
                        {
                            node.options.Add(new DialogueOption { text = "New response", nextNodeIndex = -1 });
                        }
                        
                        if (GUILayout.Button("Remove All Options & Use Linear Flow"))
                        {
                            node.options.Clear();
                            node.nextNodeIndex = -1;
                        }
                    }
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            if (GUILayout.Button("Add Node"))
            {
                dialogueData.nodes.Add(new DialogueNode());
                ArrayUtility.Add(ref foldouts, true); // Auto-expand new node
            }
            
            serializedObject.ApplyModifiedProperties();
            
            // Set the object as dirty if we've modified it
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
