using LanguageLocalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyTargetManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject target;
    private int targetIndex;

    [SerializeField]
    private Slider HPSlider;
    [SerializeField]
    private Text enemyName, nameBuffer;

    private Localization_SOURCE source;
    private OptionSetter setter;
    private BattleManager battleManager;

    public Localization_KEY key;
    public void HighlightTarget(GameObject enemy)
    {
        // Implement highlight logic here
        var renderer = enemy.GetComponent<Outline>();
        renderer.GetComponent<Outline>().enabled = true;
    }

    public void RemoveHighlight(GameObject enemy)
    {
        // Implement remove highlight logic here
        var renderer = enemy.GetComponent<Renderer>();
        renderer.GetComponent<Outline>().enabled = false;
    }
    public void SetTarget(GameObject enemy, int index, BattleManager ins)
    {

        battleManager = ins;
        target = enemy;
        HPSlider.maxValue = enemy.GetComponent<Enemy>().GetEnemyData().maxHp;
        HPSlider.value = enemy.GetComponent<Enemy>().GetEnemyData().hp;
        targetIndex = index;

        key.keyID = "E" + enemy.GetComponent<Enemy>().GetEnemyData().id;

        Invoke("RefreshText", 0.01f);

    }

    void RefreshText() 
    {
        source = GameObject.FindGameObjectWithTag("LocalizationSource").GetComponent<Localization_SOURCE>();
        setter = GameObject.FindGameObjectWithTag("OptionSetter").GetComponent<OptionSetter>();

        source.RefreshTextElementsAndKeys();
        source.LoadLanguage(setter.getLanguageIndex());

        enemyName.text = nameBuffer.text + " #" + targetIndex.ToString();
    }

    public void SelectTarget()
    {
        var renderer = target.GetComponent<Renderer>();
        renderer.GetComponent<Outline>().enabled = false;
        Debug.Log("Selected target index: " + targetIndex);
        battleManager.OnTargetSelected(targetIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightTarget(target);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveHighlight(target);
    }
}
