using System.Collections;
using UnityEngine;

public class CirclingEnemy : MonoBehaviour
{
    Player _player;
    [SerializeField] float _speed = 3;

    [SerializeField] float _stopPoint = 3;

    [SerializeField] int _pointValue = 10;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] float _explosionCoverUpDelay = .33f;

    bool _canRespawn = true;

    [Header("Screenplay Boundaries")]
    EnemyScreenBounds _screenBounds;

    [Header("Circular Motion Stuff")]
    bool _startCircle = false;
    [SerializeField] float _angluarSpeed = 1f;
    [SerializeField] float _radius = 1f;
    Vector2 _offset = Vector2.zero;

    Vector2 _centerPoint = Vector2.zero;
    float _currentAngle = 0;

    [SerializeField] float _maxTimer = 5f;
    Coroutine _circlingRoutine;

    Vector3 _newPOS = Vector3.zero;
    float _rndX = 0;


    private void Start()
    {
        _player = GameManager.Instance.Player;
        _screenBounds = GameManager.Instance.Bounds;
    }

    private void Update()
    {
        CalculateMovement();
        BoundRespawn();
    }

    void CalculateMovement()
    {
        if (_startCircle == false)
        {
            transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        }

        if (_startCircle == false && transform.position.y <= _stopPoint && _circlingRoutine == null)
        {
            _startCircle = true;
            _centerPoint = transform.position;
            _centerPoint.y -= _radius;
            _circlingRoutine = StartCoroutine(CircleTimerRoutine());
        }

        if (_startCircle)
        {
            _currentAngle += _angluarSpeed + Time.deltaTime;
            //Vector2 offset = new Vector2(Mathf.Sin(_currentAngle), Mathf.Cos(_currentAngle)) * _radius;
            _offset.x = Mathf.Sin(_currentAngle);
            _offset.y = Mathf.Cos(_currentAngle);
            _offset *= _radius;
            transform.position = _centerPoint + _offset;
        }
    }

    IEnumerator CircleTimerRoutine()
    {
        float timer = Random.Range(1, _maxTimer);
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
        _startCircle = false;
    }

    void BoundRespawn()
    {
        if (transform.position.y <= _screenBounds.bottom && _canRespawn != false)
        {
            _rndX = Random.Range(_screenBounds.left, _screenBounds.right);
            _newPOS.x = _rndX;
            _newPOS.y = _screenBounds.top;
            _newPOS.z = 0;
            transform.position = _newPOS;
            _circlingRoutine = null;
            _currentAngle = 0;
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
        GetComponent<Collider>().enabled = false;

        //Always last in this method.
        Destroy(this.gameObject, _explosionCoverUpDelay);
    }

    public void OnPlayerDeath()
    {
        _canRespawn = false;
    }
}
