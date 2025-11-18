using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private Text itemName, itemCount_txt;

    [SerializeField]
    private Button useButton;

    private int itemCount;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
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
                if (player.GetComponent<LoadPlayerData>().data.Inventory[i].itemName == itemName.text)
                {
                    getEffect(player.GetComponent<LoadPlayerData>().data.Inventory[i].effect);
                    player.GetComponent<LoadPlayerData>().data.Inventory.RemoveAt(i);
                    break;
                }
            }
            // Destroy item UI
            Destroy(gameObject);
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
