using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    Text text;

    private void Start() => text = GetComponentInChildren<Text>();
    // Start is called before the first frame update
    public void Select()
    {
        text.text = "adasdasd";
    }
}
