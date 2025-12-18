using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BattleUIController : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void PlayTransitionAnimation()
    {
        StartCoroutine(TransitionUIAnimation());
    }

    IEnumerator TransitionUIAnimation()
    {
        _rectTransform.DOAnchorPosY(250, 1);
        yield return new WaitForSeconds(1.2f);
        _rectTransform.DOAnchorPosY(2000, 1);
    }
}
