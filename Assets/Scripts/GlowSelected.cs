using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSelected : MonoBehaviour
{
    public Action<GlowSelected, GlowSelected[]> OnMouseSelect { get; set; }

    private Renderer rend;
    private Color originalColor; // OC
    [SerializeField]
    private List<GlowSelected> linkedGlowers;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        StartGlow();
        Transform[] tList = new Transform[linkedGlowers.Count + 1];      // Create list of transforms
        tList[0] = transform;
        int i = 1;
        foreach (GlowSelected gs in linkedGlowers)
        {
            gs.StartGlow();
            tList[i] = gs.transform;         // Add transforms to list
            i++;
        }
        LineRendererManager.Manager.CommissionLine(this.GetInstanceID(), tList, 0);     // please give line
    }

    private void OnMouseExit()
    {
        StopGlow();
        foreach (GlowSelected gs in linkedGlowers)
        {
            gs.StopGlow();
        }
        LineRendererManager.Manager.DecomissionLine(this.GetInstanceID());          // get this line outta here
    }

    private void OnMouseDown()
    {
        OnMouseSelect?.Invoke(this, linkedGlowers.ToArray());
    }

    public void StartGlow()
    {
        rend.material.color = Color.yellow;
    }

    public void StopGlow()
    {
        rend.material.color = originalColor;
    }

    public bool LinkGlower(GlowSelected gs)
    {
        if (linkedGlowers.Contains(gs))
        {
            return false;
        }
        linkedGlowers.Add(gs);
        return true;
    }

    public bool UnlinkGlower(GlowSelected gs)
    {
        if (linkedGlowers.Contains(gs))
        {
            linkedGlowers.Remove(gs);
            return true;
        }
        return false;
    }

    public void UnlinkAll()
    {
        linkedGlowers.Clear();
    }
}
