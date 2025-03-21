using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    private void Update()
    {
        transform.Translate(Vector3.up * (_speed * Time.deltaTime));

        if (transform.position.y > 10)
        {
            if (!transform.parent.CompareTag("Container"))
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }
}
