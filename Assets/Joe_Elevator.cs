using System.Collections;
using UnityEngine;

public class Joe_Elevator : MonoBehaviour
{
    [SerializeField] Transform startPosition;
    [SerializeField] Transform endPosition;
    [SerializeField] float travelTime;
    [SerializeField] Transform platform;
    [SerializeField] BoxCollider platformCollider;

    void Start()
    {
        StartCoroutine(MoveElevator());
    }



    void OnDrawGizmos()
    {
        if (!startPosition || !endPosition || !platformCollider) return;

        Vector3 platfColliderSize = new Vector3
        (
            platformCollider.size.x * platform.localScale.x,
            platformCollider.size.y * platform.localScale.y,
            platformCollider.size.z * platform.localScale.z
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startPosition.position, platfColliderSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(endPosition.position, platfColliderSize);
    }

    IEnumerator MoveElevator()
    {
        float curTime = 0;

        while (curTime < travelTime)
        {

            platform.position = Vector3.Lerp(startPosition.position, endPosition.position, curTime / travelTime);

            yield return new WaitForEndOfFrame();
            curTime += Time.deltaTime;
        }

        platform.position = endPosition.position;
    }
}
