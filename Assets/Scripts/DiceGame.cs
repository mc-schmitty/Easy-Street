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
    public int points;

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

    private DiceRollManager drm;

    void Start()
    {
        points = 0;
        diceList = new Dice[6];
        activeDice = new List<Dice>();
        heldDice = new List<Dice>();
        for(int i = 0; i < 6; i++)
        {
            diceList[i] = new Dice();
            activeDice.Add(diceList[i]);
        }

        drm = GetComponent<DiceRollManager>();
    }

    public void Roll()
    {
        foreach(Dice d in activeDice)
        {
            //_ = d.Roll;
            d.Roll();
        }
    }

    public int DetermineActiveScore()
    {
        int activeScore = 0;
        bool straightFlag = true;

        // assign dice to buckets
        int[] buckets = { 0, 0, 0, 0, 0, 0 };   // empy
        foreach(Dice d in activeDice)
        {
            buckets[d.result - 1]++;    // add to buckt
        }

        // Now score dice
        for(int i = 0; i < 6; i++)
        {
            straightFlag = straightFlag && buckets[i] == 1;         // Fails if ever more or less than 1 die in each bucket
            activeScore += ScoreMachine(i, buckets[i]);             // Scores dice result
        }

        if (straightFlag)
            activeScore = StraightValue;
        return activeScore;
    }

    public void GameRound()
    {
        // 1. Roll active dice
        Roll();
        PrintRoll();
        int[] temp = new int[6];
        for(int i = 0; i < 6; i++)
        {
            temp[i] = activeDice.Contains(diceList[i]) ? diceList[i].result : 0;    // gets active dice into list so dicerollmanager can roll it
        }
        drm.StartCRoll(temp);


        // 2. Score dice, find pairs etc
        int maxScore = DetermineActiveScore();
        Debug.Log("Score: " + maxScore);
        if (maxScore == 0)
            return;     // Round over

        // 3. Let user choose pairs, bank/ reroll (go to 1)

        // 4. If above not possible, end round
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

        int diceValue = value + 1;
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

}
