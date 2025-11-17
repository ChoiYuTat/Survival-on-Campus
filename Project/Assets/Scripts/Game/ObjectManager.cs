using UnityEngine;

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

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private int objectTypeID = 0; // 0: Item, 1: Description
    [SerializeField]
    private int objectItemID = 0;
    [SerializeField]
    private int objectID = 0;

    [SerializeField]
    private string description;

    [SerializeField]
    private LoadPlayerData player;

    private ItemDatabase itemDatabase;

    private bool isPickUp = false;

    void Start()
    {
        Invoke("loadItems", 0.1f);
        //loadItems();
    }

    private void Update()
    {
        if (isPickUp) 
        {
            isPickUp = false;
            PickUpObject(objectItemID);
        }
    }

    void loadItems() 
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("item");
        itemDatabase = JsonUtility.FromJson<ItemDatabase>(jsonFile.text);

        for (int i = 0; i < player.data.interactiveItemsID.Count; i++) 
        {
            if (player.data.interactiveItemsID[i] == objectID)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.Z)) 
            {
                if (objectTypeID == 0) 
                {
                    isPickUp = true;
                }
                
                if (objectTypeID == 1)
                {
                    Debug.Log($"ÃèÊö: {description}");
                }
            }
        }
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
