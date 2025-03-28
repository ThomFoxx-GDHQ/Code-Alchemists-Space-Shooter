using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private GameObject[] _powerUpPrefabs;
    [SerializeField] private Transform _powerUpContainer;

    [Header("Spawn Area")]
    [Tooltip("This is for the Positive to Negative Range of the Random X position. Where X is left and Y is Right.")]
    [SerializeField] private Vector2 _spawnXRange;
    [SerializeField] private float _topSpawnArea;
    private bool _canSpawn = true;
    [SerializeField] private GameManager _gameManager;

    private void Start()
    {
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(PowerupSpawnRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (_canSpawn)
        {
            float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
            Instantiate(_enemyPrefab, new Vector3(randomX,_topSpawnArea,0), Quaternion.identity, _enemyContainer);
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator PowerupSpawnRoutine()
    {
        while(_canSpawn)
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
}
