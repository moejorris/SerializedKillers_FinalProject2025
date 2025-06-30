using Unity.VisualScripting;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    enum UpdateType { Update, FixedUpdate, LateUpdate }
    [SerializeField] UpdateType updateMethod;
    [SerializeField] Transform targetObject;
    [SerializeField] float lerpSpeed = 0;
    [SerializeField] Vector3 offset = new();
    void Update()
    {
        if (updateMethod != UpdateType.Update) return;
        Follow(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (updateMethod != UpdateType.FixedUpdate) return;
        Follow(Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        if (updateMethod != UpdateType.LateUpdate) return;
        Follow(Time.deltaTime);
    }

    void Follow(float deltaTime)
    {
        if (lerpSpeed != 0)
        {
            transform.position = Vector3.Slerp(transform.position, targetObject.position + offset, deltaTime * lerpSpeed);
        }
        else
        {
            transform.position = targetObject.position + offset;
        }
    }
}
