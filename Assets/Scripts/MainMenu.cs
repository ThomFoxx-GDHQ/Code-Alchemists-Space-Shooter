using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    public SceneAsset _firstLevelScene;

    public void StartGame()
    {
        SceneManager.LoadScene(_firstLevelScene.name);
    }
}
