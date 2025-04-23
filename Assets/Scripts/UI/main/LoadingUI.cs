using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public static event Action<float> scheduleLoad;
    GameObject gameStart;

    public Image schedule;
    public float loadingTime = 5f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        gameStart = GameObject.Find("PlayGame");
        schedule = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
    }
    private void Start()
    {
        schedule.fillAmount = 0;

        ObserverLoading();
    }

    private void ObserverLoading()
    {
        schedule.DOFillAmount(1f, loadingTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                //if (gameStart != null) 
                //{
                //    gameStart.SetActive(true);

                //    gameStart.GetComponent<GamePlay>()?.SetGameObject(1f);
                //}
                scheduleLoad?.Invoke(schedule.fillAmount);
            })
            .OnComplete(() => {
                Debug.Log("加载进度条图片完成");
                canvasGroup.DOFade(0, 1f)
                .SetEase(Ease.InQuad);
            });
    }
}
