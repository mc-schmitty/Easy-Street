using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI totalScore;
    [SerializeField]
    private TextMeshProUGUI heldScore;
    [SerializeField]
    private DiceGame dg;

    // Update is called once per frame
    void Update()
    {
        totalScore.text = dg.totalPoints.ToString();
        heldScore.text = dg.heldPoints.ToString();
    }
}
