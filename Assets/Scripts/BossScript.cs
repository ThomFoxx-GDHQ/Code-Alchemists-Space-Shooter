using System.Collections;
using UnityEngine;

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
    Coroutine _bossCoroutine;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] Transform[] _explosionPoints;

    WaitForSeconds _shortWait;
    [SerializeField] float _shortWaitTime = .5f;

    private void Awake()
    {
        _currentState = BossStates.Entry;
        _currentAttackPhase = AttackStates.Phase1;
    }

    private void Start()
    {
        _player = GameManager.Instance.Player.gameObject;
        _shieldScale = _shieldVisualizer.transform.localScale;
        _shieldVisualizer?.SetActive(false);
        _turretHealth = _turretDefaultHealth;
        _shieldHealth = _defaultShieldHealth;
        _shortWait = new WaitForSeconds(_shortWaitTime);
    }

    private void Update()
    {
        //Debug.Log($"CurrentPhase is :{_currentState.ToString()}");
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
                AttackPhase(_currentAttackPhase);
                break;
            case BossStates.Escape:
                EscapeRoutine();
                break;
            case BossStates.Death:
                if (_bossCoroutine == null)
                {
                    _bossCoroutine = StartCoroutine(BossDeathRoutine());
                }
                else return;
                break;
            default:
                return;

        }
    }

    private void BossEntryMovement()
    {
        if (transform.position.y <= _entryStopPositionY)
            _currentState = BossStates.None;
        transform.Translate(Vector3.down * (_speed * Time.deltaTime), Space.World);
    }

    private void EscapeRoutine()
    {
        
        if (_isEscaping && Mathf.Abs(transform.position.x) >= 42)
        {
            TurnAround();
        }
        else
        {
            transform.Translate(Vector3.right * (_direction * _speed * Time.deltaTime), Space.World);
        }

        if (_isEscaping == false && Mathf.Abs(transform.position.x) <= 1f)
        {
            _currentState = BossStates.None;
        }
    }

    private void TurnAround()
    {
        //Debug.Log($"Turn Around Called on {_model.name}: {_model.transform.rotation.eulerAngles}");
        _isEscaping = false;
        //Vector3 rotation = _model.transform.rotation.eulerAngles;
        //rotation.x += 180;
        //_model.transform.rotation = Quaternion.Euler(rotation);
        _direction *= -1;

        bool facingLeft = _direction < 0;

        _model.GetComponent<Animator>()?.SetBool("FaceLeft", facingLeft);

        transform.Translate(Vector3.right * (_direction * _speed * Time.deltaTime), Space.World);
        //Debug.Log($"Turn Around Completedon {_model.name}: {_model.transform.rotation.eulerAngles}");
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
        _isShieldActive = true;
        _currentState = BossStates.Attack;
        _turretHealth = _turretDefaultHealth;
        _shieldCoroutine = null;
    }

    IEnumerator BossDeathRoutine()
    {
        Debug.Log("Death Routine Started");
        foreach (Transform t in _explosionPoints)
        {
            Debug.Log("Explosion");
            Instantiate(_explosionPrefab,t.position, Quaternion.identity);
            yield return _shortWait;
        }
        yield return null;

        while (transform.position.x < 42)
        {
            Debug.Log("leaving");
            transform.Translate(Vector3.right * ( _speed * Time.deltaTime), Space.World);
            yield return null;
        }
        UIManager.Instance.GameOver();
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

    private void AttackPhase(AttackStates state)
    {
        _turrets[(int)state].transform.LookAt(_player.transform.position, Vector3.back);
        _turrets[(int)state].transform.Rotate(Vector3.forward, 90, Space.World);
        FireLaser(_turrets[(int)state]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile") && _turretHealth > 0)
        {
            Projectile laser = other.GetComponent<Projectile>();
            if (laser != null && laser.IsEnemyProjectile() == false)
            {
                //Debug.Log($"{other.name} hit this and IsEnemyProjectile = {laser.IsEnemyProjectile()}", other.gameObject);
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
                //Debug.Log("Shield turns off?");
                _shieldHealth = _defaultShieldHealth;
            }
            _turrets[(int)_currentAttackPhase].GetComponent<Collider>().enabled = true;
            return;
        }

        if (_turretHealth > 0)
            _turretHealth--;
        
        if (_turretHealth <= 0)
        {
            _turrets[(int)_currentAttackPhase].gameObject.SetActive(false);
            Instantiate(_explosionPrefab, _turrets[(int)_currentAttackPhase].transform.position,Quaternion.identity);

            if (_currentAttackPhase != AttackStates.Phase3)
            {
                _isEscaping = true;
                _currentAttackPhase = ChangeState(_currentAttackPhase);
                _currentState = BossStates.Escape;
            }
            else
            {
                _currentState = BossStates.Death;
            }
        }
    }

    private AttackStates ChangeState(AttackStates currentState)
    {
        switch (currentState)
        {
            case AttackStates.Phase1:
                return AttackStates.Phase2;
                
            case AttackStates.Phase2:
                return AttackStates.Phase3;
                
            case AttackStates.Phase3:
                return AttackStates.Phase1;

            default:
                return currentState;                
        }
    }
}