using LanguageLocalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int itemID;
    public int quantity;
    public string description;
    public string effect;
    public bool isUsable;
}

[System.Serializable]
public class ItemDatabase
{
    public ItemData[] item;
}

public enum ObjectType
{
    Item,
    Description,
    LockedDoor
}

public enum LockType
{
    None,
    OpenClassroom,
    OpenPrincipalroom
}

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private ObjectType objectType;
    [SerializeField] 
    private LockType lockType;

    [SerializeField]
    private int objectItemID = 0;
    [SerializeField]
    private int objectID = 0;

    [SerializeField]
    private string description;

    public PlayableDirector director;
    public DialogueController dialogueController;
    public TimelineAsset pickUpTimeline;

    [SerializeField]
    private LoadPlayerData player;

    private ItemDatabase itemDatabase;

    private bool isPickUp = false;
    private bool isOpened = false;

    void Start()
    {
        Invoke("loadItems", 0.01f);
        //loadItems();

        if (lockType == LockType.None) 
        {
            isOpened = true;
        }
    }

    private void Update()
    {
        if (objectType == ObjectType.Item && isPickUp)
        {
            isPickUp = false;
            PickUpObject(objectItemID);
        }

        if (objectType == ObjectType.LockedDoor && isOpened)
        {
            isOpened = false;
            OpenDoor();
        }
    }

    void loadItems()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("item");
        itemDatabase = JsonUtility.FromJson<ItemDatabase>(jsonFile.text);

        if (objectType == ObjectType.Item)
        {
            for (int i = 0; i < player.data.interactiveItemsID.Count; i++)
            {
                if (player.data.interactiveItemsID[i] == objectID)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }

        if (objectType == ObjectType.LockedDoor) 
        {
            for (int i = 0; i < player.data.interactiveItemsID.Count; i++)
            {
                if (player.data.interactiveItemsID[i] == objectID)
                {
                    isOpened = true;
                }
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                switch (objectType)
                {
                    case ObjectType.Item:
                        director.SetGenericBinding(pickUpTimeline.GetOutputTrack(0), GetComponent<SignalReceiver>());
                        director.enabled = true;
                        dialogueController.director = director;
                        director.Play();
                        break;
                    case ObjectType.Description:
                        Debug.Log(description);
                        break;
                    case ObjectType.LockedDoor:
                        checkLocked();
                        break;
                }
            }
        }
    }

    void checkLocked() 
    {
        if (lockType != LockType.None)
        {
            bool hasKey = false;
            for (int i = 0; i < player.data.Inventory.Count; i++)
            {
                if ((lockType == LockType.OpenClassroom && player.data.Inventory[i].effect == "OpenClassroom") ||
                    (lockType == LockType.OpenPrincipalroom && player.data.Inventory[i].effect == "OpenPrincipalroom"))
                {
                    hasKey = true;
                    break;
                }
            }
            if (hasKey)
            {
                isOpened = true;
                director.SetGenericBinding(pickUpTimeline.GetOutputTrack(0),
                    transform.GetChild(2).gameObject.GetComponent<SignalReceiver>());
                director.enabled = true;
                dialogueController.director = director;
                director.Play();
            }
            else 
            {
                director.SetGenericBinding(pickUpTimeline.GetOutputTrack(0), 
                    transform.GetChild(1).gameObject.GetComponent<SignalReceiver>());
                director.enabled = true;
                dialogueController.director = director;
                director.Play();
            }
        }
    }

    public void SetPickUpTrue()
    {
        director.enabled = false;
        isPickUp = true;
    }

    public void SetDoorOpen() 
    {
        isOpened = true;
    }

    void OpenDoor() 
    {
        GameObject door = transform.GetChild(0).gameObject;
        Debug.Log(door);
        door.SetActive(true);

        GetComponent<SphereCollider>().enabled = false;
    }

    void PickUpObject(int id)
    {
        for (int i = 0; i < itemDatabase.item.Length; i++)
        {
            if (itemDatabase.item[i].itemID == objectItemID)
            {
                if (IsItemInInventory(id))
                {
                    for (int x = 0; x < player.data.Inventory.Count; x++)
                    {
                        if (player.data.Inventory[x].itemID == id)
                        {
                            player.data.Inventory[x].quantity++;
                            break;
                        }
                    }
                }
                else 
                {
                    player.data.Inventory.Add(new PlayerInvertory
                    {
                        itemID = itemDatabase.item[i].itemID,
                        itemName = itemDatabase.item[i].itemName,
                        quantity = itemDatabase.item[i].quantity,
                        effect = itemDatabase.item[i].effect,
                        description = itemDatabase.item[i].description,
                        isUsable = itemDatabase.item[i].isUsable,
                    });
                }

                player.data.interactiveItemsID.Add(objectID);
                Destroy(gameObject);
                return;
            }
        }
    }

    bool IsItemInInventory(int id)
    {
        for (int x = 0; x < player.data.Inventory.Count; x++)
        {
            if (player.data.Inventory[x].itemID == id)
            {
                return true;
            }
        }
        return false;
    }
}
