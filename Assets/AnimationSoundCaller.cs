using UnityEngine;

public class AnimationSoundCaller : MonoBehaviour
{
    public void PlayFootsteps()
    {
        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("FootstepStoneSound"));
    }

    public void PlayJump()
    {
        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("JumpSound"));
    }
    public void PlayLand()
    {
        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("LandSound"));
    }

    public void PlayDash()
    {
        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("DashSound"));
    }
    public void PlayHeal()
    {
        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("PlayerHealSound"));
    }
    public void PlayDamage()
    {
        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("PlayerDamageSound"));
    }
}
