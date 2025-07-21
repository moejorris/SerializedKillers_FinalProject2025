using UnityEngine;
using System.Collections;

public class LaserPuzzleRoom : PuzzleRoom
{
    public int weightsFallen = 0;
    public GameObject laser;
    [SerializeField] Animator[] gates;
    [SerializeField] private SoundEffectSO laserSound;


    public void WeightFallen()
    {
        weightsFallen++;

        if (weightsFallen >= 2)
        {
            Invoke("FireLaser", 1.5f);
        }
    }

    public void FireLaser()
    {
        StopCoroutine("LaserFire");
        StartCoroutine("LaserFire");
    }

    IEnumerator LaserFire()
    {
        while (laser.transform.localScale.y < 1)
        {
            Vector3 scale = laser.transform.localScale;
            scale.y += 0.03f;
            laser.transform.localScale = scale;
            yield return new WaitForSeconds(0.01f);
        }

        RoomComplete();

        foreach (Animator gate in gates)
        {
            yield return new WaitForSeconds(0.2f);
            if (sfx_gateOpen != null) SoundManager.instance.PlaySoundEffectOnObject(sfx_gateOpen, gate.transform);
            gate.SetBool("Open", true);
        }

        StartCoroutine("LaserSFXLoop");
    }

    IEnumerator LaserSFXLoop()
    {
        while (true)
        {
            SoundManager.instance.PlaySoundEffectOnObject(laserSound, laser.transform);
            yield return new WaitForSeconds(12);
        }
    }
}
