using UnityEngine;

public class AggressiveEnemy : MonoBehaviour
{
    [SerializeField] float _speed = 5;
    [Header("Screenplay Boundaries")]
    EnemyScreenBounds _screenBounds;

    private bool _canRespawn = true;
    private Player _player;
    [SerializeField] int _pointValue = 10;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] float _explosionCoverUpDelay = .33f;
    [SerializeField] float _explosionScale = 1;

    [SerializeField] Transform _model;
    Vector3 _lookAtTarget = Vector3.down;

    float _rndX = 0;
    Vector2 _newPOS = Vector2.zero;

    private void Start()
    {
        _player = GameManager.Instance.Player;
        _screenBounds = GameManager.Instance.Bounds;
    }

    private void Update()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) < 10)
        {
            _model.transform.LookAt(_player.transform.position, Vector3.back);
            _model.transform.Rotate(Vector3.forward, -90, Space.World);
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, (_speed * 2 * Time.deltaTime));
        }
        else
        {
            _model.transform.LookAt(transform.position + _lookAtTarget, Vector3.back);
            _model.transform.Rotate(Vector3.forward, -90, Space.World);
            CalculateMovement();
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < _screenBounds.bottom && _canRespawn != false)
        {
            _rndX = Random.Range(_screenBounds.left, _screenBounds.right);
            _newPOS.x = _rndX;
            _newPOS.y = _screenBounds.top;
            transform.position = _newPOS;
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
        GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * _explosionScale;
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
