using LanguageLocalization;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private Text itemName, itemCount_txt;

    [SerializeField]
    private Button useButton;

    private int itemCount;

    private string itemNameIndex;

    private GameObject player;

    public Localization_KEY key;
    private Localization_SOURCE source;
    private OptionSetter setter;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void SetItem(string name, int count, bool usable)
    {
        source = GameObject.FindGameObjectWithTag("LocalizationSource").GetComponent<Localization_SOURCE>();
        setter = GameObject.FindGameObjectWithTag("OptionSetter").GetComponent<OptionSetter>();
        key.keyID = name;
        source.RefreshTextElementsAndKeys();
        source.LoadLanguage(setter.getLanguageIndex());
        itemNameIndex = name;
        itemCount_txt.text = "x" + count.ToString();
        itemCount = count;
        if (!usable)
        {
            useButton.gameObject.SetActive(false);
        }
    }

    public void UseItem() 
    {
        updateItemCount(--itemCount);
    }

    public void updateItemCount(int count)
    {
        itemCount = count;
        itemCount_txt.text = "x" + itemCount.ToString();

        if (itemCount <= 0)
        {
            // Remove item from inventory
            for (int i = 0; i < player.GetComponent<LoadPlayerData>().data.Inventory.Count; i++)
            {
                if (player.GetComponent<LoadPlayerData>().data.Inventory[i].itemName == itemNameIndex)
                {
                    getEffect(player.GetComponent<LoadPlayerData>().data.Inventory[i].effect);
                    player.GetComponent<LoadPlayerData>().data.Inventory.RemoveAt(i);
                    Debug.Log(player.GetComponent<LoadPlayerData>().data.Inventory);
                    break;
                }
            }
            // Destroy item UI
            Destroy(gameObject);
        }
        else 
        {
            // Update inventory item count
            for (int i = 0; i < player.GetComponent<LoadPlayerData>().data.Inventory.Count; i++)
            {
                if (player.GetComponent<LoadPlayerData>().data.Inventory[i].itemName == itemName.text)
                {
                    getEffect(player.GetComponent<LoadPlayerData>().data.Inventory[i].effect);
                    player.GetComponent<LoadPlayerData>().data.Inventory[i].quantity = itemCount;
                    break;
                }
            }
        }
    }

    void getEffect(string effect) 
    {
        switch (effect)
        {
            case "Heal10":
                player.GetComponent<LoadPlayerData>().HealPlayer(10);
                break;
            case "Heal14":
                player.GetComponent<LoadPlayerData>().HealPlayer(14);
                break;
            case "Heal35":
                player.GetComponent<LoadPlayerData>().HealPlayer(35);
                break;
            default:
                Debug.Log("No effect");
                break;
        }
    }
}
