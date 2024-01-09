using UnityEngine;

public class MessageDestroyer : MonoBehaviour
{
    public float destroyDelay = 60f;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}
