using UnityEngine;
using System.Collections;

public class TemporaryElevatorTrigger : MonoBehaviour
{
    public float topY = 55f;
    public float bottomY = 30f;
    public float speed = 2f;
    public float waitTime = 2f;

    private Vector3 targetPosition;
    private bool isWaiting = false;

    void Start()
    {
        targetPosition = new Vector3(transform.position.x, topY, transform.position.z);
    }

    void Update()
    {
        if (isWaiting) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        float newY = targetPosition.y == topY ? bottomY : topY;
        targetPosition = new Vector3(transform.position.x, newY, transform.position.z);

        isWaiting = false;
    }
}