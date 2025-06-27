using UnityEngine;

public class ElectricBarrier : MonoBehaviour
{
    [SerializeField] private Behavior heldBehavior; // just to tell which type of element(?)
    [SerializeField] private int fps;
    private ParticleSystem electricity;
    private float timeElapsed = 0;
    private float displayTime = 0;
    public float fireDuration = 5;
    private Player_ScriptSteal playerScriptSteal => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_ScriptSteal>();

    private void Start()
    {
        electricity = transform.Find("Electricity").GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        timeElapsed += Time.deltaTime;

        if ((timeElapsed - displayTime) > 1f / fps)
        {
            displayTime = timeElapsed;
            electricity.Simulate(0.15f, true, false, false);
            electricity.Pause();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
            Debug.Log(other.gameObject.name);
            if (other.transform.parent != null && other.transform.parent.GetComponent<EnemyAI_Base>() != null)
            {
                if (other.transform.parent.GetComponent<EnemyAI_Base>().heldBehavior.behaviorName == "water" && other.transform.parent.GetComponent<EnemyAI_Base>().behaviorActive)
                {
                    //PutOutFire();
                }
            }
            else if (other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
            {
                //Vector3 dir = (other.transform.position - transform.position).normalized + Vector3.up * 0.25f;
                //GameObject.Find("Player").transform.Find("PlayerController").GetComponent<Player_ForceHandler>().AddForce(dir * 20f, ForceMode.VelocityChange);

                
            }
    }
}
