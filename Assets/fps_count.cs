using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fps_count : MonoBehaviour
{
    int n = 0;
    float t = 0;
    TextMeshProUGUI text;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (t>=0.3f)
        {
            text.SetText(Math.Round(n / t, 2).ToString());
            n = 0;t = 0;
        }
        t += Time.deltaTime;
        n += 1;
    }
}
