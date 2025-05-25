using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gamekit2D.Mission
{
    /// <summary>
    /// UI component for a single mission entry in the mission list
    /// </summary>
    public class MissionEntryUI : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI missionNameText;
        public TextMeshProUGUI progressText;
        public Image progressBar;
        public Image statusIcon;
        public Image missionIcon;
        
        [Header("Status Colors")]
        public Color activeColor = Color.yellow;
        public Color completedColor = Color.green;
        
        /// <summary>
        /// Setup the mission entry with mission data
        /// </summary>
        public void Setup(MissionData missionData, int current, int required, bool completed = false)
        {
            if (missionNameText != null)
            {
                missionNameText.text = missionData.missionName;
                missionNameText.color = completed ? completedColor : Color.white;
            }
            
            if (progressText != null)
                progressText.text = $"{current}/{required}";
                
            if (progressBar != null)
                progressBar.fillAmount = (float)current / required;
                
            if (statusIcon != null)
                statusIcon.color = completed ? completedColor : activeColor;
                
            if (missionIcon != null && missionData.itemIcon != null)
                missionIcon.sprite = missionData.itemIcon;
        }
    }
}
