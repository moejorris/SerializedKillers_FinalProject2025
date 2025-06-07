using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        [SerializeField] public TKey Key;
        [SerializeField] public TValue Value;
    }

public class Dev_TimeScaleController : MonoBehaviour
{
    [SerializeField] List<SerializedKeyValuePair<KeyCode, float>> timeScaleHotkeys = new List<SerializedKeyValuePair<KeyCode, float>>();

    void LateUpdate()
    {
        for (int i = 0; i < timeScaleHotkeys.Count; i++)
        {
            if (Input.GetKeyDown(timeScaleHotkeys[i].Key))
            {
                Time.timeScale = timeScaleHotkeys[i].Value;
            }
        }
    }
}
