using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    public GameObject cameral;
    public GameObject classroom;
    public bool isOpen = false;
    public GameObject door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Door")
        {
            classroom = other.transform.parent.gameObject;
            cameral = classroom.transform.GetChild(0).gameObject;
            door = other.gameObject;
            isOpen = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Door")
        {
            classroom = null;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isOpen == true)
        {
            switch (door.name)
            {
                case "01":
                    transform.position = classroom.transform.Find("10").position;
                    cameral.SetActive(false);
                    break;
                case "02":
                    transform.position = classroom.transform.Find("20").position;
                    cameral.SetActive(false);
                    break;
                case "10":
                    transform.position = classroom.transform.Find("01").position;
                    cameral.SetActive(true);
                    break;
                case "20":
                    transform.position = classroom.transform.Find("02").position;
                    cameral.SetActive(true);
                    break;
            }
        }

    }
}
