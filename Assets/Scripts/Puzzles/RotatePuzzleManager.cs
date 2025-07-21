using UnityEngine;

public class RotatePuzzleManager : MonoBehaviour
{
    [SerializeField] private Animator[] gates;
    [SerializeField] private Collider[] invincibleAreas;
    public int correctRotations = 0;
    private bool complete = false;
    [SerializeField] private SoundEffectSO gateOpenSFX;

    public void AddToCounter()
    {
        correctRotations++;
        Check();
    }

    public void RemoveFromCounter()
    {
        correctRotations--;
        Check();
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
                SoundManager.instance.PlaySoundEffect(gateOpenSFX);
            }

            foreach (Collider collider in invincibleAreas)
            {
                collider.enabled = false;
            }
        }
    }
}
