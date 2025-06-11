using System.Collections.Generic;
using UnityEngine;

public class HitBoxGenerator : MonoBehaviour
{
    [SerializeField] bool showHitboxes = false;
    [SerializeField] List<SO_Hitbox> hitboxes = new();

    void OnDrawGizmos()
    {
        if (!showHitboxes) return;

        foreach (SO_Hitbox hitbox in hitboxes)
        {
            Gizmos.color = hitbox.gizmoColor;

            Vector3 pos = hitbox.position;

            Transform parent;

            if (hitbox.parentPrefab != null)
            {
                parent = hitbox.parentPrefab.transform.Find(hitbox.parentBone);

                if (hitbox.parentBone != "" && parent != null)
                {
                    pos += parent.position;
                }
            }



            if (hitbox.shape == SO_Hitbox.HitboxShape.Sphere)
            {
                Gizmos.DrawWireSphere(pos, hitbox.size);
            }
            else
            {
                Gizmos.DrawWireCube(pos, hitbox.size * Vector3.one);
            }
        }
    }
}
