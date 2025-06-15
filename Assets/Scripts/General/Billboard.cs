using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private BillboardType billboardType;
    public enum BillboardType { Method_1, Method_2 };
    [SerializeField] private bool followTransform = false;
    [SerializeField] private Transform followedTransform;
    private float yOffset;

    private void Start()
    {
        if (followedTransform)
        {
            yOffset = transform.position.y - followedTransform.position.y;
        }
    }

    private void LateUpdate()
    {
        switch (billboardType)
        {
            case BillboardType.Method_1:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;
            case BillboardType.Method_2:
                transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (followTransform)
        {
            transform.position = new Vector3 (followedTransform.position.x, followedTransform.position.y + yOffset, followedTransform.position.z);
        }
    }
}
