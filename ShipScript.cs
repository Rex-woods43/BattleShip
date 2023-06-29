using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    //tiles
    List<GameObject> touchTiles = new List<GameObject>();
    public float xOffset = 0;
    public float zOffset = 0;
    public float zRotation = 90f;
    public GameObject ClickedTile;

    //vectors
    private Vector3 screenPoint;
    private Vector3 offset;
    public Vector3 startPosition;

    //ships
    public int shipSize;
    private bool onBoard;
    int hitCount = 0;

    //materials/colours
    private Material[] allMaterials;
    List<Color> allColours = new List<Color>();


    void Start()
    {
        //save start position
        startPosition = transform.position;

        //define materials
        allMaterials = GetComponent<Renderer>().materials;
        for(int i = 0; i < allMaterials.Length; i++)
        {
            allColours.Add(allMaterials[i].color);
        }
    }

    //record tiles
    public void RecordTilePlacement(Collision collision)
    {
        touchTiles.Add(collision.gameObject);
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    //set position functions
    public void SetClickedTile(GameObject Tile)
    {
        ClickedTile = Tile;
    }

    public Vector3 getOffset(Vector3 TilePos)
    {
        return new Vector3(TilePos.x - xOffset, 2, TilePos.z + zOffset);
    }

    public void setPosition(Vector3 moveShipToHere)
    {
        transform.position = new Vector3(moveShipToHere.x + xOffset, 2, moveShipToHere.z + zOffset);
    }

    public void rotate()
    {
        if(ClickedTile == null) { return; }
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;
        transform.Rotate(0, 0, zRotation);
        ClearTileList();
        setPosition(ClickedTile.transform.position);
        zRotation *= -1;
    }

   //boolean checks
    public bool CheckIfSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }

    public bool OnGameBoard()
    {
        return touchTiles.Count == shipSize;
    }

    //changing colours
    public void FlashColour(Color flashColour)
    {
        foreach(Material mat in allMaterials)
        {
            mat.color = flashColour;
        }
        Invoke("ResetColour", 0.5f);
    }

    public void ResetColour()
    {
        int i = 0;
        foreach (Material mat in allMaterials)
        {
            mat.color = allColours[i++];
        }
    }
}
