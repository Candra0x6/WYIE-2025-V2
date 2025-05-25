# Multiple Mission System Setup Guide

This guide explains how to set up the Multiple Mission System UI in your Unity project.

## Automatic Setup (Recommended)

The easiest way to set up the Multiple Mission System UI is to use the automatic setup wizard:

1. In Unity, go to the menu: `2D Game Kit > Setup > Mission UI Setup Wizard`
2. In the wizard window, you can:
   - Choose to use an existing Canvas or create a new one
   - Select which UI components to create (Mission List, Mission Notifications, etc.)
   - Adjust the position and size of the UI elements
3. Click "Create Mission UI" to automatically generate all the necessary UI components
4. The wizard will create:
   - A mission entry prefab
   - A mission list panel with scroll view
   - A mission notification panel
   - And set up all the required references

## Manual Setup

1. Create a new UI Canvas if you don't already have one
2. In your Canvas, create a new Panel for the mission list
3. Add a scroll view inside the panel to allow scrolling through multiple missions
4. Create a mission entry prefab with the following components:
   - Text/TextMeshPro for the mission name
   - Text/TextMeshPro for the progress (e.g., "2/5")
   - Image for the progress bar
   - Image for the mission icon (optional)
   - Image for the status icon (optional)
   - Add the `MissionEntryUI` component to this prefab

## Setting Up the MissionListUI Component

1. Add the `MissionListUI` script to your Canvas or UI manager object
2. Assign the mission list panel to the `missionListPanel` field
3. Create and assign the mission entry prefab to the `missionEntryPrefab` field
4. Create a content container inside your scroll view and assign it to the `missionEntryContainer` field
5. (Optional) Create a "No Missions" text object and assign it to the `noMissionsText` field
6. Configure the behavior settings:
   - `toggleKey`: The key to press to toggle the mission list (default is J)
   - `keepOpenAfterToggle`: If true, mission list stays open until manually closed
   - `showNotificationOnNewMission`: Whether to briefly show the mission list when a new mission is assigned

## Setting Up Mission NPCs

1. Add the `MissionNPC` component to any NPC that should give missions
2. Add multiple missions to the `availableMissions` array
3. Set `assignMissionsSequentially` to true if you want missions to be given one at a time
4. Set `assignMissionsSequentially` to false if you want all missions to be given at once

## Testing the System

1. Use the `MissionDebugger` component to test assigning and completing missions
2. You can add the `MissionDebugger` to any game object in your scene
3. Add test missions to the `testMissions` array
4. Use the context menu options to assign and complete missions

## Customizing the UI

- Modify the `MissionEntryUI` script to add more UI elements or change the appearance
- Customize the colors and styles in the Unity Editor
- Add animations or sound effects when missions are assigned or completed
