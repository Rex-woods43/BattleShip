using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    gameManager gameManager;
    Ray ray;
    RaycastHit hit;

    public Color32[] Colour = new Color32[2];
    int ColourIndex;

    bool tileGuessed = false;
   
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();

        //set material list
        Colour[0] = gameObject.GetComponent<MeshRenderer>().material.color;
        Colour[1] = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        checkClicked();
    }

    private void checkClicked()
    {
        //if a tile is clicked, call the "TileClicked" function
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == gameObject.name)
            {
                //make it so that the user cant guess a tile that they have already guessed
                if (tileGuessed == false)
                {
                    gameManager.TileClicked(hit.collider.gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if a tile is hit by a missile, enemymissile or ship
        if (collision.gameObject.CompareTag("Missile"))
        {
            tileGuessed = true;
            gameManager.CheckYourGuess(gameObject);
        }
        else if (collision.gameObject.CompareTag("EnemyMissile"))
        {
            Colour[0] = new Color32(38, 57, 76, 255);
            SwitchColours(ColourIndex);
        }
        else if (collision.gameObject.CompareTag("Ship"))
        {
            gameManager.RecordTilePlacement(collision);
        }
    }

    public void SetTileColour(int index, Color32 colour )
    {
        Colour[index] = colour;
    }

    public void SwitchColours(int ColourIndex)
    {
         GetComponent<Renderer>().material.color = Colour[ColourIndex];
    }
}
