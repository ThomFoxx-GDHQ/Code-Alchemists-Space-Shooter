using UnityEngine;

public class MineBehavior : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    Player _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player.gameObject)
        {
            _player.Damage();
            Instantiate(_explosionPrefab,transform.position,Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
