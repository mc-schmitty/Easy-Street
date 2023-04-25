using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankEnabler : MonoBehaviour
{
    private int highestScore;
    [SerializeField]
    private Button rollButton;
    [SerializeField]
    private Button bankButton;
    [SerializeField]
    private DiceGame dg;

    void Awake()
    {
        highestScore = 0;
    }

    public void TryUpdate()
    {
        bool check = highestScore < dg.heldPoints;
        rollButton.interactable = check;
        bankButton.interactable = check;
    }

    public void UpdateScore()
    {
        highestScore = dg.heldPoints;
    }

    public void ResetScore()
    {
        highestScore = 0;
        StartCoroutine(RefreshButtons());
    }

    private IEnumerator RefreshButtons()
    {
        yield return null;
        rollButton.interactable = true;
        bankButton.interactable = false;
    }
}
