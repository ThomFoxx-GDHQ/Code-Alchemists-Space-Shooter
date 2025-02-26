using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    Vector3 _position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        _position = transform.position;

        if (transform.position.y < -5f)
        {
            _position.y = -5f;
        }
        if (transform.position.y > 5f)
        {
            _position.y = 5f;
        }

        transform.position = _position;
    }

    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * (_speed * Time.deltaTime));
    }
}
