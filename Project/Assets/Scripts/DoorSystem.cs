using UnityEditor.Rendering;
using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject targetDoor, existPosition;

    [SerializeField]
    private GameObject targetCamera, currenCamera;

    public void openDoor(GameObject door) 
    {
        existPosition.transform.position = targetDoor.transform.position;
        targetCamera.SetActive(true);
        currenCamera.SetActive(false);
    }
}
