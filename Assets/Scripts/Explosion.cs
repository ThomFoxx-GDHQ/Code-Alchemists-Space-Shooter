using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void DestroyObject()
    {
        //Destroy Object through Event Call in Animation
        Destroy(this.gameObject);
    }
}
