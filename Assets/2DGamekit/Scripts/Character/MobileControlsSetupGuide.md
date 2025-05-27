# Mobile Controls Setup Guide

This guide will help you set up mobile touch controls for your 2D Gamekit project.

## Step 1: Create Mobile Controls UI

1. Create a new empty GameObject in your scene and name it "MobileControlsManager"
2. Add the `MobileControlsManager` component to it
3. Click on "Create Mobile Controls" in the Inspector to automatically generate the mobile UI elements
4. The setup will create:
   - A Canvas with CanvasScaler and GraphicRaycaster
   - A virtual joystick for movement
   - Action buttons for Jump, Melee Attack, Ranged Attack, Interact, and Pause

## Step 2: Configure the PlayerInput

1. Find your PlayerInput GameObject in the scene
2. In the Inspector, check "Mobile Controls Enabled" if you want to enable mobile controls by default
3. Drag the MobileControlsCanvas to the "Mobile Controls Canvas" field
4. Drag the Joystick component to the "Virtual Joystick" field
5. Drag each Mobile Button component to its respective field:
   - Jump Button
   - Melee Button
   - Ranged Button
   - Interact Button
   - Pause Button

## Step 3: Customize Controls (Optional)

1. Adjust the size and position of the joystick and buttons to fit your game's UI design
2. Modify the joystick settings:
   - Joystick Radius: How far the joystick knob can move
   - Dead Zone: Minimum input threshold before movement is registered
   - Dynamic Joystick: Whether the joystick appears where you first touch
   - Always Visible: Whether the joystick is always visible or only when in use

3. Customize the button visuals:
   - Change the button colors
   - Add custom icons or text
   - Adjust the pressed color and scale for visual feedback

## Step 4: Platform Detection

1. The system will automatically detect if your game is running on a mobile platform
2. You can force enable mobile controls for testing in the Unity Editor:
   - In the MobileControlsManager, check "Force Enable Mobile Controls"
   - You can also toggle mobile controls at runtime via script:
   ```csharp
   // Enable mobile controls
   PlayerInput.Instance.EnableMobileControls(true);
   
   // Disable mobile controls
   PlayerInput.Instance.EnableMobileControls(false);
   ```

## Troubleshooting

- If buttons are not responding, make sure the Canvas's "Raycast Target" option is enabled
- If the joystick isn't working, check that the joystick knob is properly assigned
- Make sure the Canvas sorting order is high enough to appear above your game content
- If controls don't show up on mobile, verify that "Mobile Controls Enabled" is checked

## Additional Customization

For more advanced customization, you can:
1. Edit the MobileJoystick and MobileButton scripts
2. Create custom button layouts for different screen orientations
3. Add additional buttons for game-specific actions
4. Implement adaptive UI that changes based on screen size
