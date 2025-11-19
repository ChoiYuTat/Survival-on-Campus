using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector director; 
    public DialogueController dialogueController;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            dialogueController.director = director;
            Debug.Log("Player entered trigger zone, playing timeline.");
            triggered = true;
            director.Play(); 
        }
    }
}
