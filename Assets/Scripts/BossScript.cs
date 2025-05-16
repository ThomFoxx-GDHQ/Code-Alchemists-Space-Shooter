using UnityEngine;

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

    private BossStates _currentState;
    [SerializeField] float _speed = 2.5f;
    [SerializeField] float _entryStopPositionY;

    private void Awake()
    {
        _currentState = BossStates.Entry;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case BossStates.None:
                break;
            case BossStates.Entry:
                BossEntryMovement();
                break;
            case BossStates.Idle:
                break;
            case BossStates.Attack:
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
}