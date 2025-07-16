using UnityEngine;

public class RotatePuzzleV2 : MonoBehaviour, IDamageable
{
    private Animator anim;
    [SerializeField] private int correctRotations = 0;
    private int currentRotations = 0;
    [SerializeField] private RotatePuzzleManager rotateManager => transform.parent.GetComponent<RotatePuzzleManager>();
    private bool isCorrect;

    private bool isRotating = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void RotateThisPuzzle()
    {
        if (isRotating) return;
        anim.SetTrigger("Activate");

        isRotating = true;

        currentRotations++;
        if (currentRotations >= 4)
        {
            currentRotations = 0;
        }
        Debug.Log("Puzzle is rotating, current rotations: " + currentRotations);
    }


    public void TakeDamage(float damage)
    {
        RotateThisPuzzle();
    }

    void CanRotate() // Called with an animation event
    {
        isRotating = false;
    }

    private void CheckIFCorrect()
    {
        bool wasCorrect = isCorrect;
        isCorrect = currentRotations == correctRotations;
        if (isCorrect && !wasCorrect)
        {
            Debug.Log("Puzzle Solved!");
            rotateManager.AddToCounter();
        }
        else if (!isCorrect && wasCorrect)
        {
            Debug.Log("Puzzle Unsolved!");
            rotateManager.RemoveFromCounter();
        }
    }
}
