using UnityEngine;

public class HomingMissile : MonoBehaviour, IProjectile
{
    private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _targetDelay;
    private bool _isEnemyMissile = false;
    private Transform _defaultTarget;

    public void AssignTarget(Transform target)
    {
        _target = target;
    }

    public void AssignEnemyMissile()
    {
        _isEnemyMissile = true;
    }

    public bool IsEnemyProjectile()
    {
        return _isEnemyMissile;
    }

    private void Awake()
    {
        _defaultTarget = GameObject.Find("DefaultMissileTarget").transform;
        _target = _defaultTarget;
        _targetDelay = _targetDelay + Time.time;
    }

    private void Update()
    {
        if (_target == null)
            _target = _defaultTarget;

        transform.Translate(Vector3.forward * (_speed * Time.deltaTime), Space.Self);

        if (Time.time > _targetDelay)
        {
            transform.LookAt(_target.position);
        }
    }
}
