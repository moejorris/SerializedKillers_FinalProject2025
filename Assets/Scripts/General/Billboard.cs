using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private BillboardType billboardType;
    public enum BillboardType { Method_1, Method_2 };

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
}
