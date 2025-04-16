using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    [SerializeField] float _leftBound;
    [SerializeField] float _rightBound;
    [SerializeField] float _topBound;
    [SerializeField] float _bottomBound;

    [Header("Circular Motion Stuff")]
    bool _startCircle = false;
    [SerializeField] float _angluarSpeed = 1f;
    [SerializeField] float _radius = 1f;

    Vector2 _centerPoint = new Vector2();
    float _currentAngle = 0;

    [SerializeField] float _maxTimer = 5f;
    Coroutine _circlingRoutine;


    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
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
            _currentAngle += _angluarSpeed /*+ Time.deltaTime*/;
            Vector2 offset = new Vector2(Mathf.Sin(_currentAngle), Mathf.Cos(_currentAngle)) * _radius;
            transform.position = _centerPoint + offset;
        }
    }

    IEnumerator CircleTimerRoutine()
    {
        float timer = Random.Range(1, _maxTimer);
        while (timer >0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
        _startCircle = false;
    }

    void BoundRespawn()
    {
        if (transform.position.y <= _bottomBound)
        {
            float rndX = Random.Range(_leftBound, _rightBound);
            transform.position = new Vector3(rndX, _topBound, 0);
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
            if (!other.GetComponent<Projectile>().IsEnemyProjectile)
            {
                Destroy(other.gameObject);
                OnEnemyDeath();
            }
        }
    }

    private void OnEnemyDeath()
    {
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
