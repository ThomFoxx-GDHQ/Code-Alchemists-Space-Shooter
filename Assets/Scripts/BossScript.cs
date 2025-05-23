using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossScript : MonoBehaviour
{
    enum BossStates
    {
        None,
        Entry,
        Idle,
        Attack,
        Escape,
        Death
    }

    enum AttackStates
    {
        Phase1,
        Phase2,
        Phase3
    }


    GameObject _player;
    [SerializeField] GameObject _model;
    private BossStates _currentState;
    private AttackStates _currentAttackPhase;
    [SerializeField] float _speed = 2.5f;
    [SerializeField] float _entryStopPositionY;
    [SerializeField] float _maxPhaseDelayTime = 5f;
    float _timer = 0;
    [SerializeField] GameObject _shieldVisualizer;
    Vector2 _shieldScale;
    bool _isShieldActive;
    int _shieldHealth;
    [SerializeField] int _defaultShieldHealth = 3;

    Coroutine _shieldCoroutine;

    [SerializeField] GameObject[] _turrets;
    [SerializeField] GameObject _turretLasersPrefab;
    [SerializeField] float _turretFireRate = 2.5f;
    float _fireRateTimer = 0;
    int _turretHealth;
    [SerializeField] int _turretDefaultHealth = 5;
    int _direction = 1;
    bool _isEscaping = false;

    private void Awake()
    {
        _currentState = BossStates.Entry;
        _currentAttackPhase = AttackStates.Phase1;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _shieldScale = _shieldVisualizer.transform.localScale;
        _shieldVisualizer?.SetActive(false);
        _turretHealth = _turretDefaultHealth;
        _shieldHealth = _defaultShieldHealth;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case BossStates.None:
                BetweenPhases();
                break;
            case BossStates.Entry:
                BossEntryMovement();
                break;
            case BossStates.Idle:
                Idle();
                break;
            case BossStates.Attack:
                AttackPhase1();
                break;
            case BossStates.Escape:
                EscapeRoutine();
                break;
            default:
                return;
                
        }
    }

    private void BossEntryMovement()
    {
        if (transform.position.y <= _entryStopPositionY)
            _currentState = BossStates.None;
        transform.Translate(Vector3.down*(_speed*Time.deltaTime), Space.World);
    }

    private void EscapeRoutine()
    {
        _isEscaping = true;
        if (Mathf.Abs(transform.position.x) >= 42)
        {
            TurnAround();
        }
        
        if (_isEscaping = false && Mathf.Abs(transform.position.x) <= .1f)
        {
            _currentState = BossStates.None;
        }
            transform.Translate(Vector3.right * (_direction * _speed * Time.deltaTime), Space.World);
    }

    private void TurnAround()
    {
        Vector3 rotation = _model.transform.rotation.eulerAngles;
        rotation.y += 180;
        _model.transform.rotation = Quaternion.Euler(rotation);
        _direction *= -1;
        transform.Translate(Vector3.right * (_direction * _speed * Time.deltaTime), Space.World);
        _isEscaping = false;
    }

    private void BetweenPhases()
    {
        if (_timer == 0)
            _timer = Time.time + Random.Range(1, _maxPhaseDelayTime);

        if (Time.time > _timer)
        {
            _currentState = BossStates.Idle;
            _timer = 0;
        }
    }

    IEnumerator ChargeShieldRoutine()
    {
        _shieldVisualizer.SetActive(true);
        float step = 0;
        while (step < 1)
        {
            _shieldVisualizer.transform.localScale = Vector2.Lerp(Vector2.one, _shieldScale, step);
            step += Time.deltaTime;
            yield return null;
        }
        _shieldVisualizer.transform.localScale = _shieldScale;
        _shieldCoroutine = null;
        _isShieldActive = true;
        _currentState = BossStates.Attack;
    }

    private void Idle()
    {
        if (_shieldCoroutine == null)
            _shieldCoroutine = StartCoroutine(ChargeShieldRoutine());
        else return;
    }

    [ContextMenu("Fire Test Laser")]
    private void TestFire()
    {
        FireLaser(_turrets[0]);
        FireLaser(_turrets[1]);
        FireLaser(_turrets[2]);    
    }

    private void FireLaser(GameObject turret)
    {
        if (_fireRateTimer < Time.time)
        {
            Vector3 pos = turret.transform.position;
            pos.z = 0;
            Instantiate(_turretLasersPrefab, pos, turret.transform.rotation);
            _fireRateTimer = Time.time + _turretFireRate;
        }
    }

    private void AttackPhase1()
    {
        _turrets[0].transform.LookAt(_player.transform.position, Vector3.back);
        _turrets[0].transform.Rotate(Vector3.forward, 90, Space.World);
        FireLaser(_turrets[0]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Projectile laser = other.GetComponent<Projectile>();
            if (laser != null && laser.IsEnemyProjectile() == false)
            {
                Debug.Log($"{other.name} hit this and IsEnemyProjectile = {laser.IsEnemyProjectile()}", other.gameObject);
                Damage();
            }
        }
    }

    private void Damage()
    {
        if (_isShieldActive)
        {
            _shieldHealth--;
            if (_shieldHealth <= 0)
            {
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
                Debug.Log("Shield turns off?");
                _shieldHealth = _defaultShieldHealth;
            }
            _turrets[(int)_currentAttackPhase].GetComponent<Collider>().enabled = true;
            return;
        }

        _turretHealth--;
        if (_turretHealth <= 0)
        {
            if (_currentAttackPhase != AttackStates.Phase3)
            {
                _currentAttackPhase++;
                _currentState = BossStates.Escape;
            }
            else
            {
                _currentState = BossStates.Death;
            }
        }
    }
}