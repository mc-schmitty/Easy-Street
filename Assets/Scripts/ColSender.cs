using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColSender : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.body != null)
            Debug.Log("Collision with" + collision.body + "Entered at:" + Time.frameCount + "\nGenerating impulse of:" + collision.impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.body != null)
            Debug.Log("Collision with" + collision.body + "Staying at:" + Time.frameCount + "\nGenerating impulse of:" + collision.impulse);
    }
}
