using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSelected : MonoBehaviour
{
    public Action<GlowSelected, GlowSelected[]> OnMouseSelect { get; set; }

    private Renderer rend;
    private Color originalColor; // OC
    private Color dullColor = new(0.3f, 0.3f, 0.3f, 1);     // No point making this editable since its local to each glower
    [SerializeField]
    private List<GlowSelected> linkedGlowers;
    private bool dullFlag = false;      // Whether the dice should be dull or not

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    private void OnEnable()
    {
        StopDull();
    }

    private void OnDisable()
    {
        OnMouseExit();  // In the case this gets disabled while the mouse is still on it, 
        SetDull(true);  // Want rolling to be finished before dulling, so don't immediately update (note currently update is galled in dicegame, but could be moved to here)
    }

    private void OnMouseEnter()
    {
        if (!enabled)   // Don't glow if disabled
            return;

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
        if (!enabled)   // Don't glow if disabled
            return;

        StopGlow();
        foreach (GlowSelected gs in linkedGlowers)
        {
            gs.StopGlow();
        }
        LineRendererManager.Manager.DecomissionLine(this.GetInstanceID());          // get this line outta here
    }

    private void OnMouseDown()
    {
        if (!enabled)   // Don't glow if disabled
            return;

        OnMouseSelect?.Invoke(this, linkedGlowers.ToArray());
        //Debug.Log(OnMouseSelect.GetInvocationList().Length);
    }

    public void StartGlow()
    {
        rend.material.color = Color.yellow;
    }

    public void StopGlow()
    {
        rend.material.color = originalColor;
    }

    public void StartDull()
    {
        rend.material.color = dullColor;
        dullFlag = true;
    }

    public void StopDull()
    {
        rend.material.color = originalColor;
        dullFlag = false;
    }

    // Sets the flag so dice dulling can be called later
    public void SetDull(bool set)
    {
        dullFlag = set;
    }

    // Updates dice dulling to result of dull flag
    public void UpdateDull()
    {
        if (dullFlag)
            StartDull();
        else
            StopDull();
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
