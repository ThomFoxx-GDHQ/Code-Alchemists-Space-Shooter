using System.Collections;
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
    private int _score;

    // Boundaries to restrict player movement
    [Header("ScreenPlay Boundaries")]
    [SerializeField] private float _topBounds;
    [SerializeField] private float _bottomBounds;
    [SerializeField] private float _leftBounds;
    [SerializeField] private float _rightBounds;

    // Laser shooting settings
    [Header("Laser Settings")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Vector3 _laserOffset;
    [SerializeField] private float _fireRate = 2.5f;
    private float _whenCanFire = -1; // Tracks when the player can shoot again
    [SerializeField] private Transform _laserContainer;
    [SerializeField] private AudioClip _laserAudioClip;

    [Header("Quad Shot Settings")]
    [SerializeField] GameObject _quadshotPreab;
    bool _isQuadActive;

    private SpawnManager _spawnManager;

    private Coroutine _QuadPowerTimerRoutine;
    private Coroutine _SpeedBoostTimerRoutine;

    private int _health = 3;
    [SerializeField] private Transform _shieldVisuals;
    [SerializeField] private bool _isShieldActive;
    private int _currentShieldHealth = 0;
    [SerializeField] private int _maxShieldHealth = 3;
    [SerializeField] private ShieldVisualization _shieldVisualization;
    [SerializeField] private GameObject[] _damageVisualization;

    [SerializeField] private float _speedBoostMultiplier = 2;
    private float _boostedMultiper = 1;

    [SerializeField] private float _thrustMultiplier = 2;
    private float _thrustingMultiplier = 1;

    private bool _isThrustActive = false;
    private float _engineHeat = 0;
    [SerializeField] private float _heatingRate = 2;
    [SerializeField] private float _cooldoawnRate = 1.5f;
    [SerializeField] private float _maxEngineHeat = 100;
    private bool _isEngineOverheated = false;

    void Start()
    {
        _spawnManager = GameObject.FindAnyObjectByType<SpawnManager>();

        if (_spawnManager == null)
            Debug.LogError("SpawnManager is Null!!!", this);

        ShieldActive(_isShieldActive, _currentShieldHealth);
        UIManager.Instance.UpdateScore(_score);
        UIManager.Instance.UpdateThruster(_engineHeat);
    }

    void Update()
    {
        ThrusterCalculations(); //Process whether Thruster is being used this frame

        CalculateMovement(); // Process movement input and apply movement

        // Check if space key is pressed and shooting cooldown has elapsed
        if (Input.GetKey(KeyCode.Space) && _whenCanFire < Time.time)
        {
            FireLaser();
        }
    }

    private void ThrusterCalculations()
    {
        if (_engineHeat <= _maxEngineHeat && !_isEngineOverheated)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _thrustingMultiplier = _thrustMultiplier;
                _isThrustActive = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _thrustingMultiplier = 1;
                _isThrustActive = false;
            }
        }
        else if (_engineHeat > _maxEngineHeat)
        {
            _engineHeat = _maxEngineHeat;
            _thrustingMultiplier = 1;
            _isThrustActive = false;
            _isEngineOverheated = true;
        }

        if (_engineHeat < 0)
        {
            _engineHeat = 0;
            _isEngineOverheated = false;
        }

        //_engineHeat = Mathf.Clamp(_engineHeat, 0, 100);

        if (_isThrustActive && _engineHeat < _maxEngineHeat)
            _engineHeat += _heatingRate * Time.deltaTime;

        if (!_isThrustActive && _engineHeat > 0)
            _engineHeat -= _cooldoawnRate * Time.deltaTime;

        UIManager.Instance.UpdateThruster(_engineHeat);
    }

    private void CalculateMovement()
    {
        // Get movement input from player
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _direction = new Vector3(_horizontalInput, _verticalInput, 0);

        // Move player based on input and speed
        transform.Translate(_direction * (_speed * _boostedMultiper * _thrustingMultiplier * Time.deltaTime));

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
        if (_isQuadActive == true)
        {
            //Instantiate a Quad Shot Projectile.
            Instantiate(_quadshotPreab, transform.position, Quaternion.identity, _laserContainer);
        }
        else
        {
            // Instantiate a laser projectile at the player's position
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity, _laserContainer);
        }
        AudioManager.Instance.PlaySoundAtPlayer(_laserAudioClip);
        // Set cooldown time to delay the next shot
        _whenCanFire = Time.time + _fireRate;
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _currentShieldHealth--;
            _shieldVisualization.ApplyHealth(_currentShieldHealth);
            if (_currentShieldHealth <= 0)
            {
                _isShieldActive = false;
                ShieldActive(false, 0);
            }
            return;
        }

        _health--;
        UIManager.Instance.UpdateLives(_health);

        if (_health == 2)
        {
            int rng = Random.Range(0, _damageVisualization.Length);
            _damageVisualization[rng].SetActive(true);
        }

        if (_health == 1)
        {
            if (_damageVisualization[0].activeInHierarchy == false)
                _damageVisualization[0].SetActive(true);
            else
                _damageVisualization[1].SetActive(true);
        }

        if (_health <=0)
        {
            _spawnManager.OnPlayerDeath();            
            //Destroy(this.gameObject);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            _damageVisualization[0].SetActive(false);
            _damageVisualization[1].SetActive(false);
            this.enabled = false;
        }
    }

    public void ActivateQuadShot()
    {
        _isQuadActive = true;

        if (_QuadPowerTimerRoutine == null)
            _QuadPowerTimerRoutine = StartCoroutine(QuadShotPowerDownRoutine());
        else
        {
            StopCoroutine(_QuadPowerTimerRoutine);
            _QuadPowerTimerRoutine = StartCoroutine(QuadShotPowerDownRoutine());
        }
    }

    IEnumerator QuadShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isQuadActive = false;
        _QuadPowerTimerRoutine = null;
    }

    public void ShieldActive(bool isActive, int health)
    {
        _shieldVisuals.gameObject.SetActive(isActive);
        _isShieldActive = isActive;

        _currentShieldHealth = health;

        if (_currentShieldHealth > _maxShieldHealth)
            _currentShieldHealth = _maxShieldHealth;

        _shieldVisualization.ApplyHealth(_currentShieldHealth);
    }

    public void ActivateSpeedBoost()
    {
        _boostedMultiper = _speedBoostMultiplier;

        if (_SpeedBoostTimerRoutine == null)
            _SpeedBoostTimerRoutine = StartCoroutine(SpeedBoostCountdownRoutine());
        else
        {
            StopCoroutine(_SpeedBoostTimerRoutine);
            _SpeedBoostTimerRoutine = StartCoroutine(SpeedBoostCountdownRoutine());
        }
    }

    IEnumerator SpeedBoostCountdownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _boostedMultiper = 1;
        _SpeedBoostTimerRoutine = null;
    }

    public void AddScore(int points)
    {
        _score += points;
        UIManager.Instance.UpdateScore(_score);
    }

    public void RestoreHealth()
    {
        if (_damageVisualization[0].activeInHierarchy == true)
            _damageVisualization[0].SetActive(false);
        else
            _damageVisualization[1].SetActive(false);
    }
}
