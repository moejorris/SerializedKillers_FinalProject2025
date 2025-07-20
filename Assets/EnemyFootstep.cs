using UnityEngine;

public class EnemyFootstep : MonoBehaviour
{
    [SerializeField] private SoundEffectSO footsteps;
    [SerializeField] private EnemyAI_Base enemyScript;
    [SerializeField] private bool overclock;
    private EnemyAI_Overclock overclockScript;

    private void Start()
    {
        if (overclock && enemyScript.transform.GetComponent<EnemyAI_Overclock>())
        {
            overclockScript = enemyScript.transform.GetComponent<EnemyAI_Overclock>();
        }
    }
    public void PlaySound()
    {
        if (overclock && overclockScript != null)
        {
            overclockScript.PlayFootstep();
        }
        else
        {
            enemyScript.PlaySound(footsteps);
        }
    }
}
