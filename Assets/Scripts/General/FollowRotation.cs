using Unity.VisualScripting;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    enum UpdateType { Update, FixedUpdate, LateUpdate }
    [SerializeField] UpdateType updateMethod;
    [SerializeField] Transform targetObject;
    void Update()
    {
        if (!updateMethod.Equals(UpdateType.Update)) return;
        Follow();
    }

    void FixedUpdate()
    {
        if (!updateMethod.Equals(UpdateType.FixedUpdate)) return;
        Follow();
    }

    void LateUpdate()
    {
        if (!updateMethod.Equals(UpdateType.LateUpdate)) return;
        Follow();
    }

    void Follow()
    {
        transform.rotation = targetObject.rotation;    
    }
}
