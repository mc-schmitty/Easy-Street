using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSelected : MonoBehaviour
{
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
        foreach (GlowSelected gs in linkedGlowers)
        {
            gs.StartGlow();
        }
    }

    private void OnMouseExit()
    {
        StopGlow();
        foreach(GlowSelected gs in linkedGlowers)
        {
            gs.StopGlow();
        }
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
