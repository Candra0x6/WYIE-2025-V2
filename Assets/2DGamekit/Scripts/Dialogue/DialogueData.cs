using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D.Dialogue
{
    [Serializable]
    public class DialogueOption
    {
        public string text;
        public int nextNodeIndex; // -1 means end dialogue
    }

    [Serializable]
    public class DialogueNode
    {
        public string speakerName;
        public Sprite speakerImage;
        [TextArea(3, 10)]
        public string text;
        public List<DialogueOption> options = new List<DialogueOption>();
        
        // If no options are provided, this is the index of the next node
        // -1 means end dialogue
        public int nextNodeIndex = -1;
    }

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Gamekit2D/Dialogue Data", order = 1)]
    public class DialogueData : ScriptableObject
    {
        public List<DialogueNode> nodes = new List<DialogueNode>();
        
        // Optional audio to play when dialogue starts
        public AudioClip startDialogueSound;
        
        // Optional audio for dialogue advancement
        public AudioClip advanceDialogueSound;
        
        // Get the first node in the dialogue
        public DialogueNode GetStartNode()
        {
            if (nodes.Count > 0)
                return nodes[0];
            return null;
        }
        
        // Get a specific node by index
        public DialogueNode GetNode(int index)
        {
            if (index >= 0 && index < nodes.Count)
                return nodes[index];
            return null;
        }
    }
}
