using UnityEngine;

public class EnemyFootstep : MonoBehaviour
{
    [SerializeField] private SoundEffectSO footsteps;
    [SerializeField] private EnemyAI_Base enemyScript;

    public void PlaySound()
    {
        enemyScript.PlaySound(footsteps);
    }
}
