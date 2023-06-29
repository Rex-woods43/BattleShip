using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireScript : MonoBehaviour
{
    public GameObject redFire;
    public GameObject yellowFire;
    public GameObject orangeFire;
    int count;

    List<Color> fireColours = new List<Color> { Color.red, Color.yellow, new Color(1.0f, 64f, 0) };

    void FixedUpdate()
    {
        if (count > 30)
        {
            //makes the components of the fire object randomly change colour to create a flickering effect
            fireColours.Add(Color.red);
            int rnd = Random.Range(0, fireColours.Count);
            redFire.GetComponent<Renderer>().material.SetColor("_Color", fireColours[rnd]);
            fireColours.RemoveAt(rnd);
            rnd = Random.Range(0, fireColours.Count);
            orangeFire.GetComponent<Renderer>().material.SetColor("_Color", fireColours[rnd]);
            fireColours.RemoveAt(rnd);
            yellowFire.GetComponent<Renderer>().material.SetColor("_Color", fireColours[0]);
            fireColours = new List<Color> { Color.red, Color.yellow, new Color(1.0f, 64f, 0) };
            count = 0;
        }
        count++;

    }
}
