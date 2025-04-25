using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    [SerializeField] bool _isEnemyProjectile;
    [SerializeField] bool _isStarburst;
    [SerializeField] float _timeDelay = 1f;
    [SerializeField] GameObject _burstExplosion;
    [SerializeField] Transform _laserContainer;
    float _timer = 0;

    private void Start()
    {
        _laserContainer = transform.parent;
        //Debug.Break();
    }

    public bool IsEnemyProjectile
    {
        get { return _isEnemyProjectile; }
    }

    private void Update()
    {
        transform.Translate(transform.up * (_speed * Time.deltaTime), Space.World);

        _timer += Time.deltaTime;

        if (_timer >= _timeDelay && _isStarburst)
        {
            if (_burstExplosion != null)
            {
                Instantiate(_burstExplosion, transform.position, Quaternion.identity, _laserContainer);
            }

            if (!transform.parent.CompareTag("Container"))
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }

        if (Mathf.Abs(transform.position.y) > 10)
        {
            if (!transform.parent.CompareTag("Container"))
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isEnemyProjectile == true)
        {
            other.GetComponent<Player>()?.Damage();
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyProjectile = true;
    }
}
