using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] private Image _shieldPowerMeter;
    [SerializeField] private Sprite[] _shieldMeterSprites;

    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;

    [SerializeField] private TMP_Text _scoreText;

    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _restartText;

    [SerializeField] private Slider _thrusterSlider;
    [SerializeField] private Image _thrusterImage;

    [SerializeField] private TMP_Text _ammoText;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    private void Start()
    {
        _gameOverText.SetActive(false);
        _restartText.SetActive(false);
    }

    public void UpdateShieldUI(int health)
    {
        if (health >= 0 || health < _shieldMeterSprites.Length)
        {
            _shieldPowerMeter.sprite = _shieldMeterSprites[health];
        }
    }

    public void UpdateLives(int health)
    { 
        if (health >=0 && health < _livesSprites.Length)
        {
            _livesImage.sprite = _livesSprites[health];
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score}";
    }

    public void UpdateThruster(float thrusterValue)
    {
        if (thrusterValue <= 0 && _thrusterSlider.gameObject.activeInHierarchy)
        {
            _thrusterSlider.gameObject.SetActive(false);
            _thrusterImage.gameObject.SetActive(false);
        }
        else if (thrusterValue > 0 && !_thrusterSlider.gameObject.activeInHierarchy)
        {
            _thrusterSlider.gameObject.SetActive(true);
            _thrusterImage.gameObject.SetActive(true);
        }

        _thrusterSlider.SetValueWithoutNotify(thrusterValue);
        _thrusterImage.fillAmount = thrusterValue/100;
    }
    
    public void UpdateAmmoCount(int amount)
    {
        _ammoText.text = $"Ammo: {amount}";
    }

    public void GameOver()
    {
        StartCoroutine(GameOverSequence());
        _restartText.SetActive(true);
    }

    IEnumerator GameOverSequence()
    {
        while (true)
        {
            yield return null;
            _gameOverText.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            _gameOverText.SetActive(false);
            yield return new WaitForSeconds(1.5f);
        }
    }

}
