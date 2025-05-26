# Scene Transition Tools for 2D Game Kit

This document provides an overview of the scene transition tools available in the 2D Game Kit.

## Available Tools

The 2D Game Kit provides several tools to help you set up scene transitions:

1. **Enhanced Scene Transition Wizard** - A step-by-step guided wizard for creating transitions (recommended)
2. **Scene Transition Setup Wizard** - A simpler wizard for quick transition setup
3. **Advanced Scene Setup Wizard** - A multi-purpose wizard for setting up complete scenes
4. **SceneSetupHelper** - A utility class with helper methods for scene setup

## When to Use Each Tool

- **Enhanced Scene Transition Wizard**: Use this when you want a guided, user-friendly experience for creating transitions between scenes. This is the recommended option for most users.

- **Scene Transition Setup Wizard**: A more straightforward tool that creates transitions with fewer options. Use this for quick setups.

- **Advanced Scene Setup Wizard**: When you need to configure multiple aspects of a scene, including transitions, checkpoints, and boundaries. This is a more comprehensive tool.

- **SceneSetupHelper**: For programmers who want to create their own setup logic or script scene transitions.

## Transition Types

You can create three types of transitions:

1. **Door** - A traditional door transition with a door sprite
2. **Portal** - A circular portal effect for teleportation
3. **Teleporter** - A teleporter pad with a sci-fi feel

## Requirements for Scene Transitions

For scene transitions to work properly:

1. Both scenes must be added to the build settings
2. Each scene should have a SceneController
3. The player object should be properly tagged and have necessary components
4. Transition tags must match between source and destination points

## Transition Tags (A-G)

Transition tags are used to match entry and exit points between scenes. When a player enters a transition with tag "B" in one scene, they will appear at the destination with tag "B" in the target scene.

## Common Setup Issues

If your transitions aren't working:

1. **Missing SceneController**: Each scene needs a SceneController
2. **Scene not in build settings**: Add both scenes to build settings
3. **Mismatched tags**: Ensure source and destination have the same tag
4. **Player not set**: The transitioning game object must be properly set

## Additional Resources

For more details, see:
- `SceneTransitionSetupGuide.md` - Detailed guide on setting up transitions
- Scene Management scripts in the 2D Game Kit documentation
