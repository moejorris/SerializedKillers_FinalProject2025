using UnityEngine;

public class RotatePuzzle : MonoBehaviour
{
    [SerializeField] private float targetYRotation;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private RotatePuzzleManager rotateManager => transform.parent.GetComponent<RotatePuzzleManager>();
    //[SerializeField] private GameObject jailCell;
    //private OpenJail jailCellScript;
    private float currentY;
    private bool isCorrect;

    private bool isRotating = false;
    private float desiredYRotation;

    private void Start()
    {
        desiredYRotation = transform.eulerAngles.y;
        //jailCellScript = jailCell.GetComponent<OpenJail>();
    }

    private void Update()
    {
        if (isRotating)
        {
            float newY = Mathf.MoveTowardsAngle(transform.eulerAngles.y, desiredYRotation, rotateSpeed * Time.deltaTime * 100f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newY, transform.eulerAngles.z);

            if (Mathf.Approximately(newY, desiredYRotation))
            {
                isRotating = false;
                CheckIfCorrect();
            }
        }
    }

    public void RotateThisPuzzle()
    {
        if (!isRotating && rotateManager.correctRotations != 4)
        {
            float currentY = Mathf.Round(transform.eulerAngles.y / 90f) * 90f;
            float nextY = currentY + 90f;
            if (nextY > 270f) nextY = 0f;

            desiredYRotation = nextY;
            isRotating = true;
        }
    }

    private void CheckIfCorrect()
    {
        float y = Mathf.Round(transform.eulerAngles.y) % 360f;
        float target = ((targetYRotation % 360f) + 360f) % 360f;
        bool wasCorrect = isCorrect;
        isCorrect = y == target;
        if (isCorrect && !wasCorrect)
        {
            Debug.Log("Puzzle solved!");
            //jailCellScript.AddToCounter();
            rotateManager.AddToCounter();
        }
        else if (!isCorrect && wasCorrect)
        {
            //jailCellScript.RemoveFromCounter();
            rotateManager.RemoveFromCounter();
        }
    }
}
