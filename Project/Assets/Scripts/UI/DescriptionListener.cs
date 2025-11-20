using LanguageLocalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class DescriptionListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private ItemManager itemManager;
    private GameObject player;

    private Localization_KEY des_key;
    private Localization_SOURCE source;
    private OptionSetter setter;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        des_key = GameObject.FindGameObjectWithTag("Description").GetComponent<Localization_KEY>();
        source = GameObject.FindGameObjectWithTag("LocalizationSource").GetComponent<Localization_SOURCE>();
        setter = GameObject.FindGameObjectWithTag("OptionSetter").GetComponent<OptionSetter>();

        for (int i = 0; i < player.GetComponent<LoadPlayerData>().data.Inventory.Count; i++)
        {
            if (player.GetComponent<LoadPlayerData>().data.Inventory[i].itemName == itemManager.getItemName())
            {
                Debug.Log("Enter");
                des_key.keyID = "I" + player.GetComponent<LoadPlayerData>().data.Inventory[i].itemID;
                source.RefreshTextElementsAndKeys();
                source.LoadLanguage(setter.getLanguageIndex());
                break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("Description").GetComponent<Text>().text = "";
        des_key = GameObject.FindGameObjectWithTag("Description").GetComponent<Localization_KEY>();
        des_key.keyID = "";
    }
}
