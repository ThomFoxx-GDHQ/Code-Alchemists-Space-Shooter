using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct EnemyScreenBounds
{
    public float top;
    public float bottom;
    public float left; 
    public float right;
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private bool _isPlayerDead;

    private Player _player;
    [SerializeField] private EnemyScreenBounds _bounds;

    [Header("Public Access")]
    public Player Player => _player; 
    public EnemyScreenBounds Bounds => _bounds;
    PlayerInputAction _inputActions;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    private void Start()
    {
        _inputActions = new PlayerInputAction();
        _inputActions.Player.Enable();
        _inputActions.Player.Escape.performed += Escape_performed;
        _inputActions.Player.Restart.performed += Restart_performed;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Restart_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_isPlayerDead)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Escape_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene("MainMenu");
    }

    // ### Legacy Input Manager Code ### //
    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.R) && _isPlayerDead)
    //    //{
    //    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.Escape))
    //    //{
    //    //    SceneManager.LoadScene("MainMenu");
    //    //}
    //}

    public void OnPlayerDeath()
    {
        _isPlayerDead = true;
    }
}
