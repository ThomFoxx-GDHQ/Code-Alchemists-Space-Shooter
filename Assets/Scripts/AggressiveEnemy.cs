using UnityEngine;

public class AggressiveEnemy : MonoBehaviour
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

    [SerializeField] Transform _model;
    Vector3 _lookAtTarget = Vector3.down;


    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        //if (Vector3.Distance(_player.transform.position, transform.position) < 10)
            //LookAt(_player.transform);
        //else
        //   LookAt(transform.position - _lookAtTarget);

        _model.transform.LookAt(_player.transform.position, Vector3.back);
        _model.transform.Rotate(Vector3.forward, -90, Space.World);
        CalculateMovement();
    }



    private void LookAt(Transform target)
    {
        Vector3 diff = target.position - transform.position;
        diff.Normalize();
        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Vector3 rotation = _model.transform.rotation.eulerAngles;
        rotation.x = rot;
        _model.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down *(_speed*Time.deltaTime));

        if (transform.position.y < _bottomBound)
        {
            float rndX = Random.Range(_leftBound, _rightBound);
            transform.position = new Vector3(rndX, _topBound, 0);
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
