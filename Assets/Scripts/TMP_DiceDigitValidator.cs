using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TMP_DiceDigitValidator", menuName = "Input Field Validator")]
public class TMP_DiceDigitValidator : TMPro.TMP_InputValidator
{
    // So there is no actual explanation of the input validator in the TMP documentation, but some gigachad on reddit figured it out
    /// <summary>
    /// Override Validate method to implement your own validation
    /// </summary>
    /// <param name="text">This is a reference pointer to the actual text in the input field; changes made to this text argument will also result in changes made to text shown in the input field</param>
    /// <param name="pos">This is a reference pointer to the input field's text insertion index position (your blinking caret cursor); changing this value will also change the index of the input field's insertion position</param>
    /// <param name="ch">This is the character being typed into the input field</param>
    /// <returns>Return the character you'd allow into </returns>
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if(char.IsDigit(ch) && (char.GetNumericValue(ch) > 0 && char.GetNumericValue(ch) < 7))      // Ensure its within range of 1 - 6
        {
            if(text.Length < 6)     // less than 6 numbers, append to end
            {
                text = text.Insert(pos, ch.ToString());
                pos++;
            }
            else if(pos < 6)        // 6 digits, add to position and trim
            {
                text = text.Insert(pos, ch.ToString())[..6];
                pos++;
            }
            else                    // 6 digits and at end, add to end but remove first digit
            {
                text = text[1..].Insert(pos - 1, ch.ToString());
            }
            return ch;
        }

        return '\0';
    }
}
