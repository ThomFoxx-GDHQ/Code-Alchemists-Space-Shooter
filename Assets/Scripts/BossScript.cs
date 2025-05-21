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
        Attack
    }

    enum AttackStates
    {
        Phase1,
        Phase2,
        Phase3
    }


    GameObject _player;
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

    Coroutine _shieldCoroutine;

    [SerializeField] GameObject[] _turrets;
    [SerializeField] GameObject _turretLasersPrefab;

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
            default:
                break;
        }
    }

    private void BossEntryMovement()
    {
        if (transform.position.y <= _entryStopPositionY)
            _currentState = BossStates.None;
        transform.Translate(Vector3.down*(_speed*Time.deltaTime), Space.World);
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
        Vector3 pos = turret.transform.position;
        pos.z = 0;
        Instantiate(_turretLasersPrefab, pos, turret.transform.rotation);
    }

    private void AttackPhase1()
    {
        _turrets[0].transform.LookAt(_player.transform.position, Vector3.back);
        _turrets[0].transform.Rotate(Vector3.forward, 90, Space.World);
        FireLaser(_turrets[0]);
    }
}