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
    [SerializeField] private WaveEnemySpawns[] _enemySpawns;
    private float[] _enemySpawnPercentage;
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private GameObject[] _powerUpPrefabs;
    [SerializeField] private int[] _powerUpWeights;
    private float[] _powerUpSpawnPercentages;
    [SerializeField] private Transform _powerUpContainer;

    //gatcha mockup code
    private GameObject[] _commonPowerUps, _uncommonPowerUps, _rarePowerUps;
    private float[] _catorgoryPercantages;


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

        PowerUpPercentCalaculation();
    }

    private void PowerUpPercentCalaculation()
    {
        //PowerUp Percentage Calaculation
        int totals = 0;
        foreach (int i in _powerUpWeights)
            totals += i;

        _powerUpSpawnPercentages = new float[_powerUpWeights.Length];

        for (int j = 0; j < _powerUpWeights.Length; j++)
            _powerUpSpawnPercentages[j] = (float)_powerUpWeights[j] / (float)totals;
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (_canSpawn && _waveCount < _finalWave)
        {
            //Debug.Log("Start Wave");
            _waveCount++;
            _enemiesInWave = _waveCount * 10;
            UIManager.Instance.UpdateWaveBanner(_waveCount);

            _enemySpawnPercentage = _enemySpawns[_waveCount - 1].SpawnRates();

            yield return new WaitForSeconds(5f);

            //counts for in the wave
            _currentEnemies = 0;
            _spawnedEnemies = 0;

            while (_spawnedEnemies < _enemiesInWave && _canSpawn)
            {
                float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
                int randomEnemy = EnemyPicker();
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
        }
    }

    IEnumerator PowerupSpawnRoutine()
    {
        while (_canSpawn && _waveCount < _finalWave)
        {
            yield return new WaitForSeconds(5f);
            while (_currentEnemies > 0)
            {

                float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
                int randomPowerUp = PowerupPicker();

                Instantiate(_powerUpPrefabs[randomPowerUp], new Vector3(randomX, _topSpawnArea, 0), Quaternion.identity, _powerUpContainer);
                yield return new WaitForSeconds(2);
            }
            yield return null;
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

    private int PowerupPicker()
    {
        int item = 0;

        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i<_powerUpSpawnPercentages.Length;i++)
        {
            runningTotal += _powerUpSpawnPercentages[i];
            if (RNG <= runningTotal)
                return i;
        }

       return item;
    }

    private int EnemyPicker()
    {
        int enemy = 0;

        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i<_enemySpawnPercentage.Length;i++)
        {
            runningTotal+= _enemySpawnPercentage[i];
            if (RNG <= runningTotal)
                return i;
        }

        return enemy;
    }

    //Gathca MockUp
    private GameObject PowerUpCatorgyPick()
    {
        GameObject powerUp = _commonPowerUps[0];

        int catID = 0;

        float RNG = Random.value;
        float runningTotal = 0;

        for (int i=0;i<_catorgoryPercantages.Length;i++)
        {
            runningTotal += _catorgoryPercantages[i];
            if (RNG <= runningTotal)
                catID = i;
        }

        int randompowerup;
        
        switch (catID)
        {
            case 0:
                randompowerup = Random.Range(0, _commonPowerUps.Length);
                powerUp = _commonPowerUps[randompowerup];
                break;

            case 1:
                randompowerup = Random.Range(0, _uncommonPowerUps.Length);
                powerUp = _uncommonPowerUps[randompowerup];
                break;
            case 2:
                randompowerup = Random.Range(0, _rarePowerUps.Length);
                powerUp = _rarePowerUps[randompowerup];
                break;                
            default:
                break;
        }

        return powerUp;
    }
}
