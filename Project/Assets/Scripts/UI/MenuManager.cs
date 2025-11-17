using NUnit.Framework;
using System.Collections.Generic;
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

    private List<GameObject> itemObjects = new List<GameObject>();

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        HP.text = data.HP.ToString();
        ATK.text = data.Attack.ToString();
        DEF.text = data.Defense.ToString();
        Level.text = data.Level.ToString();
        EXP.text = data.CurrentExp.ToString();
        nextEXP.text = data.RequiredExp.ToString();

        for (int i = 0; i < itemObjects.Count; i++)
        {
            Destroy(itemObjects[i]);
        }

        foreach (var item in data.Inventory)
        {
            GameObject itemObj = Instantiate(ItemPrefab, ItemView.transform);
            itemObj.GetComponent<ItemManager>().SetItem(item.itemName, item.quantity, item.isUsable);
            itemObjects.Add(itemObj);
        }
    }
}
