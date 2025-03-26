using NUnit.Framework;
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

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage();
            OnEnemyDeath();
        }
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            OnEnemyDeath();
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
