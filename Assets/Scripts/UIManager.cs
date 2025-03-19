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

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
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
}
