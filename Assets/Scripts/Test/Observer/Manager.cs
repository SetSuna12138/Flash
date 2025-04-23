using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FillProgress());
    }

    IEnumerator FillProgress()
    {
        //while (true)
        //{
        //    if (Input.GetButtonDown("Jump"))
        //    { 
        //        OnClick();
        //        break;
        //    }
        //}


        //yield return null;

        //Event.action?.Invoke();

        while(!Input.GetButtonDown("Jump"))
        {
            yield return null;
        }

        OnClick();

        yield return new WaitForSeconds(1f);

        Event.action?.Invoke();
    }

    public void OnClick()
    {
        this.transform.GetComponent<Renderer>()?.material.DOColor(Color.red, 1f);
    }
}
