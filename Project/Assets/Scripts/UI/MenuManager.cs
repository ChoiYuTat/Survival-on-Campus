using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private LoadPlayerData playerData;

    [SerializeField]
    private Text HP, ATK, DEF, EXP, nextEXP, Level;

    [SerializeField]
    private GameObject ItemView, battleItemView, ItemPrefab, battleItemPrefab, player;

    private List<GameObject> itemObjects = new List<GameObject>();
    private GameObject[] enemies;

    void Start()
    {
        UpdateUI();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void UpdateUI()
    {
        HP.text = playerData.data.HP.ToString() + " / " + playerData.data.MaxHP.ToString();
        ATK.text = playerData.data.Attack.ToString();
        DEF.text = playerData.data.Defense.ToString();
        Level.text = playerData.data.Level.ToString();
        EXP.text = playerData.data.CurrentExp.ToString();
        nextEXP.text = playerData.data.RequiredExp.ToString();

        for (int i = 0; i < itemObjects.Count; i++)
        {
            Destroy(itemObjects[i]);
        }

        foreach (var item in playerData.data.Inventory)
        {
            GameObject itemObj = Instantiate(ItemPrefab, ItemView.transform);
            itemObj.GetComponent<ItemManager>().SetItem(item.itemName, item.quantity, item.isUsable);
            itemObjects.Add(itemObj);
        }
    }

    public void UpdateBattleUI() 
    {
        for (int i = 0; i < itemObjects.Count; i++)
        {
            Destroy(itemObjects[i]);
        }

        foreach (var item in playerData.data.Inventory)
        {
            GameObject itemObj = Instantiate(battleItemPrefab, battleItemView.transform);
            itemObj.GetComponent<ItemManager>().SetItem(item.itemName, item.quantity, item.isUsable);
            itemObjects.Add(itemObj);
        }
    }

    public void Freeze() 
    {
        player.GetComponent<luna>().enabled = false;
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

    public void Unfreeze()
    {
        player.GetComponent<luna>().enabled = true;
        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
        }
    }
}
