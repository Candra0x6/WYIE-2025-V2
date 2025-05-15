# Energy System for 2D Game Kit

This system implements an energy mechanic that limits how often players can enter levels, with regeneration over time.

## Features

- Energy limit per player (configurable)
- Energy cost per level entry (configurable)
- Automatic energy regeneration over time
- Energy regenerates even when the game is closed
- Visual feedback with UI components
- Persistent energy storage using PlayerPrefs and Unity's persistence system

## Setup Guide

### Basic Setup

1. **Add the Energy Manager**:
   - Go to "2D GameKit > Energy System > Setup" in the Unity menu
   - Run the setup wizard with your preferred settings
   - OR add the EnergyManager prefab to your starting scene

2. **Add Energy UI to Your Canvas**:
   - Add the EnergyPanel prefab to your UI canvas
   - Position it where you want the energy display to appear

3. **Add Not Enough Energy Panel**:
   - Add the NotEnoughEnergyPanel prefab to your UI canvas
   - Set it to inactive by default 
   - Assign it to the EnergyManager component

### Advanced Configuration

#### Energy Manager Settings

| Setting | Description |
|---------|-------------|
| Max Energy | Maximum energy points a player can have |
| Current Energy | Current energy points (starts at max) |
| Energy Cost Per Level | How much energy is spent to enter a level |
| Regeneration Time | Time in minutes to regenerate 1 energy point |
| Not Enough Energy Panel | Reference to the panel that appears when player lacks energy |

#### Integration with Scene Management

The system automatically integrates with the TransitionPoint component:

- TransitionPoints have a new "Requires Energy" checkbox
- When set to true, transitioning to a different zone will check energy
- Players will be prevented from entering if they have insufficient energy
- The NotEnoughEnergyPanel will be displayed

#### UI Components

1. **EnergyUI**: Controls display of current energy, maximum energy, and regeneration timer
2. **NotEnoughEnergyPanel**: Shows when a player tries to enter a level without enough energy

## Customization

### Changing Energy Icons

To use custom energy icons:
1. Create your energy icon as a UI Image prefab
2. Assign it to the "Energy Icon Prefab" field in the EnergyUI component

### Modifying Energy Costs

To set different energy costs for specific levels:
1. Find the TransitionPoint for that level
2. Create a custom script that extends TransitionPoint
3. Override the TransitionInternal method to specify a custom energy cost

## Example Usage

```csharp
// Check if player has enough energy to enter a level
if (EnergyManager.Instance.CanEnterLevel())
{
    // Enter the level
    SceneManager.LoadScene("YourLevelScene");
}
else
{
    // Show not enough energy panel
    EnergyManager.Instance.ShowNotEnoughEnergyPanel();
}

// Manually add energy points (e.g., as a reward)
EnergyManager.Instance.RegenerateEnergy(2);
```

## Troubleshooting

- **Energy not persisting**: Ensure PersistentDataManager is in your scene
- **UI not updating**: Check EnergyUI component connections to EnergyManager
- **Energy not being spent**: Verify TransitionPoint has "Requires Energy" checked

## Extending the System

The energy system can be extended for more features:

- In-app purchases to refill energy
- Energy rewards for achievements
- VIP status to reduce energy costs
- Custom energy regeneration rates
