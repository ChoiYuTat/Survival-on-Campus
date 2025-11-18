using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private LoadPlayerData player;

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
        HP.text = player.data.HP.ToString() + " / " + player.data.MaxHP.ToString();
        ATK.text = player.data.Attack.ToString();
        DEF.text = player.data.Defense.ToString();
        Level.text = player.data.Level.ToString();
        EXP.text = player.data.CurrentExp.ToString();
        nextEXP.text = player.data.RequiredExp.ToString();

        for (int i = 0; i < itemObjects.Count; i++)
        {
            Destroy(itemObjects[i]);
        }

        foreach (var item in player.data.Inventory)
        {
            GameObject itemObj = Instantiate(ItemPrefab, ItemView.transform);
            itemObj.GetComponent<ItemManager>().SetItem(item.itemName, item.quantity, item.isUsable);
            itemObjects.Add(itemObj);
        }
    }
}
