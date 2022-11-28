using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectDiceComponent : MonoBehaviour
{
    Rigidbody savedDice;
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
    
    private void Start()
    {
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
            StartCoroutine(VectorLerp(rb, savePoint, timeToMove));
        }
    }

    // Return rb to its original position
    public void Remove(float timeToMove)
    {
        if (!isEmpty)
        {
            StartCoroutine(VectorLerp(savedDice, oldPos, timeToMove));
            savedDice = null;
            oldPos = savePoint;
            oldRot = Quaternion.identity;
            isEmpty = true;
        }
    }

    IEnumerator VectorLerp(Rigidbody rb, Vector3 destination, float timeToMove)
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
        Vector3 startPoint = rb.position;

        float stepSize = Time.fixedDeltaTime * timeToMove;
        for(float step = stepSize; step <= 1; step += stepSize)
        {
            rb.MovePosition(Vector3.Lerp(startPoint, destination, step));
            yield return new WaitForFixedUpdate();
        }
    }
}
