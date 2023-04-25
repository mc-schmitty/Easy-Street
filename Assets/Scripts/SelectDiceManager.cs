using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiceManager : MonoBehaviour
{
    public Action<GlowSelected, GlowSelected[]> onMouseSelect;      // Gets clicks
    public static SelectDiceManager Manager;

    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private SelectDiceComponent[] diceHolders;
    [SerializeField]
    private GlowSelected[] dice;        // Contains all dice
    [SerializeField] private Rigidbody diceRbTest;
    [SerializeField] private float travelTime = 1f;

    private void Awake()
    {
        if (Manager == null)
        {
            Manager = this;
        }
        else
        {
            this.enabled = false;
            return;
        }

        onMouseSelect += MoveDice;
    }

    void Start()
    {
        onMouseSelect += FindObjectOfType<DiceGame>().DiceWasSelected;      // forcibly find the dicegame so we can shove its function into this delegate. idk why it doesnt work the other way round, and this is horrendous (but it works for now)

        foreach(SelectDiceComponent sdc in diceHolders)
        {
            sdc.SetCurve = curve;
        }

        // Pass action to each Selector
        foreach(GlowSelected gs in dice)
        {
            gs.OnMouseSelect = onMouseSelect;
        }
    }

    public void MoveTest()
    {
        if (diceHolders[0].Empty)
        {
            diceHolders[0].Insert(diceRbTest, travelTime);
        }
        else
        {
            diceHolders[0].Remove(travelTime);
        }
    }

    public void MoveDice(GlowSelected obj, GlowSelected[] linkedList)
    {
        /*Debug.Log(onMouseSelect.GetInvocationList().Length);
        foreach (Delegate d in onMouseSelect.GetInvocationList())
        {
            Debug.Log(d.Method.Name);
        }*/

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        int index = Find(rb);
        if(index < 0)   // If rigidbody not contained in components
        {
            int e = GetNextEmpty();
            if(e >= 0)      // Check shouldn't be necessary because we have exactly 6 dice and 6 holders, so there should always be space if rb isnt already inside
            {
                diceHolders[e].Insert(rb, travelTime - (float)e*0.2f);  // Extra travel time is to add slight variation
            }
        }
        else
        {
            diceHolders[index].Remove(travelTime - (float)index * 0.2f);
        }
        // pseudo-recurse for each other linked dice
        if(linkedList != null)
        {
            foreach(GlowSelected linkedGS in linkedList)
            {
                MoveDice(linkedGS, null);
            }
        }
    }

    public void FreeAllDiceHolders()
    {
        foreach(SelectDiceComponent sdci in diceHolders)
        {
            sdci.Clear();
        }
    }

    // Ok not efficient but n=6 at most so its probably fine
    // Returns index of SelectDiceComponent containing rigidbody, otherwises returns -1
    private int Find(Rigidbody rb)
    {
        for(int i = 0; i < diceHolders.Length; i++)
        {
            if(rb == diceHolders[i].SavedDice)
            {
                return i;
            }
        }
        return -1;
    }

    // Gets next empty Selector in the list (or -1 if none)
    private int GetNextEmpty()
    {
        for(int i = 0; i < diceHolders.Length; i++)
        {
            if (diceHolders[i].Empty)
                return i;
        }
        return -1;
    }
}
