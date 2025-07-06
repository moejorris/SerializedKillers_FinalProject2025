using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HealthComponent : Health
{
    [SerializeField] PlayerHealth playerHealthUI => GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();
    [SerializeField] private Renderer[] meshes;
    [SerializeField] private List<Material> materialList;

    private void Start()
    {
        foreach (Renderer mesh in meshes)
        {
            foreach (Material material in mesh.materials)
            {
                materialList.Add(material);
                //Debug.Log("added " + material);
            }
        }
    }

    public override void TakeDamage(float damage = 0)
    {
        base.TakeDamage(damage);

        if (currentHealth < 0) currentHealth = 0;

        playerHealthUI.TakeDamage(currentHealth);

        StopCoroutine("MaterialFade");
        StartCoroutine("MaterialFade");
    }

    public override void Heal(float healAmount = 0)
    {
        base.Heal(healAmount);

        playerHealthUI.HealDamage(currentHealth);
    }

    public void UpdatePlayerHealth()
    {
        playerHealthUI.UpdateHealth(currentHealth);
    }

    public override void Die()
    {
        PlayerController.instance.Respawn.Respawn();
    }

    IEnumerator MaterialFade()
    {
        float percent = 0;
        for (int i = 0; i < 5; i++)
        {
            foreach (Material material in materialList)
            {
                material.SetFloat("_Percentage", percent);
            }
            if (i == 0) yield return new WaitForSeconds(0.04f);
            percent += 0.25f;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
