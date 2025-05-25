using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnStartClicked()
    {
        Debug.Log("Start clicked!");
        SceneManager.LoadScene("World 1"); // Make sure this matches your scene name
    }
}
