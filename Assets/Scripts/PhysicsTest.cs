using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTest : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startingPos;
    private Quaternion startingRot;

    [SerializeField] private Vector3 randForce;
    [SerializeField] private Vector3 randTorque;
    [SerializeField][Range(0, 6)] private int intendedRoll;
    private Quaternion correctionRot;
    private bool qfix = true;

    private FaceDetection fd;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startingPos = rb.position;
        startingRot = rb.rotation;

        correctionRot = Quaternion.identity;

        // Generate random force
        randForce = Random.insideUnitSphere * Random.Range(1f, 2f);
        randTorque = Random.insideUnitSphere;

        //Apply force
        ApplyRandForce();

        fd = GetComponent<FaceDetection>();
    }

    private void FixedUpdate()
    {
        if (qfix)   // Ignore first frame
        {
            qfix = false;
            return;
        }

        
        if(rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero)
        {
            DiceFace df = fd.CheckFace();
            Debug.Log("Position: " + rb.position.ToString() + " Face: " + df + "/Value: " + (int)df);   // Display position

            // Try to generate corrective rotation
            if (intendedRoll > 0)
            {
                if(df != (DiceFace)intendedRoll)
                    correctionRot = GenerateCorrectiveRotation(df, (DiceFace)intendedRoll);
            }
            else
                correctionRot = Quaternion.identity;

            // Reset to start, reapply force
            rb.MovePosition(startingPos);
            rb.MoveRotation(startingRot);
            ApplyRandForce();
            
            
        }
    }

    private void ApplyRandForce()
    {
        Debug.Log(correctionRot.eulerAngles);
        rb.MoveRotation(correctionRot);     // Test by adding a rotation before applying force
        rb.AddForce(randForce, ForceMode.Impulse);
        rb.AddTorque(randTorque, ForceMode.Impulse);
    }

    // Creates rotation from ending faces idk literally just guessing at this point
    private Quaternion GenerateCorrectiveRotation(DiceFace currentEndingFace, DiceFace intendedEndingFace)
    {
        return Quaternion.FromToRotation(FaceToVector(intendedEndingFace), FaceToVector(currentEndingFace));
    }

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
