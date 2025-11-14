using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButtonListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI buttonText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.text = $"<u>{buttonText.text}</u>";
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonText.text = buttonText.text.Replace("<u>", "").Replace("</u>", "");
    }
}
