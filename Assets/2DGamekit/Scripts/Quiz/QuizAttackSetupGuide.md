# Quiz Attack System Setup Guide

## 1. Create Quiz Data Assets

1. Right-click in the Project window → Create → Gamekit2D → Quiz Data
2. Configure your quiz in the inspector:
   - Set the quiz title
   - Add questions and answers
   - Configure damage amounts for correct and wrong answers

## 2. Set Up UI Canvas

Create a Quiz UI Canvas with the following structure:

```
QuizCanvas (Canvas)
└── QuizPanel (Panel)
    ├── TitleText (TextMeshProUGUI)
    ├── QuestionText (TextMeshProUGUI)
    └── AnswersPanel
        ├── Answer1Button (Button)
        │   └── Answer1Text (TextMeshProUGUI)
        ├── Answer2Button (Button)
        │   └── Answer2Text (TextMeshProUGUI)
        ├── Answer3Button (Button)
        │   └── Answer3Text (TextMeshProUGUI)
        └── Answer4Button (Button)
            └── Answer4Text (TextMeshProUGUI)
```

3. Add the `QuizUIController` component to the Canvas
4. Assign references in the Inspector:
   - Set Quiz Panel
   - Set Question Text
   - Set Quiz Title Text
   - Add Answer Buttons array (size 4)
   - Add Answer Texts array (size 4)
   - (Optional) Assign audio players for correct/wrong answers

## 3. Add Quiz Attack to Enemies

1. Add the `QuizAttack` component to any enemy you want to have quiz battles with
2. Configure in the Inspector:
   - Assign a Quiz Data asset
   - Set Detection Radius
   - Set Player Layer (typically "Player")
   - Set Quiz Cooldown
   - Assign enemy's Damageable component
   - (Optional) Add a quiz trigger indicator object

## 4. Testing

- Enter the detection radius of an enemy with Quiz Attack
- Press the interaction key (default: E) to start the quiz
- Answer questions correctly to damage the enemy
- Answer questions incorrectly to take damage yourself
- After answering all questions, you'll regain control of your character

## Note

This system uses the PlayerCharacter's existing DisableMovement() and EnableMovement() functions, 
which were previously set up for dialogue systems. These functions handle disabling player input
during quiz interactions and re-enabling it afterwards.

## Troubleshooting

- **Player can't move after quiz**: The system is designed to automatically restore player movement when the quiz ends, either by answering all questions or when the enemy's health reaches zero. If you're experiencing issues with player movement not being restored, check that the QuizAttack component is properly receiving the completion callback from QuizUIController.

- **Enemy not taking damage**: Check that the enemy has the Damageable component assigned in the QuizAttack inspector, and that the damage values in the QuizData asset are appropriate for the enemy's health.
