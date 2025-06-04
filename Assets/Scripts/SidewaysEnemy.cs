using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SidewaysEnemy : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    [Header("Screen Boundaries")]
    [SerializeField] EnemyScreenBounds _screenBounds;

    int _directionMultiplier = 1;

    [SerializeField] GameObject _firingPoint;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Vector3 _laserOffset;
    [SerializeField] float _fireRate = 2.5f;
    float _whenCanFire;
    Transform _laserContainer;

    GameObject _player;

    [SerializeField] int _pointValue;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] float _explosionCoverUpDelay;

    private void Start()
    {
        ResetSpawn();
        _laserContainer = GameObject.Find("LaserContainer")?.transform;
        _player = GameManager.Instance.Player.gameObject;
    }

    private void ResetSpawn()
    {
        float rndY = Random.Range(_screenBounds.bottom, _screenBounds.top);

        transform.position = new Vector3(_screenBounds.left+.05f, rndY, 0);
    }

    private void Update()
    {
        CalculateMovement();
        LookAtTarget();
        if (_whenCanFire < Time.time)
            FireLaser();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.right * (_directionMultiplier * _speed * Time.deltaTime));
        if (transform.position.x < _screenBounds.left || transform.position.x > _screenBounds.right)
        {
            //Change Direction
            _directionMultiplier *= -1;
            transform.localScale = new Vector3(_directionMultiplier, 1, 1);
            //Change vertical position
            float rngY = Random.Range(_screenBounds.bottom, _screenBounds.top);
            transform.position = new Vector3(transform.position.x, rngY, 0);
        }
    }

    private void FireLaser()
    {
        GameObject go = Instantiate(_laserPrefab, _firingPoint.transform.position + _laserOffset, _firingPoint.transform.rotation);
        go.GetComponent<Projectile>()?.AssignEnemyLaser();
        go.transform.parent = _laserContainer;
        _whenCanFire = Time.time + _fireRate;
    }

    private void LookAtTarget()
    {
        Vector3 diff = _player.transform.position - _firingPoint.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        _firingPoint.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.GetComponent<Player>()?.Damage();
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

        _player.GetComponent<Player>()?.AddScore(_pointValue);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _speed = 0;
        GetComponent<Collider>().enabled = false;

        //Always last in this method.
        Destroy(this.gameObject, _explosionCoverUpDelay);
    }
}
