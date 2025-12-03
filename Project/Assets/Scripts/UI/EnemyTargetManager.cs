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

    public Localization_KEY key;
    public void HighlightTarget(GameObject enemy)
    {
        // Implement highlight logic here
        var renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red; // Example highlight color
        }
    }

    public void RemoveHighlight(GameObject enemy)
    {
        // Implement remove highlight logic here
        var renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white; // Example default color
        }
    }
    public void SetTarget(GameObject enemy, int index)
    {
        source = GameObject.FindGameObjectWithTag("LocalizationSource").GetComponent<Localization_SOURCE>();
        setter = GameObject.FindGameObjectWithTag("OptionSetter").GetComponent<OptionSetter>();

        target = enemy;
        HPSlider.maxValue = enemy.GetComponent<Enemy>().GetEnemyData().maxHp;
        HPSlider.value = enemy.GetComponent<Enemy>().GetEnemyData().hp;
        targetIndex = index;

        key.keyID = "E" + enemy.GetComponent<Enemy>().GetEnemyData().id;
        source.RefreshTextElementsAndKeys();
        source.LoadLanguage(setter.getLanguageIndex());

        enemyName.text = nameBuffer.text + " #" + targetIndex.ToString();
    }

    public void SelectTarget()
    {
        var renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
        Debug.Log("Selected target index: " + targetIndex);
        BattleManager.Instance.OnTargetSelected(targetIndex);
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
