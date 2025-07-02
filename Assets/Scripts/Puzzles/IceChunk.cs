using UnityEngine;

public class IceChunk : MonoBehaviour, IElemental
{
    [SerializeField] private float maxHits = 5;
    [SerializeField] private float hits;
    [SerializeField] private Room room;
    private Vector3 newScale = Vector3.one;
    private void Start()
    {
        hits = maxHits;
        newScale = transform.localScale;
    }
    public void InteractElement(Behavior behavior)
    {
        if (behavior == null) return;
        if (behavior.behaviorName == "fire")
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        hits--;
        newScale = new Vector3 (1, 1 * (hits / maxHits), 1);
        transform.Find("IceChunk").GetComponent<Animator>().Play("ObjectShake", 0, 0);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * 15);
        if (transform.localScale.y <= 0)
        {
            if (room != null) room.RoomComplete();
            Destroy(gameObject);
        }
    }
}
