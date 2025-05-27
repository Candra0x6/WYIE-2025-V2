# 2D Game Kit Quick Setup

This tool provides a fast and easy way to set up a new 2D Game Kit scene with all the essential elements.

## Getting Started

1. Open Unity and load your 2D Game Kit project
2. Go to the menu: **2D Game Kit > Quick Setup**
3. The Quick Setup window will appear

## Quick Setup Window

The Quick Setup window allows you to select which components you want to add to your scene:

### Setup Options

- **Player Character**: Adds the Ellen character (the player) to your scene
- **Gameplay Camera**: Adds the main camera that follows the player
- **Game UI**: Adds the in-game user interface elements (health, inventory, etc.)
- **Mobile Controls**: Adds touch controls for mobile devices
- **Dialogue System**: Adds a basic dialogue system
- **Checkpoints**: Adds a checkpoint for player respawning

### Prefab References

The tool automatically finds the necessary prefabs in your project. If any prefabs can't be found, you can manually assign them using the fields provided.

## Using Mobile Controls

If you selected the Mobile Controls option, a touch control system will be set up in your scene:

- A virtual joystick for movement
- Buttons for Jump, Melee Attack, Ranged Attack, Interact, and Pause
- Mobile controls are disabled by default and will only appear on mobile devices

For more details on mobile controls, see the **MobileControlsSetupGuide.md** file.

## After Setup

After running the setup, your scene will contain a new GameObject called "2D Game Kit" with all the selected components organized under it.

You can then:
1. Position the player character where you want them to start
2. Adjust the camera settings if needed
3. Build your level with tilemaps, platforms, enemies, etc.
4. Configure any special settings for the components you added

## Tips

- The setup tool creates a clean scene hierarchy to keep everything organized
- Mobile controls are designed to work alongside keyboard and controller inputs
- You can access more detailed guides for specific systems through the Unity menu: **2D Game Kit > Guides**

## Troubleshooting

If you encounter any issues:
- Make sure you have the complete 2D Game Kit package imported
- Check that all prefabs are correctly referenced
- Try re-running the setup tool with different options

For more help, refer to the 2D Game Kit documentation or the Unity forums.
