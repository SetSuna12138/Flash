using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    //private CanvasGroup canvasGroup;

    public float fadeDuration = 1f;
    public bool playOnStart = true;
    public static LoadingVideoPlayer instance;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();
        //if(canvasGroup == null)
        //{
        //    canvasGroup = GetComponent<CanvasGroup>();
        //}
        //canvasGroup.alpha = 1;

    }
    void Start()
    {
        videoPlayer.isLooping = true;

        if(playOnStart)
        {

        }
    }


}
