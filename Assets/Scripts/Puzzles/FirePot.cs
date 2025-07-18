using UnityEngine;
using System.Collections;

public class FirePot : MonoBehaviour, IElemental
{
    [SerializeField] private Behavior requiredBehavior;
    private bool lit = false;
    [SerializeField] private TutorialRoom room => transform.parent.GetComponent<TutorialRoom>();
    [SerializeField] private Renderer gemMesh;


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
        StartCoroutine("FireGemFade");
    }

    IEnumerator FireGemFade()
    {
        Material[] materialsCopy = gemMesh.materials;
        Material endMaterial = materialsCopy[1];

        Color EmisColor = new Color(0.8396226f, 0.2738763f, 0.003960493f);



        for (int i = 0; i < 5; i++)
        {
            float i2 = i;
            yield return new WaitForSeconds(0.1f);
            endMaterial.EnableKeyword("_EMISSION");
            endMaterial.SetVector("_EmissionColor", Color.Lerp(Color.black, new Vector4(EmisColor.r,EmisColor.g,EmisColor.b, i2/5), i2/5));
            materialsCopy[1] = endMaterial;
            gemMesh.materials = materialsCopy;
        }

    }
}
