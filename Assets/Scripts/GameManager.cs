using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isPlayerDead;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isPlayerDead)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void OnPlayerDeath()
    {
        _isPlayerDead = true;
    }
}
