using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButtonListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Text buttonText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.text = "<color=white><b><i>" + buttonText.text + "</i></b></color>";
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonText.text = buttonText.text.Replace("<i>", "").Replace("</i>", "")
            .Replace("<b>", "").Replace("</b>", "")
            .Replace("<color=white>", "").Replace("</color>", "");
    }
}
