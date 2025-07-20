using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound Effects/Variable Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    public List<AudioClip> soundVariations = new();
    public float volume = 1f;
    public bool usesRandomPitch = false;
    [SerializeField] float minPitch = 0.9f;
    [SerializeField] float maxPitch = 1.1f;

    public float RandomPitch { get => Random.Range(minPitch, maxPitch); }


    /// <summary>
    /// Returns a random sound effect.
    /// </summary>
    [HideInInspector]
    public AudioClip SoundEffect() //returns a random sound effect
    {
        if (soundVariations.Count < 2)
        {
            return soundVariations[0];
        }
        else
        {
            return soundVariations[Random.Range(0, soundVariations.Count)];
        }
    }


    /// <summary>
    /// Returns a specific sound effect by index.
    /// </summary>
    [HideInInspector]
    public AudioClip GetSoundEffectByIndex(int searchIndex) //returns a specific sound effect in the list by index. If the index value given is less than zero, it returns the first item. If the value given is too great it returns the last item in the list.
    {
        searchIndex = Mathf.Clamp(searchIndex, 0, soundVariations.Count - 1);
        return soundVariations[searchIndex];
    }

    /// <summary>
    /// Returns a specific sound effect by name.
    /// </summary>
    [HideInInspector]
    public AudioClip GetSoundEffectByName(string searchName) //returns a specific sound effect in the list by name. If the specified sound effect can't be found it returns a random sound effect in the list.
    {
        foreach (AudioClip sound in soundVariations)
        {
            if (sound.name == searchName)
            {
                return sound;
            }
        }

        Debug.LogError("Unable to find AudioClip in SoundVarations by name of " + searchName + ". ");
        return SoundEffect();
    }
}
