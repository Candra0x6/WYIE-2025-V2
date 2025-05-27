# 2D Game Kit Mobile Controls

This package adds touch controls support to the Unity 2D Game Kit, allowing your game to be played on mobile devices like smartphones and tablets.

## Features

- Virtual joystick for character movement
- Touch buttons for jumping, attacking, and interacting
- Automatic platform detection
- Easy setup wizard
- Customizable control layout
- Support for different screen sizes and orientations
- Integration with the existing 2D Game Kit systems

## Quick Start

1. Open your 2D Game Kit project in Unity
2. Go to the top menu and select **2D Game Kit > Mobile Controls Setup**
3. Use the wizard to configure your mobile controls
4. Play your game on a mobile device or use the Unity Remote app to test

## Advanced Customization

The Mobile Controls Setup Wizard provides extensive customization options:

- **Visual appearance**: Change colors, sizes, and opacity of controls
- **Layout**: Position controls anywhere on the screen
- **Behavior**: Configure joystick responsiveness and button feedback
- **Default state**: Choose whether controls are enabled by default

You can also export your control layout as a prefab for reuse in other scenes.

## Mobile-Specific Considerations

When developing for mobile platforms:

- Test on different screen sizes and resolutions
- Consider making gameplay elements larger for touchscreen use
- Adjust difficulty to account for touch controls
- Implement auto-save features for mobile play sessions
- Consider battery usage and performance optimization

## Documentation

For more detailed information, refer to the following guides:

- **Mobile Controls Setup Guide**: Basic setup instructions
- **Mobile Controls Wizard Guide**: Advanced customization options
- **Mobile Controls API Reference**: For programmatic control

Access these guides from the Unity menu: **2D Game Kit > Guides**

## Troubleshooting

If you encounter issues with the mobile controls:

1. Make sure your scene has a PlayerInput component
2. Verify that all control references are properly assigned
3. Check that your Canvas settings are configured for proper scaling
4. Test with the Unity Remote app before building to device

## Support

For questions or issues, please visit the Unity forums or submit an issue on the GitHub repository.

Happy mobile game development!
