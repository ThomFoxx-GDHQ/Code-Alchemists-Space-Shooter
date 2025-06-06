using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _bottomBounds = -12f;
    [SerializeField] private PowerUpType _powerUpID;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private int _powerupAmount = 5;

    Player _player;

    private void Start()
    {
        _player = GameManager.Instance.Player;
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        if (Input.GetKey(KeyCode.C))
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, (_speed * 2 * Time.deltaTime));
        else
            transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        
        if (transform.position.y <= _bottomBounds)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                        

            switch (_powerUpID)
            {
                case PowerUpType.None:
                    Debug.Log("No PowerUp to to this Item!");
                    break;
                case PowerUpType.QuadShot:
                    _player.ActivateQuadShot();
                    break;
                case PowerUpType.SpeedBoost:
                    _player.ActivateSpeedBoost();
                    break;
                case PowerUpType.Shield:
                    _player.ShieldActive(true, 99);
                    break;
                case PowerUpType.Ammo:
                    _player.AddAmmo(_powerupAmount);
                    break;
                case PowerUpType.Repair:
                    _player.AddHealth();
                    break;
                case PowerUpType.GatlingGun:
                    _player.ActivateGatlingGun();
                    break;
                case PowerUpType.Starburster:
                    _player.ActivateStarBurster();
                    break;
                case PowerUpType.ControlJam:
                    _player.ActivateJammedControls();
                    break;
                case PowerUpType.HomingMissile:
                    _player.AddMissiles();
                    break;
                default:
                    break;
            }

            OnCollect();
        }

        if (other.CompareTag("Projectile"))
        {
            IProjectile projectile = other.GetComponent<IProjectile>();
            if (projectile.IsEnemyProjectile() == true)
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollect()
    {
        AudioManager.Instance.PlaySoundAtPlayer(_clip);
        _speed = 0;
        //turn off the renderer of the children components.
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.enabled = false;

        GetComponent<Collider>().enabled = false;

        //Always Last in this Method
        Destroy(this.gameObject, .9f);
    }
}
