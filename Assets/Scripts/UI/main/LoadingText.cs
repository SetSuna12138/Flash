using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public LoadingUI image;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>(true);
        if (text == null)
            Debug.LogError("");
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        if(image == null)
        {
            image = GetComponent<LoadingUI>();
            //Debug.LogError("imageÕÒ²»µ½");
            //return;
        }
    }

    void OnEnable() => LoadingUI.scheduleLoad += SelectText;

    void OnDisable() => LoadingUI.scheduleLoad -= SelectText;

    private void Start()
    {
        float currentValue = 0f;
        DOVirtual.Float(0, 1, image.loadingTime, value =>
        {
            currentValue = value;
            image.schedule.fillAmount = value;
            text.text = Mathf.RoundToInt(value * 100) + "%";
            
        })
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            text.text = "100%";
            canvasGroup.DOFade(0, 1f)
            .SetEase(Ease.InQuad);
        });
    }

    void SelectText(float fill)
    {

    }
}
