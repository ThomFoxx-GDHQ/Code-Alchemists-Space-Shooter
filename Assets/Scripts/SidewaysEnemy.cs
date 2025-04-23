using UnityEngine;

public class SidewaysEnemy : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    [Header("Screen Boundaries")]
    [SerializeField] float _leftBounds;
    [SerializeField] float _rightBounds;

    int _directionMultiplier = 1;


    private void Update()
    {
        transform.Translate(Vector3.right * (_directionMultiplier * _speed * Time.deltaTime));
        if (transform.position.x < _leftBounds || transform.position.x > _rightBounds)
        {
            _directionMultiplier *= -1;
            transform.localScale = new Vector3(_directionMultiplier, 1, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider was hit.");
    }
}
