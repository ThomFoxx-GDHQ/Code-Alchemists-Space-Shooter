using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _animationTime;

    private void Start()
    {
        //Destroy Object after time from Instantiation
        //Destroy(this.gameObject, _animationTime);
    }

    public void DestroyObject()
    {
        //Destroy Object through Event Call in Animation
        Destroy(this.gameObject);
    }
}
