using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData data;

    [SerializeField]
    private Text HP, ATK, DEF, EXP, nextEXP, Level;

    [SerializeField]
    private GameObject ItemView, ItemPrefab;

    void Start()
    {
        data.Inventory.Add(new PlayerInvertory { itemID = 1, itemName = "Health Potion", quantity = 5, effect = "AddHP20", isUsable = true});
        data.Inventory.Add(new PlayerInvertory { itemID = 2, itemName = "Key", quantity = 1, isUsable = false});

        UpdateUI();

        foreach (var item in data.Inventory)
        {
            GameObject itemObj = Instantiate(ItemPrefab, ItemView.transform);
            itemObj.GetComponent<ItemManager>().SetItem(item.itemName, item.quantity, item.isUsable);
        }
    }

    public void UpdateUI()
    {
        HP.text = data.HP.ToString();
        ATK.text = data.Attack.ToString();
        DEF.text = data.Defense.ToString();
        Level.text = data.Level.ToString();
        EXP.text = data.CurrentExp.ToString();
        nextEXP.text = data.RequiredExp.ToString();
    }
}
