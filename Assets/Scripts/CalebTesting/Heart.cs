using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private float healAmount = 4;
    [SerializeField] private float rotateSpeed = 1;
    [SerializeField] private float hoverAmount = 1;
    [SerializeField] private float fineTuning = 0.5f;
    public float rollCycle;
    private Transform heart => transform.Find("Heart");
    //public Vector3 startPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_HealthComponent>().Heal(healAmount);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Vector3 rot = heart.eulerAngles;
        rot.y += Time.deltaTime * rotateSpeed;
        heart.eulerAngles = rot;

        Vector3 pos = transform.position;
        rollCycle += Mathf.PI / (Time.deltaTime * fineTuning);
        rollCycle = rollCycle % (Mathf.PI * 2);
        pos.y = transform.position.y + 0.3f + Mathf.Sin(rollCycle) * hoverAmount;
        heart.position = pos;

        GetComponent<Rigidbody>().AddForce(Vector3.down / 2.5f);
    }
}
