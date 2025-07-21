using UnityEngine;

public class ElevatorLever : MonoBehaviour, IDamageable
{
    [SerializeField] Joe_MovingPlatform elevator;
    [SerializeField] Transform leverTransform;
    [SerializeField] private SoundEffectSO leverOnSFX;
    public void TakeDamage(float damage = 0)
    {
        leverTransform.localEulerAngles = new Vector3(-45f, 0, 0);
        elevator.StartMoving();
        SoundManager.instance.PlaySoundEffectOnObject(leverOnSFX, leverTransform);
    }
}
