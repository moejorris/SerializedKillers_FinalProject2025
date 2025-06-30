using Unity.VisualScripting;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    enum UpdateType { Update, FixedUpdate, LateUpdate }
    [SerializeField] UpdateType updateMethod;
    [SerializeField] Transform targetObject;
    void Update()
    {
        if (updateMethod != UpdateType.Update) return;
        Follow();
    }

    void FixedUpdate()
    {
        if (updateMethod != UpdateType.FixedUpdate) return;
        Follow();
    }

    void LateUpdate()
    {
        if (updateMethod != UpdateType.LateUpdate) return;
        Follow();
    }

    void Follow()
    {
        transform.rotation = targetObject.rotation;    
    }
}
