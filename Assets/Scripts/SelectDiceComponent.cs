using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectDiceComponent : MonoBehaviour
{
    
    private Rigidbody savedDice;
    public Rigidbody SavedDice 
    {
        get 
        {
            return isEmpty ? null : savedDice;      // Returns null if empty, otherwise returns stored rb
        } 
    }
    public Vector3 savePoint;
    private Vector3 oldPos;
    private Quaternion oldRot;

    private bool isEmpty;
    public bool Empty
    {
        get
        {
            return isEmpty;
        }
    }

    private AnimationCurve curve;    // Makes things look nice
    public AnimationCurve SetCurve
    {
        set
        {
            curve = value;
        }
    }
    
    private void Awake()
    {
        savePoint = transform.position;
        isEmpty = true;
    }

    // Move a rb into said position
    public void Insert(Rigidbody rb, float timeToMove)
    {
        if (isEmpty)
        {
            savedDice = rb;
            isEmpty = false;
            oldPos = rb.position;
            oldRot = rb.rotation;
            StartCoroutine(RigidbodyLerp(rb, savePoint, RoundRotation(oldRot), timeToMove));
        }
    }

    // Return rb to its original position
    public void Remove(float timeToMove)
    {
        if (!isEmpty)
        {
            StartCoroutine(RigidbodyLerp(savedDice, oldPos, oldRot, timeToMove));
            savedDice = null;
            oldPos = savePoint;
            oldRot = Quaternion.identity;
            isEmpty = true;
        }
    }

    // Clears stored parameters and resets values
    public void Clear()
    {
        savedDice = null;
        oldPos = savePoint;
        oldRot = Quaternion.identity;
        isEmpty = true;
    }

    // Just rounds all angles to 90 degrees
    private Quaternion RoundRotation(Quaternion initialRot)
    {
        Vector3 euler = initialRot.eulerAngles;
        euler.x = RoundRotationHelper(euler.x);
        euler.y = RoundRotationHelper(euler.y);
        euler.z = RoundRotationHelper(euler.z);

        return Quaternion.Euler(euler);

    }

    // Rounds float to nearest 90 degrees
    private float RoundRotationHelper(float angle)
    {
        float neg = angle < 0 ? -1 : 1;
        return Mathf.Ceil(Mathf.Floor(Mathf.Abs(angle) / 45) / 2) * 90 * neg;
    }

    IEnumerator RigidbodyLerp(Rigidbody rb, Vector3 destination, Quaternion destRotation, float timeToMove)
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
        Vector3 startPoint = rb.position;
        Quaternion startRot = rb.rotation;

        float stepSize = Time.fixedDeltaTime * timeToMove;
        for(float step = stepSize; step <= 1; step += stepSize)
        {
            rb.MovePosition(Vector3.Lerp(startPoint, destination, curve.Evaluate(step)));
            rb.MoveRotation(Quaternion.Lerp(startRot, destRotation, curve.Evaluate(step)));
            yield return new WaitForFixedUpdate();
        }

        rb.position = destination;
        rb.rotation = destRotation;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
