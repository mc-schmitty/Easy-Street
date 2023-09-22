using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    [SerializeField]
    private Image loadingPanel;
    [SerializeField]
    private TextMeshProUGUI loadingtext;
    [SerializeField]
    private TextMeshProUGUI iterationtext;
    [SerializeField]
    private DiceRollManager drm;
    [SerializeField][Range(1, 6)]
    private int maxChecks = 5;
    [SerializeField][Min(0.1f)]
    private float fadeOutTime = 1f;
    [SerializeField][Min(0.01f)]
    private float blinkInterval = 0.6f;
    private int checks;
    private bool finishedRoll;
    

    private void Start()
    {
#if !UNITY_WEBGL
        gameObject.SetActive(false);
        return;
#endif
        DiceRollManager.OnDiceRollFinished += DoTestRoll;
        StartCoroutine(LoadingSequence());
        StartCoroutine(LoadingText());
    }

    void DoTestRoll(Rigidbody dice)
    {
        finishedRoll = true;
    }

    IEnumerator LoadingSequence()
    {
        checks = 1;
        finishedRoll = true;
        iterationtext.text = $"Iteration: 0/{maxChecks}";       // update text display
        yield return null;          // Setup delay
        while(checks <= maxChecks)       // Do this max checks number of times
        {
            if (finishedRoll)       // Wait until roll is finished
            {
                yield return new WaitForFixedUpdate();       // wait until dice have settled  
                drm.StartCRoll(checks);     // Start a roll with check num
                iterationtext.text = $"Iteration: {checks}/{maxChecks}";    // update it count
                checks++;
                finishedRoll = false;
            }
            yield return null;
        }

        while (!finishedRoll)   // Waiting for last queued roll to finish
            yield return null;

        iterationtext.text = "Success :)";          // note success
        yield return new WaitForFixedUpdate();      // Wait for any position setting stuff to end
        drm.ResetAllDicePosition();                 // Before setting position works?
        StartCoroutine(RemoveLoadingScreen(fadeOutTime));
    }

    IEnumerator LoadingText()
    {
        string loadText = "Loading";
        int dots = 0;
        loadingtext.text = loadText;

        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            if(dots >= 3)
            {
                loadText = "Loading";
                dots = 0;
            }
            else
            {
                loadText += " .";
                dots++;
            }
            loadingtext.text = loadText;
        }
    }

    IEnumerator RemoveLoadingScreen(float timeToRemove)
    {
        float time = 0;
        float alpha;
        while(time <= timeToRemove)
        {
            alpha = Mathf.InverseLerp(timeToRemove, 0, time);
            loadingPanel.color = new Color(loadingPanel.color.r, loadingPanel.color.g, loadingPanel.color.b, alpha);
            loadingtext.color = new Color(loadingtext.color.r, loadingtext.color.g, loadingtext.color.b, alpha);
            iterationtext.color = new Color(iterationtext.color.r, iterationtext.color.g, iterationtext.color.b, alpha);        // One day I'll figure out a better way of editing alpha
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
