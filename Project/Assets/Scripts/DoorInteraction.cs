using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public GameObject interactionimage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactionimage != null)
        {
            interactionimage.SetActive(false);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door trigger area.");
            ShowImage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideImage();
        }
    }

    void  ShowImage()
    {
        if (interactionimage != null)
        {
            interactionimage.SetActive(true);
        }
    }

    void HideImage()
    {
        if (interactionimage != null)
        {
            interactionimage.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
