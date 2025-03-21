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
        if (health >=0 || health < _livesSprites.Length)
        {
            _livesImage.sprite = _livesSprites[health];
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = $"Score: {score}";
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
