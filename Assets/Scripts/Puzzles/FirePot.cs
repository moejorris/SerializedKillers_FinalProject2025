using UnityEngine;

public class FirePot : MonoBehaviour, IElemental
{
    [SerializeField] private Behavior requiredBehavior;
    private bool lit = false;
    [SerializeField] private TutorialRoom room => transform.parent.GetComponent<TutorialRoom>();
    public void InteractElement(Behavior behavior)
    {
        if (behavior == requiredBehavior && PlayerController.instance.ScriptSteal.BehaviorActive())
        {
            LightFire();
        }
    }

    public void LightFire()
    {
        if (lit) return;

        lit = true;
        transform.Find("Fire").gameObject.SetActive(true);
        //transform.Find("Smoke").GetComponent<ParticleSystem>().Stop();
        room.StartFire();
    }
}
