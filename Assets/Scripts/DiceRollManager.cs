using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollManager : MonoBehaviour
{
    [SerializeField]
    private DiceSimulation sim;
    [SerializeField]
    private Rigidbody[] dice;
    private int diceLength;
    private FaceDetection[] fd;
    private Vector3[] diceStartingPos;
    private Quaternion[] diceStartingRot;
    [SerializeField]
    private Button[] rollButtons;

    private int[] simmedRoll;
    private int[] wantedRoll;
    private int[] rolledResult;
    private bool canRoll;
    private bool ffskip; // ignore first check

    //test
    Vector3[] randForces;
    Vector3[] randTorques;
    //test
    void Start()
    {
        diceLength = dice.Length;
        fd = new FaceDetection[diceLength];
        diceStartingPos = new Vector3[diceLength];
        diceStartingRot = new Quaternion[diceLength];
        //Debug.unityLogger.logEnabled = false;
        // test
        randForces = new Vector3[diceLength];
        randTorques = new Vector3[diceLength];
        // test
        for (int i = 0; i < diceLength; i++)
        {
            fd[i] = dice[i].GetComponent<FaceDetection>();
            diceStartingPos[i] = dice[i].position;
            diceStartingRot[i] = dice[i].rotation;
            //test
            randForces[i] = RandomForce();
            randTorques[i] = RandomTorque();
        }

        simmedRoll = new int[diceLength];
        rolledResult = new int[diceLength];
        canRoll = true;
        ffskip = true;
    }

    private void bUpdate()
    {
        if (canRoll)
        {
            StartCRoll();
        }
    }

    public void StartCRoll()
    {
        int[] temp = new int[diceLength];
        for(int i = 0; i < diceLength; i++)
        {
            temp[i] = Random.Range(1, 7);
        }
        StartRoll(temp, true);
    }

    public void StartCRoll(int num)
    {
        int[] temp = new int[diceLength];
        for (int i = 0; i < diceLength; i++)
        {
            temp[i] = num;
        }
        StartRoll(temp, true);
    }

    public void StartCRoll(int[] expectedRolls)
    {
        StartRoll(expectedRolls, true);
    }

    public void StartNRoll()
    {
        int[] temp = new int[diceLength];
        for(int i = 0; i < diceLength; i++)
        {
            temp[i] = 7;
        }
        StartRoll(temp, false);
    }

    /// <summary>
    ///  Rolls a single dice with random force. Simulates roll and apples correction to turn roll result into expectedRoll.
    /// </summary>
    /// <param name="expectedRolls"></param> expected rolls oh wow this is outdated
    /// <param name="correctionRoll"></param> whether rolls should be corrected or not
    private void StartRoll(int[] expectedRolls, bool correctionRoll)
    {
        wantedRoll = expectedRolls;
        if(correctionRoll)
            Debug.Log("Attempting to roll " + expectedRolls.ToCommaSeparatedString());

        // Get random roll forces
        //Vector3[] randForces = new Vector3[diceLength];
        //Vector3[] randTorques = new Vector3[diceLength];
        bool[] rolling = new bool[diceLength];

        for(int i = 0; i < diceLength; i++)     // Setup dice pre-simulation
        {
            if (expectedRolls[i] > 0)
            {
                randForces[i] = RandomForce();
                randTorques[i] = RandomTorque();

                // Reset dice transform (actually its rigidbody i hope that reflected in the transform otherwise we might have issues)
                dice[i].position = diceStartingPos[i];
                dice[i].rotation = diceStartingRot[i];
                dice[i].velocity = Vector3.zero;
                dice[i].angularVelocity = Vector3.zero;
                fd[i].ResetFaces();
                rolling[i] = true;
            }
            else
            {
                rolling[i] = false;     // Determine which dice are going to be simulated or not
            }
            
        }

        if (correctionRoll)     // Check if sim required, apply and post-sim results
        {
            // Start simulation with random generated dice roll
            DiceFace[] result = sim.SimulateRoll(rolling, diceStartingPos, diceStartingRot, randForces, randTorques);
            List<Vector3> qrs = new();
            for (int i = 0; i < diceLength; i++)
            {
                if (rolling[i])
                {
                    simmedRoll[i] = (int)result[i];
                    // Generate correction quaternion
                    Quaternion correctiveRotation = GenerateCorrectiveRotation(result[i], (DiceFace)wantedRoll[i]);
                    qrs.Add(correctiveRotation.eulerAngles);
                    fd[i].RotateFaces(correctiveRotation);                  // Apply correction
                    ApplyForce(dice[i], randForces[i], randTorques[i]);     // Apply force
                }  
            }
            Debug.Log("Simulation rolled a " + simmedRoll.ToCommaSeparatedString() + " with Force: " + randForces.ToCommaSeparatedString() + "; Torque: "
                + randTorques.ToCommaSeparatedString() + ".\nApplying corrective rotation: " + qrs.ToCommaSeparatedString());
        }
        else
        {
            for(int i = 0; i < diceLength; i++)     // Just apply the force
            {
                if (rolling[i])
                {
                    ApplyForce(dice[i], randForces[i], randTorques[i]);
                }
            }
        }

        canRoll = false;        // Now rolling, so cant roll until finished
        ffskip = true;
        foreach(Button b in rollButtons)
        {
            b.interactable = false;
        }
    }

    private void FixedUpdate()
    {
        if (!canRoll)
        {
            if (ffskip)
            {
                ffskip = false;
                /*for(int i=0; i < diceLength; i++)
                {
                    if (dice[i].position == diceStartingPos[i])
                        return;
                }*/
                return;
            }

            if(!sim.TestForVelocity(dice))       // Test if rolling has stopped
            {
                for(int i = 0; i < diceLength; i++)
                {
                    rolledResult[i] = (int)(fd[i].CheckFace());     // Check all new faces
                }

                if (wantedRoll[0] == 7)
                {
                    Debug.Log("Rolled " + rolledResult.ToCommaSeparatedString() + "!");
                }
                else if(rolledResult.ToCommaSeparatedString() == wantedRoll.ToCommaSeparatedString())
                {
                    Debug.Log("Successfully changed " + simmedRoll.ToCommaSeparatedString() + " roll to " + rolledResult.ToCommaSeparatedString() + " roll!");
                }
                else
                {
                    Debug.LogError("Roll correction failed. Expected " + wantedRoll.ToCommaSeparatedString() + " but rolled " + rolledResult.ToCommaSeparatedString());
                }
                canRoll = true;
                foreach (Button b in rollButtons)
                {
                    b.interactable = true;
                }
            }
        }
    }

    private Vector3 RandomForce()
    {
        // Generate random vector in any direction, with added force multiplier
        return new Vector3(Random.Range(-1.6f, -2f), Random.Range(0f, 0.5f), Random.Range(-0.01f, 0.01f));
    }

    private Vector3 RandomTorque()
    {
        // Generate random vector in any direction
        return Random.onUnitSphere;
    }

    // Generates rotation from one face to the other, which will correct dice when applied
    private Quaternion GenerateCorrectiveRotation(DiceFace currentEndingFace, DiceFace intendedEndingFace)
    {
        return Quaternion.FromToRotation(FaceToVector(intendedEndingFace), FaceToVector(currentEndingFace));
    }

    private void ApplyForce(Rigidbody rb, Vector3 force, Vector3 torque)
    {
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    // I think I'm gonna make a static class for this conversion function eventially (maybe face detection too), but moving it here for now (again)
    private Vector3 FaceToVector(DiceFace df)  // Gonna be moved eventually
    {
        switch (df)
        {
            case DiceFace.Up:
                return Vector3.up;
            case DiceFace.Forward:
                return Vector3.forward;
            case DiceFace.Left:
                return Vector3.left;
            case DiceFace.Right:
                return Vector3.right;
            case DiceFace.Back:
                return Vector3.back;
            case DiceFace.Down:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }
}
