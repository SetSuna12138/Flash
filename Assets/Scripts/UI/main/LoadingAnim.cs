using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingAnim : MonoBehaviour
{
    void OnEnable() => LoadingUI.scheduleLoad += SelctScence;
    void OnDisable() => LoadingUI.scheduleLoad -= SelctScence;


    void SelctScence(float fill)
    {
        if (Input.GetMouseButtonDown(0) && fill >= 1) 
        {
            LoadNextScene();
        }

    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("CharSelect");
    }

    //IEnumerator SelctScence(float fill)
    //{

    //}
}
