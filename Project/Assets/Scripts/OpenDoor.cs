using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    public GameObject cameral;
    public GameObject classroom;
    public bool isOpen = false;
    public GameObject door;
    public DoorSystem doorSystem;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Door")
        {
            classroom = other.transform.parent.gameObject;
            cameral = classroom.transform.GetChild(0).gameObject;
            door = other.gameObject;
            doorSystem = other.gameObject.GetComponent<DoorSystem>();
            isOpen = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Door")
        {
            classroom = null;
            cameral = null;
            doorSystem = null;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isOpen == true && classroom != null)
        {
            doorSystem.openDoor(door);
        }
    }
}
