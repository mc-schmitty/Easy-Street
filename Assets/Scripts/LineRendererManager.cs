using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    public static LineRendererManager Manager;      // Make a singleton

    [SerializeField]
    private LineRendererLink linePrefab;
    private Stack<LineRendererLink> availableLines;             // Contains available, generated lines
    private Dictionary<int, LineRendererLink> assignedLines;    // Contains linked lines

    private void Awake()
    {
        if(Manager == null)
        {
            Manager = this;
        }
        else
        {
            this.enabled = false;
        }

        availableLines = new Stack<LineRendererLink>();
        assignedLines = new Dictionary<int, LineRendererLink>();
    }

    private void Start()
    {
        LineRendererLink init = Instantiate(linePrefab.gameObject).GetComponent<LineRendererLink>();
        
        availableLines.Push(init);
        
    }

    /// <summary>
    /// Generates a linerenderer of the given type with nodes set by the provided transforms.
    /// </summary>
    /// <param name="id">Id of calling GameObject, used for reference.</param>
    /// <param name="nodeList">List of transforms to be used as nodes.</param>
    /// <param name="type">Type of line required. Currently unused.</param>
    public void CommissionLine(int id, Transform[] nodeList, int type)
    {
        if (!assignedLines.ContainsKey(id))      // New reference, add nodes and line
        {
            if(availableLines.Count > 0)       // Available line, 
            {
                LineRendererLink line = availableLines.Pop();   // Get preinitialized line
                foreach(Transform t in nodeList)
                {
                    line.AddNode(t);
                }
                line.EnableLine();
                assignedLines.Add(id, line);

            }
            else        // Need to generate new line
            {
                LineRendererLink newLine = Instantiate(linePrefab).GetComponent<LineRendererLink>();     // New line
                foreach (Transform t in nodeList)
                {
                    newLine.AddNode(t);
                }
                newLine.EnableLine();
                assignedLines.Add(id, newLine);
            }
        }
        else        // Existing reference, just recommision line with new nodes
        {
            assignedLines[id].ClearNodes();
            foreach(Transform t in nodeList)
            {
                linePrefab.AddNode(t);
            }
            assignedLines[id].EnableLine();
        }
    }

    /// <summary>
    /// Frees line from use and adds it to available pool
    /// </summary>
    /// <param name="id">Id of calling GameObject.</param>
    public void DecomissionLine(int id)
    {
        if (!assignedLines.ContainsKey(id))     // Need reference to exist to decomission
            return;

        if (assignedLines[id] == null)          // Dont want null references inside dict
        {
            assignedLines.Remove(id);
            return;
        }

        LineRendererLink line = assignedLines[id];     // Remove from dict
        assignedLines.Remove(id);

        line.ClearNodes();  // Disable line
        line.DisableLine();

        availableLines.Push(line);      // Now line available, add it to stack
    }

}
