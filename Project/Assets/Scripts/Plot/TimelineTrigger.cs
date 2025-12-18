using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector director; 
    public DialogueController dialogueController;
    public GameObject otherTrigger;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            director.enabled = true;
            dialogueController.director = director;
            Debug.Log("Player entered trigger zone, playing timeline.");
            triggered = true;
            director.Play(); 
        }
    }

    public void EnableOtherTrigger() 
    {
        if (otherTrigger != null) 
        {
            otherTrigger.GetComponent<Collider>().enabled = true;
        }
    }
}
