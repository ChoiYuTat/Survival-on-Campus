using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector director; 

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone, playing timeline.");
            triggered = true;
            director.Play(); 
        }
    }
}
