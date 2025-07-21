using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private TutorialManager tutorialManager => GameObject.FindGameObjectWithTag("Canvas").transform.Find("Tutorial").GetComponent<TutorialManager>();

    [SerializeField] private TutorialPhase tutorialPhase;
    public enum TutorialPhase { PhaseOne, PhaseTwo, PhaseThree, PhaseFour, PhaseSix, PhaseSeven, PhaseEight };
    private void OnTriggerEnter(Collider other)
    {
        if (other == PlayerController.instance.Collider && !tutorialManager.isRunning)
        {
            switch (tutorialPhase)
            {
                case TutorialPhase.PhaseOne:
                    tutorialManager.StartPhaseOne(); break;
                case TutorialPhase.PhaseTwo:
                    tutorialManager.StartPhaseTwo(); break;
                case TutorialPhase.PhaseThree:
                    tutorialManager.StartPhaseThree(); break;
                case TutorialPhase.PhaseFour:
                    tutorialManager.StartPhaseFour(); break;
                case TutorialPhase.PhaseSix:
                    tutorialManager.StartPhaseSix(); break;
                case TutorialPhase.PhaseSeven:
                    tutorialManager.StartPhaseSeven(); break;
                case TutorialPhase.PhaseEight:
                    tutorialManager.StartPhaseEight(); break;
                default: break;
            }

            Destroy(gameObject);
        }
    }
}
