using UnityEngine;
using System.Collections.Generic;

public class CombatRoom : Room
{
    [Header("Combat Config")]
    [SerializeField] private List<GameObject> combatEnemies;


    public override void Start()
    {
        OpenDoors(entranceDoors);
    }

    public override void CheckEnemies()
    {
        RemoveListNulls(combatEnemies);
        if (combatEnemies.Count <= 0)
        {
            // enemies all dead (unless count starts at 0 freaking LAME!)
            RoomComplete();
        }
    }

    public override void RoomComplete()
    {
        base.RoomComplete();
        OpenDoors(exitDoors);
    }

    public override void BeginChallenge()
    {
        base.BeginChallenge();
        CloseDoors(entranceDoors);
    }
}
