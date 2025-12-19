using UnityEngine;

public class ShowEnd : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endScreen.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
}
