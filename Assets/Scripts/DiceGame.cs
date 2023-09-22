using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
struct Dice     // Yeah I know the singular of "dice" is "die" but die kinda references other things ok?
{
    public int result;
    public GameObject diceObj;

    public int Roll
    {
        get
        {
            result = UnityEngine.Random.Range(1, 7);
            return result;
        }

        
    }
}*/

// Originally i wanted to try using structs, but its probably better to make these Dice objects
public class Dice
{
    public int result;
    public GameObject diceObj;

    public void Roll()
    {
        result = UnityEngine.Random.Range(1, 7);
    }
}

public class DiceGame : MonoBehaviour
{
    public int totalPoints;
    public int heldPoints;
    public int round;

    public const int OneScore = 100;        // Score for individual ones
    public const int FiveScore = 50;        // Score for individual fives
    public const int OneMult = 10;          // Score multiplier for triples or more of one
    public const int TwoMult = 2;           // Score multiplier for triples or more of two
    public const int ThreeMult = 3;         // Score multipleir for triples ir more of three
    public const int FourMult = 4;          // Score multipleir for triples or more of four
    public const int FiveMult = 5;          // Score multipler fro triples or more of five
    public const int SixMult = 6;           // Score multipleier for triples o r more of six
    public const int TriplesMult = 100;     // Score multiplier for triples
    public const int QuadsMult = 200;       // Score multiplier for quadruples
    public const int QuintsMult = 300;      // Score multiplier for quintuples
    public const int SexMult = 400;        // Score multiplier for sextuples
    public const int StraightValue = 4000;  // Score for a straight

    private Dice[] diceList;
    private List<Dice> activeDice;
    private List<Dice> heldDice;
    private Dictionary<int, int> diceToDiceMap;    // Get our other dice from object dice, GameObject id key, Dice index value
    private bool isWaitingForUser;
    private bool doDebugRoll;
    private int[] debugRoll;

    private DiceRollManager drm;
    // UI Elements and Canvas stuff
    [SerializeField]
    private BankEnabler bankButton;
    [SerializeField]
    private InfoText infoText;

    void Start()
    {
        // init values and containters
        totalPoints = 0;
        heldPoints = 0;
        round = 1;
        diceList = new Dice[6];
        activeDice = new List<Dice>();
        heldDice = new List<Dice>();
        diceToDiceMap = new Dictionary<int, int>();    // GameObject id key, Dice index value
        isWaitingForUser = false;

        doDebugRoll = false;                       // Concerning setting rolls for debug

        drm = GetComponent<DiceRollManager>();      // temp, ill prob do smth with static stuff eventually

        // initialize dice, and link to real dice
        GameObject[] realDice = drm.GetDice();
        for (int i = 0; i < 6; i++)
        {
            diceList[i] = new Dice();
            activeDice.Add(diceList[i]);
            diceList[i].diceObj = realDice[i];
            diceToDiceMap.Add(realDice[i].GetInstanceID(), i);
        }

        //onMouseSelect = SelectDiceManager.Manager.onMouseSelect;
        //onMouseSelect += this.DiceWasSelected;         // Now we also are listening for dice click events
        //Debug.Log(onMouseSelect.GetInvocationList().Length);

        DiceRollManager.OnDiceRollFinished += diceRB => diceRB.GetComponent<GlowSelected>().UpdateDull();       // Whenever a dice is finished rolling, update dull (maybe move this to GlowSelected)
    }

    public void Roll()
    {
        foreach(Dice d in activeDice)
        {
            //_ = d.Roll;
            d.Roll();
        }
    }

    public void DebugRoll()
    {
        List<int> list = new List<int>(debugRoll);
        foreach(Dice d in activeDice)
        {
            d.result = list[0];
            list.RemoveAt(0);
        }
    }

    public void SetDebugRoll(int[] numbers)
    {
        doDebugRoll = true;
        debugRoll = numbers;
    }

    public void UnsetDebugRoll()
    {
        doDebugRoll = false;
    }

    public (int,int) DetermineActiveScore()
    {
        int activeScore = 0;
        bool straightFlag = true;

        // assign dice to buckets (uh i forgor how to do buckets properly i think u use a heap whatever)
        List<Dice>[] buckets = { new List<Dice>(), new List<Dice>(), new List<Dice>(), new List<Dice>(), new List<Dice>(), new List<Dice>() };   // empy 
        foreach(Dice d in activeDice)
        {
            buckets[d.result - 1].Add(d);    // add to buckt
        }

        // Now score dice
        for(int i = 0; i < 6; i++)
        {
            straightFlag = straightFlag && buckets[i].Count == 1;         // Fails if ever more or less than 1 die in each bucket
            activeScore += ScoreMachine(i+1, buckets[i].Count);             // Scores dice result
        }

        // For visual flair, link together dice
        int affected = LinkDice(buckets, straightFlag);

        if (straightFlag)
            activeScore = StraightValue;
        return (activeScore, affected);
    }

    public void GameRound()
    {
        // 0. Setup if we are rerolling!
        if (isWaitingForUser)
        {
            // Make old held dice unselectable
            if(activeDice.Count == 0)
            {
                foreach(Dice d in heldDice)
                {
                    activeDice.Add(d);
                }
                heldDice.Clear();
                SelectDiceManager.Manager.FreeAllDiceHolders();
            }
            else
            {
                foreach (Dice d in heldDice)
                {
                    GlowSelected gs = d.diceObj.GetComponent<GlowSelected>();
                    gs.enabled = false;
                    gs.UpdateDull();
                }
            }
            // Make all active dice selectable (might want to move this into DiceRollManager later
            foreach(Dice d in activeDice)
            {
                GlowSelected gs = d.diceObj.GetComponent<GlowSelected>();
                gs.enabled = true;
                gs.UnlinkAll();
            }
            isWaitingForUser = false;
        }
        else // new round, make all dice interactable again
        {
            foreach (Dice d in activeDice)
            {
                d.diceObj.GetComponent<GlowSelected>().enabled = true;
            }
        }
        infoText.ClearText(0);

        // 1. Roll active dice
        if (doDebugRoll)
            DebugRoll();
        else
            Roll();
        PrintRoll();
        int[] temp = new int[6];
        for(int i = 0; i < 6; i++)
        {
            temp[i] = activeDice.Contains(diceList[i]) ? diceList[i].result : 0;    // gets active dice into list so dicerollmanager can roll it
        }
        drm.StartCRoll(temp);


        // 2. Score dice, find pairs etc
        (int, int) pair = DetermineActiveScore();
        int maxScore = pair.Item1;
        bool isStreak = activeDice.Count == pair.Item2;
        Debug.Log("Score: " + maxScore);
        infoText.UpdateInfoText(maxScore, isStreak, 2.5f);        // Update onscreen text with max value
        if (maxScore == 0)
        {
            heldPoints = 0;     // No possible score, big fail :(
            isWaitingForUser = true;
            EndRound();
            return;     // Round over
        }
        // Otherwise, can still save result or reroll
        bankButton.UpdateScore();           // Set to new held result
        // 3. Let user choose pairs, bank/ reroll (go to 1)
        isWaitingForUser = true;

    }

    public void EndRound()      // user clicked end round, keeping all points and moving to next round
    {
        if (!isWaitingForUser)       // Dont want to call if waiting is false
            return;
        isWaitingForUser = false;

        totalPoints += heldPoints;
        foreach(Dice d in heldDice)     // clear held dice, active dice becomes full
        {
            activeDice.Add(d);
        }
        heldDice.Clear();
        if(heldPoints > 0)
            infoText.ClearText(1);                   // Clear max score text
        heldPoints = 0;

        foreach(Dice d in diceList)     // Remove glowers from dice, make them uninteractible
        {
            var gs = d.diceObj.GetComponent<GlowSelected>();
            gs.UnlinkAll();
            gs.enabled = false;
            //gs.StopDull();         
        }

        SelectDiceManager.Manager.FreeAllDiceHolders();     // Empty the holder
        //drm.ResetAllDicePosition();                         // Remove dice from screen
        bankButton.ResetScore();                            // Enable buttons
        round++;                                            // increment round counter
    }

    // User is clicking dice, add or remove from list
    public void DiceWasSelected(GlowSelected obj, GlowSelected[] linkedList)
    {
        if (!isWaitingForUser)
            return;

        Dice mainDie = diceList[diceToDiceMap[obj.gameObject.GetInstanceID()]];
        //Debug.Log("Dice value should be: " + mainDie.result);
        // Find if dice in active dice and move to held, or vice reversa
        if (activeDice.Contains(mainDie))
        {
            // ok so big assumtion, but all the other linked dice must also be active as well
            int score;
            
            if (IsStraight())
            {
                score = StraightValue;
            }
            else
            {
                score = ScoreMachine(mainDie.result, 1 + (linkedList != null ? linkedList.Length : 0));
            }

            heldPoints += score;            // Update held score
            activeDice.Remove(mainDie);    // move dice to list
            heldDice.Add(mainDie);
            if(linkedList != null)          // move the rest as well
            {
                foreach(GlowSelected gs in linkedList)
                {
                    Dice d = diceList[diceToDiceMap[gs.gameObject.GetInstanceID()]];
                    activeDice.Remove(d);
                    heldDice.Add(d);
                }
            }

        }
        // do the same thing but in reverse (kinda hate how big this becomes + repeated code + L + ratio)
        else if (heldDice.Contains(mainDie))
        {
            int score;
            if (IsStraight())
                score = StraightValue;
            else
                score = ScoreMachine(mainDie.result, 1 + (linkedList != null ? linkedList.Length : 0));

            heldPoints -= score;            // Update held score
            activeDice.Add(mainDie);    // move dice to list
            heldDice.Remove(mainDie);
            if (linkedList != null)          // move the rest as well
            {
                foreach (GlowSelected gs in linkedList)
                {
                    Dice d = diceList[diceToDiceMap[gs.gameObject.GetInstanceID()]];
                    activeDice.Add(d);
                    heldDice.Remove(d);
                }
            }
        }

        bankButton.TryUpdate();         // Update buttons, enable if more score held than initial
    }

    private void PrintRoll()    // Print result in active dice
    {
        String p = "Rolled: ( ";
        foreach(Dice d in activeDice)
        {
            p = p + d.result;
            p += " ";
        }
        Debug.Log(p + ").");
    }

    private int ScoreMachine(int value, int amount)
    {
        if (amount == 0)     // Simple case outta da way
            return 0;

        int diceValue = value; // + 1; yeah we increment the list in the actual list section now
        // Scoring for under 3 dice
        if(amount < 3)
        {
            switch(diceValue){
                case 1:
                    return OneScore * amount;
                case 5:
                    return FiveScore * amount;
            }
        }
        else        // we gamin: triples or above, babye
        {
            int valueMult = 0;
            int amountMult = 0;

            switch (diceValue)      // This kinda looks horrific idk if theres a better way, just sets 2 = 2 lol
            {
                case 1:
                    valueMult = OneMult;
                    break;
                case 2:
                    valueMult = TwoMult;
                    break;
                case 3:
                    valueMult = ThreeMult;
                    break;
                case 4:
                    valueMult = FourMult;
                    break;
                case 5:
                    valueMult = FiveMult;
                    break;
                case 6:
                    valueMult = SixMult;
                    break;
                default:
                    valueMult = 1;
                    break;
            }

            switch (amount)     // score for the amount now
            {
                case 3:
                    amountMult = TriplesMult;
                    break;
                case 4:
                    amountMult = QuadsMult;
                    break;
                case 5:
                    amountMult = QuintsMult;
                    break;
                case 6:
                    amountMult = SexMult;
                    break;
                default:
                    amountMult = 0;
                    break;
            }

            return valueMult * amountMult;
        }

        return 0;
    }

    /// <summary>
    /// Straight defined by sequence 1, 2, 3, 4, 5, 6. 
    /// </summary>
    /// <returns></returns>
    private bool IsStraight()
    {
        bool heldCount = heldDice.Count == 6;
        if (activeDice.Count == 6 || heldCount)
        {
            int[] str = { 0, 0, 0, 0, 0, 0 };
            foreach (Dice d in diceList)
            {
                // Kind of an annoying test case: since held dice persist, need to check if each die is interactable (so actual straight and not built over multiple rolls)
                if (heldCount && !d.diceObj.GetComponent<GlowSelected>().enabled)     
                    return false;
                if (str[d.result - 1] > 0)      // if duplicate dice exists
                    return false;
                str[d.result - 1]++;

            }
            return true;        // Passes all test cases, so straight
        }

        return false;       // all 6 dice must be in same list
    }

    /// <summary>
    /// Links Dice together with GlowSelected Components. Returns number of dice affecting score (not number of dice affected by linking)
    /// </summary>
    /// <param name="buckets"></param>
    /// <param name="isStraight"></param>
    /// <returns></returns>
    private int LinkDice(List<Dice>[] buckets, bool isStraight)
    {
        if (isStraight)
        {
            // link every dice together because all 6 are involved
            GlowSelected[] gsList = new GlowSelected[buckets.Length];
            for (int i = 0; i < buckets.Length; i++)
            {
                // get all Glow Selected in an array (straight so only one per bucket)
                gsList[i] = buckets[i][0].diceObj.GetComponent<GlowSelected>();
            }

            // link them together, i explain why this approach kinda sucks in a comment below
            foreach(GlowSelected gsi in gsList)
            {
                foreach(GlowSelected gsj in gsList)
                {
                    if (!gsi.Equals(gsj))
                    {
                        gsi.LinkGlower(gsj);
                    }
                }
            }
            return activeDice.Count;
        }

        int affected = activeDice.Count;
        for(int i = 0; i < buckets.Length; i++)
        {
            if (buckets[i].Count < 3 && i != 0 && i != 4)       // not 1 or 5, and under 3 = not selectable
            {
                foreach(Dice d in buckets[i])
                {
                    d.diceObj.GetComponent<GlowSelected>().enabled = false;     // So disable selection
                    affected--;
                }
            }
            else if (buckets[i].Count >= 3)     // Excludes single or double 1 or 5, as nothing needs to be done with them
            {
                // We want to link each die of a kind together
                // The current way I do this is very bad, requiring up to 5^2 connections and 6^2 checks
                // I might implement something more efficient later, but this works for now
                GlowSelected[] gsList = new GlowSelected[buckets[i].Count];
                for(int j = 0; j < buckets[i].Count; j++)
                {
                    // While this adds another loop per check, it prevents calling GetComponent O(n^2) times
                    gsList[j] = buckets[i][j].diceObj.GetComponent<GlowSelected>();
                }

                foreach(GlowSelected gsi in gsList)     // still doing O(n^2) though, which im not happy about. but n is at most 6 so honestly its prob fine
                {
                    foreach(GlowSelected gsj in gsList)
                    {
                        if (!gsi.Equals(gsj))
                        {
                            gsi.LinkGlower(gsj);
                        }
                    }
                }

            }

        }
        return affected;
    }

}
