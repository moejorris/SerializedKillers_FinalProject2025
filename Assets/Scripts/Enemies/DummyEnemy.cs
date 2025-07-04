using UnityEngine;

public class DummyEnemy : MonoBehaviour, ITargetable, IDamageable, IComboTarget
{
    public float TargetScore { get; set; }

    public void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        //Dummy enemies don't have health so we're fine
    }
}
