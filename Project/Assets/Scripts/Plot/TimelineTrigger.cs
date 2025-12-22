using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector director; 
    public DialogueController dialogueController;
    public GameObject otherTrigger;
    public GameObject playerData;
    public string ID;

    private bool triggered = false;

    private void Start()
    {
        Invoke("LoadTrigger", 0.01f);
    }

    void LoadTrigger() 
    {
        Debug.Log(playerData.GetComponent<LoadPlayerData>().data.plotID.Count);
        for (int i = 0; i < playerData.GetComponent<LoadPlayerData>().data.plotID.Count; i++) 
        {
            Debug.Log("Checking triggered timelines: " + playerData.GetComponent<LoadPlayerData>().data.plotID[i]);
            if (playerData.GetComponent<LoadPlayerData>().data.plotID[i] == ID)
            {
                gameObject.SetActive(false);
                triggered = true;
                Debug.Log("Timeline " + ID + " already triggered, disabling trigger.");
                break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            playerData.GetComponent<LoadPlayerData>().data.plotID.Add(ID);
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
            otherTrigger.SetActive(true);
            if (otherTrigger.GetComponent<TimelineTrigger>() != null)
                otherTrigger.GetComponent<TimelineTrigger>().ResetTrigger();
        }
    }

    public void ResetTrigger()
    {
        triggered = false;
    }   
}
