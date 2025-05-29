using UnityEngine;
using System.Collections.Generic;

public class BossBehavior : MonoBehaviour
{
    #region Unity Inspector Variables
    [Header("Boss Components")]
    [Tooltip("Prefab for the projectile that the boss will spawn.")]
    [SerializeField] private GameObject projectilePrefab; // Prefab for the projectile
    [Tooltip("Check to enable testing boss behavior.")]
    [SerializeField] private bool isTesting = false; // Flag to enable testing mode
    private Transform player; // Reference to the player's transform
    private Animator anim; // Reference to the animator component for animations

    #endregion

    #region Unity Methods
    void Awake()
    {
        // Find the playr by tag and assign it to the player variable
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
        }

        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found! Make sure this GameObject has an Animator component attached.");
        }
    }

    void Update()
    {
        if (isTesting)
        {
            // If testing mode is enabled, the boss will spawn a projetile when the P key is pressed
            if (Input.GetKeyDown(KeyCode.P))
            {
                anim.SetTrigger("Fall"); // Trigger the "Fall" animation
            }
        }
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && player != null)
        {
            Instantiate(projectilePrefab, new Vector3(player.position.x, transform.position.y + 15, player.position.z), Quaternion.identity); // Spawn the projectile above the player
        }
        else
        {
            Debug.LogError("Projectile prefab or player reference is missing!"); // Log an error if the prefab or player is not set
        }
    }

    #endregion

    #region Anmation Methods

    #endregion
}
