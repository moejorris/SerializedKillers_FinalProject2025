using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private float healAmount = 4;
    [SerializeField] private float rotateSpeed = 1;
    [SerializeField] private float hoverAmount = 1;
    [SerializeField] private float fineTuning = 0.5f;
    public float rollCycle;
    private Transform heart => transform.Find("Heart");
    [SerializeField] private GameObject pickupPartcle;
    //public Vector3 startPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other == PlayerController.instance.Collider && !PlayerController.instance.Health.HasMaxHealth())
        {
            PlayerController.instance.Health.Heal(healAmount);
            Instantiate(pickupPartcle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Vector3 rot = heart.eulerAngles;
        rot.y += Time.deltaTime * rotateSpeed;
        heart.eulerAngles = rot;

        Vector3 pos = transform.position;
        rollCycle += Mathf.PI * (Time.deltaTime * fineTuning);
        rollCycle = rollCycle % (Mathf.PI * 2);
        pos.y = 0.1f + transform.position.y + 0.3f + Mathf.Sin(rollCycle) * hoverAmount;
        heart.position = pos;

        GetComponent<Rigidbody>().AddForce(Vector3.down / 2.5f);
    }
}
