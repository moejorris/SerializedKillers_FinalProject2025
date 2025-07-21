using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound Effects/Sound Bank")]
public class SoundBankSO : ScriptableObject
{
    public List<SoundEffectSO> soundEffects = new();

    public SoundEffectSO GetSoundByName(string name)
    {
        SoundEffectSO returnValue = null;

        foreach (SoundEffectSO sound in soundEffects)
        {
            if (sound.name == name)
            {
                returnValue = sound;
            }
        }

        return returnValue;
    }
}
