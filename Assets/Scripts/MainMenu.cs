using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Scene _firstLevelScene;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
