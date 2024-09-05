using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpPanelController : MonoBehaviour
{
    Image mPanel;
    CanvasGroup popUpGroup;
    [SerializeField] TextMeshProUGUI titleText;
    
    void Awake()
    {
        mPanel = GetComponent<Image>();
        popUpGroup = GetComponentInChildren<CanvasGroup>();

        mPanel.enabled = false;
        popUpGroup.alpha = 0;
    }

    public void ShowPanel(string title)
    {
        titleText.text = title;
        StartCoroutine(AnimatePanelOn());
    }

    IEnumerator AnimatePanelOn()
    {
        const float Sequence1_duration = 1.2f;
        const float Sequence2_duration = 0.8f;
        const float Sequence2_delay = 0.3f;
        const float mPanelFinalAlpha = 0.8f;

        mPanel.enabled = true;
        float elapsedTime = 0;

        while (elapsedTime < Mathf.Max(Sequence1_duration, Sequence2_duration+Sequence2_delay))
        {
            float t1 = Mathf.Clamp01(elapsedTime / Sequence1_duration);
            mPanel.color = new Color(0, 0, 0, t1* mPanelFinalAlpha);

            float t2 = Mathf.Clamp01((elapsedTime- Sequence2_delay) / Sequence2_duration);
            popUpGroup.alpha = t2;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mPanel.color = new Color(0, 0, 0, mPanelFinalAlpha);
        popUpGroup.alpha = 1;
    }

}
