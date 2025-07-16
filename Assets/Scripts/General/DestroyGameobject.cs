using UnityEngine;

public class DestroyGameobject : MonoBehaviour
{
    [SerializeField] private GameObject objectToDestroy;

    public void DestroyObject()
    {
        if (objectToDestroy != null) Destroy(transform.gameObject);
        else Destroy(objectToDestroy);
    }
}
