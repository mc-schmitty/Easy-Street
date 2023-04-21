using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceFace
{
    Up = 1,
    Forward = 2,
    Left = 3,
    Right = 4,
    Back = 5,
    Down = 6
}

public class FaceDetection : MonoBehaviour
{
    // Child transform that carries the actal dice numbers
    [SerializeField] private Transform childTransform;
    public bool debugMode;

    // Rotates the numbers to any desired location
    public void RotateFaces(Quaternion rotation)
    {
        childTransform.localRotation *= rotation;
    }

    // Resets numbers to default orientation
    public void ResetFaces()
    {
        childTransform.localRotation = Quaternion.identity;
    }

    // Summary:
    // Checks which face on an approximated cube is facing up.
    public DiceFace CheckFace()
    {
        // Get facings for all 
        float upF = Vector3.Dot(childTransform.up, Vector3.up);
        float fwdF = Vector3.Dot(childTransform.forward, Vector3.up);
        float rightF = Vector3.Dot(childTransform.right, Vector3.up);
        // Convert to int
        int up = Mathf.RoundToInt(upF);
        int fwd = Mathf.RoundToInt(fwdF);
        int right = Mathf.RoundToInt(rightF);

        // Debug
        if (debugMode)
        {
            Debug.Log(gameObject.GetInstanceID() + "> up: " + upF + ", fwd: " + fwdF + ", right: " + rightF);
        }
        
        //Nasty looking ifs     (could definitely reduce some of the mathf calls but whatever if it works)
        if(up != 0 && Mathf.Abs(upF) > Mathf.Abs(fwdF) && Mathf.Abs(upF) > Mathf.Abs(rightF))
        {
            if (up == 1)
                return DiceFace.Up;
            else
                return DiceFace.Down;
        }
        else if(fwd != 0 && Mathf.Abs(fwdF) > Mathf.Abs(upF) && Mathf.Abs(fwdF) > Mathf.Abs(rightF))
        {
            if (fwd == 1)
                return DiceFace.Forward;
            else
                return DiceFace.Back;
        }
        else
        {
            if (right == 1)
                return DiceFace.Right;
            else
                return DiceFace.Left;
        }
    }


    private void Update()
    {
        if(debugMode)
            Debug.Log(gameObject.GetInstanceID() + "> result: " + ((int)CheckFace()));
    }
}
