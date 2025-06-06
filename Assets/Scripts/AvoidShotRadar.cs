using UnityEngine;
using System.Collections;

public class AvoidShotRadar : MonoBehaviour
{
    [SerializeField] Enemy _parentBody;
    [SerializeField] Sides _moveDirection;
    [SerializeField] float _detectionDelay = 2.5f;
    bool _canDetect = true;
    WaitForSeconds _detectionDelayWait;

    private void Start()
    {
        _detectionDelayWait = new WaitForSeconds(_detectionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile") && _canDetect)
        {
            Projectile laser = other.GetComponent<Projectile>();
            if (laser != null && !laser.IsEnemyProjectile())
            {            
                _parentBody.DodgeFire(_moveDirection);
                StartCoroutine(DetectionCoolDownRoutine());
            }
        }
    }

    IEnumerator DetectionCoolDownRoutine()
    {
        _canDetect = false;
        yield return _detectionDelayWait;
        _canDetect = true;
    }
}
