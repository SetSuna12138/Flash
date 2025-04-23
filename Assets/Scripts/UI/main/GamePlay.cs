using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{

    Text gameStart;
    Sequence sequence;
    CanvasGroup canvas;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        if(canvas == null)
        {
            canvas = gameObject.AddComponent<CanvasGroup>();
        }
        canvas.alpha = 0f;
    }

    void OnEnable() => LoadingUI.scheduleLoad += SetGameObject;
    void OnDisable() => LoadingUI.scheduleLoad -= SetGameObject;

    private void Start()
    {
        gameStart = GetComponentInChildren<Text>();
        //text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        //transform.gameObject.SetActive(false);

    }
    public void SetGameObject(float fill)
    {
        //Debug.Log($"µ±Ç°µÄfill:{fill}");
        if(fill >= 1)
        {
            transform.gameObject.SetActive(true);
            //if (sequence != null || sequence.IsPlaying())
            //{
            //    return;
            //}

                sequence?.Kill();
                sequence = DOTween.Sequence();
                sequence.Append(gameStart.transform.DOScale(1.2f, 0.5f).SetEase(Ease.Linear));
                sequence.Join(canvas.DOFade(1f, 0.5f));
                sequence.Join(gameStart.DOColor(Color.yellow, 0.5f).SetEase(Ease.Linear));
                sequence.SetLoops(-1, LoopType.Yoyo);
            
        }

    }

    private void OnDestroy()
    {
        sequence?.Kill();
    }
}
