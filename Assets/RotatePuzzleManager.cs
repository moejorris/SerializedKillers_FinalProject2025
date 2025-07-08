using UnityEngine;

public class RotatePuzzleManager : MonoBehaviour
{
    [SerializeField] private Animator[] gates;
    [SerializeField] private Collider[] invincibleAreas;
    public int correctRotations = 0;
    private bool complete = false;

    public void AddToCounter()
    {
        correctRotations++;
        Check();
    }

    public void RemoveFromCounter()
    {
        correctRotations--;
    }
    
    public void Check()
    {
        if (complete) return;

        if (correctRotations >= 4)
        {
            complete = true;
            foreach (Animator anim in gates)
            {
                anim.SetBool("Open", true);
            }

            foreach (Collider collider in invincibleAreas)
            {
                collider.enabled = false;
            }
        }
    }
}
