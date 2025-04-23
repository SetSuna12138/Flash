using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AudioStop : MonoBehaviour
{
    LoadingVideoPlayer player;
    Text text;
    public void Awake()
    {
        player = GetComponent<LoadingVideoPlayer>();

        text = GetComponentInChildren<Text>();
    }

    public void StopAudio()
    {
        Debug.Log("�����");
        if (player.videoPlayer.audioTrackCount > 0) 
        {
            player.videoPlayer.SetDirectAudioVolume(0, 0f);
            text.text = "ȡ������";
            
        }
        else
        {
            player.videoPlayer.SetDirectAudioVolume(0, 1f);
            text.text = "����";
        }
    }

}
