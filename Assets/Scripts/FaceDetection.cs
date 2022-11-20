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
        int up = Mathf.RoundToInt(Vector3.Dot(childTransform.up, Vector3.up));
        int fwd = Mathf.RoundToInt(Vector3.Dot(childTransform.forward, Vector3.up));
        int right = Mathf.RoundToInt(Vector3.Dot(childTransform.right, Vector3.up));
        
        //Nasty looking ifs     (ok coming back they arent too bad)
        if(up != 0)
        {
            if (up == 1)
                return DiceFace.Up;
            else
                return DiceFace.Down;
        }
        else if(fwd != 0)
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
}
