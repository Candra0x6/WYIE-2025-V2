# Dialogue System Setup Guide

The Dialogue System allows you to create interactive conversations between the player and NPCs in your Gamekit2D project.

## 1. Create Dialogue Data Assets

1. Right-click in the Project window → Create → Gamekit2D → Dialogue Data
2. Configure your dialogue:
   - Add nodes for each part of the conversation
   - Set speaker name and optional image for each node
   - Add dialogue text
   - Add response options for branching dialogues or set a linear flow
   - Configure optional audio effects

## 2. Set Up UI Canvas

Create a Dialogue UI Canvas with the following structure:

```
DialogueCanvas (Canvas)
└── DialoguePanel (Panel)
    ├── SpeakerContainer (Panel)
    │   ├── SpeakerImage (Image)
    │   └── SpeakerNameText (TextMeshProUGUI)
    ├── DialogueText (TextMeshProUGUI)
    ├── ContinuePrompt (GameObject)
    │   └── ContinueIcon (Image)
    └── OptionsPanel (Panel)
```

3. Add the `DialogueUIController` component to the Canvas
4. Assign references in the Inspector:
   - Set DialoguePanel
   - Set SpeakerNameText
   - Set SpeakerImage and SpeakerImageContainer
   - Set DialogueText
   - Set OptionsPanel and OptionButtonPrefab 
   - Set ContinuePrompt
   - (Optional) Assign audio player for dialogue sounds

## 3. Create Option Button Prefab

1. Create a UI Button prefab with a TextMeshProUGUI child component
2. Assign this prefab to the `OptionButtonPrefab` in the DialogueUIController

## 4. Add Dialogue Triggers to NPCs

1. Add the `DialogueTrigger` component to any NPC or object you want to have conversations with
2. Configure in the Inspector:
   - Assign a Dialogue Data asset
   - Set Interaction Radius
   - Set Player Layer (typically "Player")
   - (Optional) Add a dialogue indicator sprite
   - (Optional) Add events for dialogue start/end

## 5. Testing

- Enter the interaction radius of an NPC with DialogueTrigger
- Press the interaction key (default: E) to start the conversation
- Press Space or E to advance through dialogue
- Click on response options if available
- After the conversation ends, you'll regain control of your character

## Tips

- Use a short interaction radius (around 2-3 units) for NPCs
- Make dialogue indicators clear to players
- Keep dialogue text concise and readable
- Add character sprites to make conversations more engaging
- Use branching dialogues for important story decisions
