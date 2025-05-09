using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
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

    [SerializeField] GameObject _enemyLaserPrefab;
    [SerializeField] GameObject _shieldVisual;
    bool _isShieldActive = false;

    Sides _dodgeDirection;
    bool _isDodging;
    [SerializeField] float _dodgeTime;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        StartCoroutine(FireLaserRoutine());

        //Shield Activation
        float rndShield = Random.value;
        if (rndShield > 0.5f)
        {
            _isShieldActive = true;
            _shieldVisual.SetActive(true);
        }
        else
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
        }
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        if (_isDodging)
            transform.Translate(Vector3.right * ((int)_dodgeDirection * _speed * Time.deltaTime));        
        else
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
            Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            float randomRefireTime = Random.Range(2, 5);
            yield return new WaitForSeconds(randomRefireTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage();
            if (!_isShieldActive)
                OnEnemyDeath();
            else
            {
                _isShieldActive = false;
                _shieldVisual.SetActive(false);
            }
        }
        if (other.CompareTag("Projectile"))
        {
            if (!other.GetComponent<Projectile>().IsEnemyProjectile)
            {
                Destroy(other.gameObject);
                if (!_isShieldActive)
                    OnEnemyDeath();
                else
                {
                    _isShieldActive = false;
                    _shieldVisual.SetActive(false);
                }
            }
        }
    }

    private void OnEnemyDeath()
    {
        SpawnManager.Instance.OnEnemyDeath();

        _player.AddScore(_pointValue);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _speed = 0;
        GetComponent<Collider>().enabled = false;

        //Always last in this method.
        Destroy(this.gameObject, _explosionCoverUpDelay);
    }

    public void OnPlayerDeath()
    {
        _canRespawn = false;
    }

    public void FireAtPowerUp()
    {
        Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
    }

    public void DodgeFire(Sides dodgeDirection)
    {
        if (_isDodging == false)
        {        
            _dodgeDirection = dodgeDirection;
            _isDodging = true;
            StartCoroutine(DodgeTimeRoutine());
        }
    }

    IEnumerator DodgeTimeRoutine()
    {
        yield return new WaitForSeconds(_dodgeTime);
        _isDodging = false;
    }
}
