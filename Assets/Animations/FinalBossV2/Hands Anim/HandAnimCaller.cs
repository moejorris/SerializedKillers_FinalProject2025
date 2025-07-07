using UnityEngine;

public class HandAnimCaller : MonoBehaviour
{
    public Animator hController;
    public void handMove()
    {hController.SetTrigger("Hand Move");}

    public void handSummon()
    {hController.SetTrigger("Hand Summon");}

    public void handSmash()
    { hController.SetTrigger("Hand Smash"); }

    public void handSwipe()
    { hController.SetTrigger("Hand Swipe"); }

    public void handStunned()
    { hController.SetTrigger("Hand Stunned"); }
    public void handStunnedLoop()
    { hController.SetTrigger("Hand Stunned Loop"); }
    public void handStunnedHit()
    { hController.SetTrigger("Hand Stunned Hit"); }
}
