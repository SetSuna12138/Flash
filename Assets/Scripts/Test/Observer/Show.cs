using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show : MonoBehaviour
{
    private void OnEnable()
    {
        Event.action += ShowEvent;
    }

    private void OnDisable()
    {
        Event.action -= ShowEvent;
    }
    void ShowEvent()
    {
        transform.DOScale(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            //transform.DOScale(1f, 1f).SetEase(Ease.Linear);
            Debug.Log("µã»÷ÁË");
        });

        transform.GetComponent<Renderer>().material.DOColor(Color.blue, 1f);
    }
}
