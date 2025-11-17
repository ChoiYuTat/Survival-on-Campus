using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private Text itemName, itemCount_txt;

    [SerializeField]
    private Button useButton;

    private int itemCount;

    [SerializeField]
    private LoadPlayerData player;

    public void SetItem(string name, int count, bool usable)
    {
        itemName.text = name;
        itemCount_txt.text = "x" + count.ToString();
        itemCount = count;
        if (!usable)
        {
            useButton.gameObject.SetActive(false);
        }
    }

    public void UseItem() 
    {
        UpdateItemCount(--itemCount);
    }

    public void UpdateItemCount(int count)
    {
        itemCount = count;
        itemCount_txt.text = "x" + itemCount.ToString();

        if (itemCount <= 0)
        {
            // Remove item from inventory
            for (int i = 0; i < player.data.Inventory.Count; i++)
            {
                if (player.data.Inventory[i].itemName == itemName.text)
                {
                    player.data.Inventory.RemoveAt(i);
                    break;
                }
            }
            // Destroy item UI
            Destroy(gameObject);
        }
    }
}
