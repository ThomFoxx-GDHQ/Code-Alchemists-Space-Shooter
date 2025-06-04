using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotationSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * _rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            SpawnManager.Instance.StartSpawning();
            Destroy(this.gameObject);
        }
    }
}
