using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private int damage = 10; // Damage dealt by the projectile
    [SerializeField] private GameObject barrelExplosionFX; // Explosion effect when the projectile hits
    [SerializeField] private GameObject barrelObj;
    [SerializeField] private float explosionRadius = 5f; // Radius of the explosion
    private ParticleSystem ps;
    private float lifeTime = 5f; // Lifetime of the projectile

    void Awake()
    {
        ps = barrelExplosionFX.GetComponent<ParticleSystem>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Explode();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Explode();
        }
    }

    void Explode()
    {
        barrelObj.SetActive(false); // Deactivate the barrel object
        if (ps != null)
        {
            ps.Play(); // Play the explosion particle system
        }
        Destroy(gameObject, 1f); // Destroy the projectile after the particle system finishes
        Collider[] col = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Player"));
        foreach (Collider c in col)
        {
            Player_HealthComponent playerHealth = c.GetComponent<Player_HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                KnockPlayerBack(10f); // Knock the player back with a force of 10
            }
        }
    }


    public void KnockPlayerBack(float force)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = 0.5f;
        dir *= force;
        PlayerController.instance.ForceHandler.AddForce(dir, ForceMode.VelocityChange);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Draw the explosion radius in the
    }
}
