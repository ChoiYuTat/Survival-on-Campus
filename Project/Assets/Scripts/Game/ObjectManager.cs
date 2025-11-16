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
    private string description;

    [SerializeField]
    private PlayerData data;

    private ItemDatabase itemDatabase;

    private bool isPickUp = false;

    void Start()
    {
        loadItems();
    }

    private void Update()
    {
        if (isPickUp) 
        {
            isPickUp = false;
            PickUpObject();
        }
    }

    void loadItems() 
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("item");
        itemDatabase = JsonUtility.FromJson<ItemDatabase>(jsonFile.text);
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
                
            }
        }
    }

    void PickUpObject()
    {
        for (int i = 0; i < itemDatabase.item.Length; i++)
        {
            if (itemDatabase.item[i].itemID == objectItemID)
            {
                data.Inventory.Add(new PlayerInvertory
                {
                    itemID = itemDatabase.item[i].itemID,
                    itemName = itemDatabase.item[i].itemName,
                    quantity = itemDatabase.item[i].quantity,
                    effect = itemDatabase.item[i].effect,
                    description = itemDatabase.item[i].description,
                    isUsable = itemDatabase.item[i].isUsable
                });

                Debug.Log($"ÒÑÊ°È¡: {itemDatabase.item[i].itemName}");
                Destroy(gameObject);
                return;
            }
        }
    }
}
