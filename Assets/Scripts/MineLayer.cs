using System.Collections;
using UnityEngine;

public class MineLayer : MonoBehaviour
{
    [SerializeField] float _speed = 5;
    [Header("Screenplay Boundaries")]
    [SerializeField] float _leftBound;
    [SerializeField] float _rightBound;
    [SerializeField] float _topBound;
    [SerializeField] float _bottomBound;

    private bool _canRespawn = true;
    private Player _player;
    [SerializeField] int _pointValue = 10;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] float _explosionCoverUpDelay = .33f;

    [SerializeField] GameObject _minePrefab;
    [SerializeField] float _mineDelayTime;
    float _canLayMine = 0;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        StartCoroutine(FireLaserRoutine());

        //Shield Activation
        float rndShield = Random.value;

    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y < _bottomBound && _canRespawn)
        {
            Respawn();
        }
        else if (transform.position.y < _bottomBound && _canRespawn == false)
        {
            Destroy(this.gameObject);
        }
    }

    private void Respawn()
    {
        float rng = Random.Range(_leftBound, _rightBound);
        transform.position = new Vector3(rng, _topBound, 0);
    }

    IEnumerator FireLaserRoutine()
    {
        while (_player != null)
        {
            if (transform.position.y <= _player.transform.position.y)
            {
                if (_canLayMine < Time.time)
                {
                    Instantiate(_minePrefab,transform.position, Quaternion.identity);
                    _canLayMine = Time.time + _mineDelayTime;
                }
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage();

            OnEnemyDeath();

        }
        if (other.CompareTag("Projectile"))
        {
            if (!other.GetComponent<IProjectile>().IsEnemyProjectile())
            {
                Destroy(other.gameObject);

                OnEnemyDeath();

            }
        }
    }

    private void OnEnemyDeath()
    {
        SpawnManager.Instance.OnEnemyDeath();

        _player.AddScore(_pointValue);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _speed = 0;
        var colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        //Always last in this method.
        Destroy(this.gameObject, _explosionCoverUpDelay);
    }

    public void OnPlayerDeath()
    {
        _canRespawn = false;
    }
}
