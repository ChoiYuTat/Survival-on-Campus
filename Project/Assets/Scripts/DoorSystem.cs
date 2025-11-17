using UnityEditor.Rendering;
using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject targetDoor, existPosition;

    [SerializeField]
    private GameObject targetCamera;

    [SerializeField]
    private bool enterState;

    public void openDoor(GameObject door) 
    {
        existPosition.transform.position = targetDoor.transform.position;
        targetCamera.SetActive(enterState);
    }
}
