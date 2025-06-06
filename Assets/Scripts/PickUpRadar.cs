using System.Collections;
using UnityEngine;

public class PickUpRadar : MonoBehaviour
{
    [SerializeField] Enemy _parentBody;
    [SerializeField] float _detectionDelay = 2.5f;
    bool _canDetect = true;
    WaitForSeconds _detectionDelayWait;

    private void Start()
    {
        _detectionDelayWait = new WaitForSeconds(_detectionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PowerUp>(out PowerUp powerUp) && _canDetect == true)
        {
            //Tell Parent to fire.
            //Debug.Log("Found PowerUP");
            _parentBody.FireAtPowerUp();
            StartCoroutine(DetectionCoolDownRoutine());
        }
    }

    IEnumerator DetectionCoolDownRoutine()
    {
        _canDetect = false;
        yield return _detectionDelayWait;
        _canDetect = true;
    }
}
