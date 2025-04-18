using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public TextMeshProUGUI num;
    public Slider slider;

    public GameObject start;
    public GameObject end;
    public GameObject loading;

    public void Awake()
    {
        num = transform.Find("Slider/Num").GetComponent<TextMeshProUGUI>();
        start = transform.Find("GameStart").gameObject;
        loading = transform.Find("LoadGame").gameObject;
        end = transform.Find("EndGame").gameObject;

        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.maxValue = 100;
        slider.minValue = 0;
        slider.value = 0;
    }

    private IEnumerator Start()
    {
        slider.gameObject.SetActive(true);
        start.SetActive(false);
        end.SetActive(false);
        loading.SetActive(false);
        slider.onValueChanged.AddListener(delegate { ChangeSlider(); });
        yield return new WaitUntil(() => slider.value == 100);

        slider.gameObject.SetActive(false);
        start.SetActive(true);
        end.SetActive(true);
        loading.SetActive(true);

    }


    private void Update()
    {
        slider.value += Time.deltaTime * 50;
        
    }

    private void ChangeSlider()
    {
        num.text = $"{Mathf.RoundToInt(slider.value)}%";
    }
}
