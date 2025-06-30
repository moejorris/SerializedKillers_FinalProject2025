using UnityEngine;

public class OpenJail : MonoBehaviour
{
    private int correctTilesToOpen = 4;
    private int currentTilesCorrect = 0;
    private Animator anim;
    private bool hasWon = false; // Track the win

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void AddToCounter()
    {
        currentTilesCorrect++;
        CheckWinCond();
    }

    public void RemoveFromCounter()
    {
        currentTilesCorrect = Mathf.Max(0, currentTilesCorrect - 1);
        CheckWinCond();
    }

    void CheckWinCond()
    {
        bool isWinning = currentTilesCorrect >= correctTilesToOpen;
        if (isWinning && !hasWon)
        {
            anim.SetTrigger("Activate");
        }
        else if (!isWinning && hasWon)
        {
            anim.SetTrigger("Drop");
        }
        hasWon = isWinning;
    }
}
