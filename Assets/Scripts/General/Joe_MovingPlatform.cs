using System.Collections;
using UnityEngine;

public class Joe_MovingPlatform : MonoBehaviour
{
    [SerializeField] bool isOneWay = false;
    [SerializeField] Transform startPosition;
    [SerializeField] Transform endPosition;
    [SerializeField] float travelTime = 3f;
    [SerializeField] Transform platform;

    bool isMoving = false;

    enum MeshDraw { BoxCollider, PlatformMesh, None }

    [Header("Gizmos Settings")]
    [SerializeField] MeshDraw platformGizmo = MeshDraw.BoxCollider;
    [SerializeField] Color startPositionColor = Color.red;
    [SerializeField] Color endPositionColor = Color.green;

    void Start()
    {
        platform.position = startPosition.position;
    }

    void OnDrawGizmos()
    {
        if (!startPosition || !endPosition || platformGizmo == MeshDraw.None) return;

        if (!Application.isPlaying)
        {
            platform.position = startPosition.position;
        }

        if (platformGizmo == MeshDraw.PlatformMesh)
        {
            MeshGizmo();
        }
        else
        {
            BoxGizmo();
        }
    }

    IEnumerator MoveElevator()
    {
        isMoving = true;

        Vector3 curStart = platform.position;

        Vector3 curEnd = platform.position == startPosition.position ? endPosition.position : startPosition.position;

        float curTime = 0;

        while (curTime < travelTime)
        {
            platform.position = Vector3.Lerp(curStart, curEnd, curTime / travelTime);

            yield return new WaitForEndOfFrame();
            curTime += Time.deltaTime;
        }

        platform.position = curEnd;

        isMoving = false;
    }

    public void StartMoving()
    {
        if ((transform.position == endPosition.position && isOneWay) || isMoving)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(MoveElevator());

    }


    void BoxGizmo()
    {
        BoxCollider platformCollider = platform.GetComponent<BoxCollider>();

        if (platformCollider == null) return;

        Vector3 platfColliderSize = new Vector3
        (
            platformCollider.size.x * platform.localScale.x,
            platformCollider.size.y * platform.localScale.y,
            platformCollider.size.z * platform.localScale.z
        );

        Gizmos.color = startPositionColor;
        Gizmos.DrawWireCube(startPosition.position, platfColliderSize);

        Gizmos.color = endPositionColor;
        Gizmos.DrawWireCube(endPosition.position, platfColliderSize);
    }

    void MeshGizmo()
    {
        Mesh platformMesh = platform.GetComponent<MeshFilter>().mesh;

        if (platformMesh == null) return;

        Gizmos.color = startPositionColor;
        Gizmos.DrawWireMesh(platformMesh, startPosition.position, Quaternion.identity, platform.localScale);

        Gizmos.color = endPositionColor;
        Gizmos.DrawWireMesh(platformMesh, endPosition.position, Quaternion.identity, platform.localScale);
    
    }
}
