using UnityEngine;

public class PlayHealSound : MonoBehaviour
{
    [SerializeField] private SoundEffectSO healthBloop;
    [SerializeField] private SoundEffectSO darkFwoop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayFwoop()
    {
        SoundManager.instance.PlaySoundEffect(darkFwoop);
    }

    public void PlayHeart()
    {
        SoundManager.instance.PlaySoundEffect(healthBloop);
    }

    public void CutsceneDone()
    {
        GetComponent<Animator>().SetTrigger("CutsceneDone");
    }
}
