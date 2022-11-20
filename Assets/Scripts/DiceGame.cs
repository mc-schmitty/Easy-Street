using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public class DiceGame : MonoBehaviour
{
    public int points;

    private Dice[] diceList;
    private List<Dice> activeDice;
    private List<Dice> heldDice;


    void Start()
    {
        points = 0;
        diceList = new Dice[6];
        for(int i = 0; i < 6; i++)
        {
            diceList[i] = new Dice();
        }
    }

    public void Roll()
    {
        foreach(Dice d in activeDice)
        {
            _ = d.Roll;
        }
    }

    public void DetermineActiveScore()
    {

    }

    public void GameRound()
    {
        Roll();

    }

}
