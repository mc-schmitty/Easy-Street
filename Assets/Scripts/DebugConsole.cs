using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField]
    private DiceGame dg;
    private TMP_InputField input;

    private void Start()
    {
        input = GetComponent<TMP_InputField>();
    }

    /// <summary>
    /// Recieves a string of numbers representing dice faces, numbers should already be between 0-6
    /// </summary>
    /// <param name="roll"></param>
    public void DebugRoll(string roll)
    {
        if (roll.Length != 6)
        {
            input.text = "";
            dg.UnsetDebugRoll();
            return;
        }

        int[] debugRoll = new int[6];
        int i = 0;
        foreach(char c in roll)
        {
            debugRoll[i] = (int)Mathf.Min((float)char.GetNumericValue(c), 6);
            i++;
        }

        dg.SetDebugRoll(debugRoll);
    }
}
