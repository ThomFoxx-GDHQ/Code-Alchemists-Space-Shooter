using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private GameObject[] _powerUpPrefabs;
    [SerializeField] private Transform _powerUpContainer;

    [Header("Spawn Area")]
    [Tooltip("This is for the Positive to Negative Range of the Random X position. Where X is left and Y is Right.")]
    [SerializeField] private Vector2 _spawnXRange;
    [SerializeField] private float _topSpawnArea;
    private bool _canSpawn = true;
    [SerializeField] private GameManager _gameManager;

    [Header("Wave Information")]
    private int _waveCount = 0;
    private int _enemiesInWave = 10;
    private int _currentEnemies = 0;
    private int _spawnedEnemies = 0;
    [SerializeField] private int _finalWave = 5;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    private void Start()
    {
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(PowerupSpawnRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (_canSpawn && _waveCount < _finalWave)
        {
            //Debug.Log("Start Wave");
            _waveCount++;
            _enemiesInWave = _waveCount * 10;
            UIManager.Instance.UpdateWaveBanner(_waveCount);

            //counts for in the wave
            _currentEnemies = 0;
            _spawnedEnemies = 0;

            while (_spawnedEnemies < _enemiesInWave && _canSpawn)
            {
                float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
                int randomEnemy = Random.Range(0, _enemyPrefabs.Length);
                Instantiate(_enemyPrefabs[randomEnemy], new Vector3(randomX, _topSpawnArea, 0), Quaternion.identity, _enemyContainer);

                _spawnedEnemies++;
                _currentEnemies++;
                yield return new WaitForSeconds(2);
            }

            while (_currentEnemies > 0)
            {
                //Debug.Log($"Current Enemy COunt = {_currentEnemies}");
                yield return null;
            }

            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator PowerupSpawnRoutine()
    {
        while(_canSpawn && _waveCount < _finalWave)
        {
            float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
            int randomPowerUp = Random.Range(0, _powerUpPrefabs.Length);

            Instantiate(_powerUpPrefabs[randomPowerUp], new Vector3(randomX, _topSpawnArea, 0), Quaternion.identity, _powerUpContainer);
            yield return new WaitForSeconds(2);
        }
    }

    public void OnPlayerDeath()
    {
        _canSpawn = false;
        UIManager.Instance.GameOver();
        _gameManager.OnPlayerDeath(); ;
        Enemy[] enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.OnPlayerDeath();
        }
    }

    public void OnEnemyDeath()
    {
        _currentEnemies--;
    }
}
