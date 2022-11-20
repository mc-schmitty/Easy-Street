using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollManager : MonoBehaviour
{
    [SerializeField]
    private DiceSimulation sim;
    [SerializeField]
    private Rigidbody dice;
    private FaceDetection fd;
    private Vector3 diceStartingPos;
    private Quaternion diceStartingRot;
    [SerializeField]
    private Button[] rollButtons;

    private int simmedRoll;
    private int wantedRoll;
    private bool canRoll;

    void Start()
    {
        //Debug.unityLogger.logEnabled = false;
        fd = dice.GetComponent<FaceDetection>();
        diceStartingPos = dice.position;
        diceStartingRot = dice.rotation;

        canRoll = true;
    }

    private void Update()
    {
        if (canRoll)
        {
            StartCRoll();
        }
    }

    public void StartCRoll()
    {
        StartCRoll(Random.Range(1, 7));  // Get random dice face
    }

    /// <summary>
    ///  Rolls a single dice with random force. Simulates roll and apples correction to turn roll result into expectedRoll.
    /// </summary>
    /// <param name="expectedRoll"></param>
    public void StartCRoll(int expectedRoll)
    {
        wantedRoll = expectedRoll;
        Debug.Log("Attempting to roll a " + expectedRoll);

        // Get random roll forces
        Vector3 randForce = RandomForce();
        Vector3 randTorque = RandomTorque();

        // Reset dice transform (actually its rigidbody i hope that reflected in the transform otherwise we might have issues)
        dice.position = diceStartingPos;
        dice.rotation = diceStartingRot;
        dice.velocity = Vector3.zero;
        dice.angularVelocity = Vector3.zero;
        fd.ResetFaces();

        // Start simulation with random generated dice roll
        DiceFace result = sim.SimulateRoll(diceStartingPos, diceStartingRot, randForce, randTorque);
        simmedRoll = (int)result;
        /* Testing */
        //expectedRoll = simmedRoll;
        //wantedRoll = simmedRoll;
        /* Testing */
        // Generate correction quaternion
        Quaternion correctiveRotation = GenerateCorrectiveRotation(result, (DiceFace)expectedRoll);

        Debug.Log("Simulation rolled a " + (int)result + " with Force: " + randForce.ToString("n2") + "; Torque: "
            + randTorque.ToString("n2") + ".\nApplying corrective rotation: " + correctiveRotation.eulerAngles);

        // Apply correction, then force
        //dice.rotation *= correctiveRotation;
        //dice.angularVelocity = Vector3.zero;
        fd.RotateFaces(correctiveRotation);
        dice.AddForce(randForce, ForceMode.Impulse);
        dice.AddTorque(randTorque, ForceMode.Impulse);
        canRoll = false;        // Now rolling, so cant roll until finished
        foreach(Button b in rollButtons)
        {
            b.interactable = false;
        }
    }

    public void StartNRoll()
    {
        wantedRoll = 0;
        simmedRoll = 0;

        // Get random roll forces
        Vector3 randForce = RandomForce();
        Vector3 randTorque = RandomTorque();

        // Reset dice transform (actually its rigidbody i hope that reflected in the transform otherwise we might have issues)
        dice.position = diceStartingPos;
        dice.rotation = diceStartingRot;
        dice.velocity = Vector3.zero;
        dice.angularVelocity = Vector3.zero;

        Debug.Log("Rolling dice with Force: " + randForce.ToString("n2") + " and Torque: " + randTorque.ToString("n2"));
        dice.AddForce(randForce, ForceMode.Impulse);
        dice.AddTorque(randTorque, ForceMode.Impulse);
        canRoll = false;        // Now rolling, so cant roll until finished
        foreach (Button b in rollButtons)
        {
            b.interactable = false;
        }
    }

    private void FixedUpdate()
    {
        if (!canRoll)
        {
            if(dice.position != diceStartingPos && dice.velocity == Vector3.zero && dice.angularVelocity == Vector3.zero)       // Test if rolling has stopped
            {
                int rolledResult = (int)(fd.CheckFace());
                if(wantedRoll == 0)
                {
                    Debug.Log("Rolled a " + rolledResult + "!");
                }
                else if(rolledResult == wantedRoll)
                {
                    Debug.Log("Successfully changed a " + simmedRoll + " roll to a " + rolledResult + " roll!");
                }
                else
                {
                    Debug.LogError("Roll correction failed. Expected " + wantedRoll + " but rolled " + rolledResult);
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
        return new Vector3(Random.Range(-1.6f, -2f), Random.Range(0f, 0.5f), Random.Range(-0.8f, 0.8f));
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
