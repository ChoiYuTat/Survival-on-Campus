using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeFont : MonoBehaviour
{
    [SerializeField]
    private List<TMP_Text> target_texts = new List<TMP_Text>();
    [SerializeField]
    private TMP_FontAsset EN, CN;

    void Start()
    {
        updateENFont();
    }

    public void updateENFont()
    {
        foreach (var text in target_texts)
        {
            text.font = EN;
        }
    }

    public void updateCNFont()
    {
        foreach (var text in target_texts)
        {
            text.font = CN;
        }
    }
}
