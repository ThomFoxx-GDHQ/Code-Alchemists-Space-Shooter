using UnityEngine;

public class Player : MonoBehaviour
{
    // Player movement settings
    [Header("Player Stats")]
    [SerializeField] private float _speed = 5f;
    private Vector3 _position;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _direction;

    // Boundaries to restrict player movement
    [Header("ScreenPlay Boundaries")]
    [SerializeField] private float _topBounds;
    [SerializeField] private float _bottomBounds;
    [SerializeField] private float _leftBounds;
    [SerializeField] private float _rightBounds;

    // Laser shooting settings
    [Header("Laser Settings")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _fireRate = 2.5f;
    private float _whenCanFire = -1; // Tracks when the player can shoot again
    [SerializeField] private Transform _laserContainer;

    void Start()
    {
        // Initialization (currently unused)
    }

    void Update()
    {
        CalculateMovement(); // Process movement input and apply movement

        // Check if space key is pressed and shooting cooldown has elapsed
        if (Input.GetKey(KeyCode.Space) && _whenCanFire < Time.time)
        {
            FireLaser();
        }
    }

    private void CalculateMovement()
    {
        // Get movement input from player
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _direction = new Vector3(_horizontalInput, _verticalInput, 0);

        // Move player based on input and speed
        transform.Translate(_direction * (_speed * Time.deltaTime));

        // Check movement boundaries only if the player is moving
        if (_direction != Vector3.zero)
            BoundsCheck();
    }

    private void BoundsCheck()
    {
        _position = transform.position;

        // Restrict vertical movement within top and bottom bounds
        if (_position.y < _bottomBounds) _position.y = _bottomBounds;
        if (_position.y > _topBounds) _position.y = _topBounds;

        // Implement horizontal wrapping (moving past one side moves to the other)
        if (_position.x < _leftBounds) _position.x = _rightBounds;
        if (_position.x > _rightBounds) _position.x = _leftBounds;

        transform.position = _position; // Apply adjusted position
    }

    private void FireLaser()
    {
        // Instantiate a laser projectile at the player's position
        Instantiate(_laserPrefab, transform.position, Quaternion.identity, _laserContainer);

        // Set cooldown time to delay the next shot
        _whenCanFire = Time.time + _fireRate;
    }
}
