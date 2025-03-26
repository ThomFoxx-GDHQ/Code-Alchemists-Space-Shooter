using UnityEngine;


public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _bottomBounds = -12f;
    [SerializeField] private PowerUpType _powerUpID;
    [SerializeField] private AudioClip _clip;

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y <= _bottomBounds)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();            

            switch (_powerUpID)
            {
                case PowerUpType.None:
                    Debug.Log("No PowerUp to to this Item!");
                    break;
                case PowerUpType.QuadShot:
                    player.ActivateQuadShot();
                    break;
                case PowerUpType.SpeedBoost:
                    player.ActivateSpeedBoost();
                    break;
                case PowerUpType.Shield:
                    player.ShieldActive(true, 99);
                    break;
                default:
                    break;
            }

            OnCollect();
        }
    }

    private void OnCollect()
    {
        AudioManager.Instance.PlaySoundAtPlayer(_clip);
        _speed = 0;
        GetComponentInChildren<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        //Always Last in this Method
        Destroy(this.gameObject, .9f);
    }
}
