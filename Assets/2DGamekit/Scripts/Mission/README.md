# Mission System for 2D GameKit

This mission system allows you to create item collection missions in your 2D platformer. Players can receive missions from NPCs, collect items throughout the level, and return to NPCs to complete missions and receive rewards.

## Setup Instructions

### 1. Create Mission Manager

First, add the Mission Manager to your scene:

1. Right-click in the Hierarchy window
2. Select GameObject > 2D GameKit > Mission > Create Mission Manager

This creates a singleton object that persists across scenes and manages all active missions.

### 2. Create Mission UI

Add the Mission UI components to your scene:

1. Right-click in the Hierarchy window
2. Select GameObject > 2D GameKit > Mission > Create Mission UI

This adds a notification system for when missions are assigned/updated and a HUD element to show current mission progress.

### 3. Create Mission Data

Create mission definitions:

1. Right-click in the Project window
2. Select Create > Gamekit2D > Mission Data
3. Fill out the mission details in the inspector:
   - Mission Name: Name displayed to the player
   - Description: Description of the mission
   - Target Item ID: The inventory key of the item to collect (must match InventoryItem.inventoryKey)
   - Required Amount: How many items to collect
   - Health Bonus: Amount to increase player's max health when completed
   - Start/Complete Dialogue: Optional DialogueData assets for mission start/completion

4. Place mission data assets in a Resources/Missions folder to enable loading them after save/load.

### 4. Set Up NPCs

Add the MissionNPC component to any NPC that should give missions:

1. Select an NPC GameObject
2. Add Component > Gamekit2D > Mission > Mission NPC
3. Assign the Mission Data asset to the "Available Mission" field
4. Make sure the NPC has a DialogueTrigger component

### 5. Create Collectible Items

For collectible items that count toward mission progress:

1. Make sure your collectible prefabs have the InventoryItem component
2. Set the "inventoryKey" field to match the "Target Item ID" in your MissionData

## How It Works

1. Player interacts with an NPC by pressing the Interact button in range
2. NPC assigns a mission via dialogue
3. Mission progress appears in the HUD
4. Player collects items matching the mission's target ID
5. Mission progress updates in real-time
6. When all items are collected, the mission is marked complete
7. Player returns to the NPC to claim the reward (increased max health)
8. NPC dialogue changes to reflect mission completion

## Tips

- You can have multiple active missions at once
- Mission progress is saved when the game saves
- The MissionDebugger component can be used to test missions during development
- Create custom DialogueData assets for mission start/completion to enhance the narrative

## Sample Setup

A SampleMissionSetup component is provided to quickly set up a test mission on any NPC. Add this component and use the context menu option "Setup Sample Mission" to create a basic collection mission.
