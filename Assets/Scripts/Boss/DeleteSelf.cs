using UnityEngine;

public class DeleteSelf : MonoBehaviour
{
    // Script should only be used to delete the GameObject is is attached to using animation events.

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
