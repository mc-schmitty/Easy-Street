using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    private TextMeshProUGUI textbar;

    private void Awake()
    {
        textbar = GetComponent<TextMeshProUGUI>();
        textbar.text = "";
    }

    public void UpdateInfoText(int score, bool streak, float delay)
    {
        if (score == 0)
        {
            StartCoroutine(UpdateDelay("Bust!", delay));
        }
        else
        {
            StartCoroutine(UpdateDelay(((streak ? "Streak! +" : "Max: +") + score), delay));
        }
    }

    public void ClearText(float delay)
    {
        StartCoroutine(UpdateDelay("", delay));
    }

    IEnumerator UpdateDelay(string message, float delay)
    {
        while(delay > 0)
        {
            yield return null;
            delay -= Time.deltaTime;
        }
        textbar.text = message;
    }
}
