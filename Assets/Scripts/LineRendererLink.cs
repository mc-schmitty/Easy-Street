using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * LineRenderer except with Transforms instead of Vectors
 */


[RequireComponent(typeof(LineRenderer))]
public class LineRendererLink : MonoBehaviour
{
    public bool Enabled
    {
        get{return lr.enabled;}
        set{lr.enabled = value;}
    }

    LineRenderer lr;
    int i;      // Funny cached i
    List<Transform> assignedObjects;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        assignedObjects = new List<Transform>();
    }

    private void Start()
    {
        lr.enabled = false;
    }

    private void Update()
    {
        lr.positionCount = assignedObjects.Count;
        for(i = 0; i < assignedObjects.Count; i++)
        {
            lr.SetPosition(i, assignedObjects[i].position);
        }
    }

    public void AddNode(Transform node)
    {
        assignedObjects.Add(node);
    }

    public void RemoveNode(Transform node)
    {
        assignedObjects.Remove(node);
    }

    public void ClearNodes()
    {
        assignedObjects.Clear();
    }

    public void EnableLine()
    {
        lr.enabled = true;
    }

    public void DisableLine()
    {
        lr.enabled = false;
    }

    public bool IsEnabled()
    {
        return lr.enabled;
    }
}
