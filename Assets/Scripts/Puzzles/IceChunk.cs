using UnityEngine;

public class IceChunk : MonoBehaviour, IElemental
{
    public float maxHits = 5;
    public float hits;
    public float healthForAction = 0;
    [SerializeField] private Room room;
    public ParticleSystem snow;
    public Vector3 newScale = Vector3.one;
    public Vector3 startScale = Vector3.one;
    private void Start()
    {
        startScale = transform.localScale;
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

    public virtual void TakeDamage()
    {
        hits--;
        newScale = new Vector3 (startScale.x, startScale.y * (hits / maxHits), startScale.z);
        transform.Find("IceChunk").GetComponent<Animator>().Play("ObjectShake", 0, 0);


        if (hits <= healthForAction)
        {
            PerformAction();
        }

        if (hits <= 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * 15);
    }

    public virtual void PerformAction()
    {
        if (room != null) room.RoomComplete();
        if (snow != null) snow.Stop();
    }
}
