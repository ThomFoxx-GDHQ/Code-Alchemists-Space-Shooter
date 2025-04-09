using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    [SerializeField] bool _isEnemyProjectile;

    public bool IsEnemyProjectile
    {
        get { return _isEnemyProjectile; }
    }

    //private void Start()
    //{
    //    Debug.Log($"Direction of travel: {transform.up}");
    //}

    private void Update()
    {
        transform.Translate(transform.up * (_speed * Time.deltaTime), Space.World);
        //transform.Translate(Vector3.up * (_speed * Time.deltaTime), Space.World);
        

        if (Mathf.Abs(transform.position.y) > 10)
        {
            if (!transform.parent.CompareTag("Container"))
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isEnemyProjectile == true)
        {
            other.GetComponent<Player>()?.Damage();
            Destroy(this.gameObject);
        }
    }
}
