using NUnit.Framework;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 5;
    [Header("Screenplay Boundaries")]
    [SerializeField] float _leftBound;
    [SerializeField] float _rightBound;
    [SerializeField] float _topBound;
    [SerializeField] float _bottomBound;

    private bool _canRespawn = true;

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y < _bottomBound && _canRespawn)
        {
            Respawn();
        }
        else if (transform.position.y < _bottomBound && _canRespawn == false)
        {
            Destroy(this.gameObject);
        }
    }

    private void Respawn()
    {
        float rng = Random.Range(_leftBound, _rightBound);
        transform.position = new Vector3(rng, _topBound, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>()?.Damage();     
            Destroy(this.gameObject);
        }
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void OnPlayerDeath()
    {
        _canRespawn = false;
    }
}
