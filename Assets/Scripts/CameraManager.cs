using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Vector3 _orginalPosition;
    Coroutine _coroutine;
    float _rndX;
    float _rndY;
    float _time;
    Vector3 _shakePosition = new Vector3();

    private void Start()
    {
        _orginalPosition = transform.position;
    }

    public void CameraShake(float duration, float intensity)
    {
        if (_coroutine == null) 
            _coroutine = StartCoroutine(ShakeRoutine(duration, intensity));
    }

    IEnumerator ShakeRoutine(float duration, float intensity)
    {
        _time = 0;
        while (_time < duration)
        {
            _rndX = Random.value;
            _rndY = Random.value;

            _shakePosition.x = _rndX;
            _shakePosition.y = _rndY;
            _shakePosition.z = _orginalPosition.z;

            transform.position = _shakePosition;

            _time += Time.deltaTime;
            yield return null;
        }

        transform.position = _orginalPosition;
        _coroutine = null;
    }
}
