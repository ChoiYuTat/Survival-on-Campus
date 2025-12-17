using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public GameObject interactionButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactionButton != null)
        {
            interactionButton.SetActive(false);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door trigger area.");
            ShowButton();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideButton();
        }
    }

    void  ShowButton()
    {
        if (interactionButton != null)
        {
            interactionButton.SetActive(true);
        }
    }

    void HideButton()
    {
        if (interactionButton != null)
        {
            interactionButton.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
