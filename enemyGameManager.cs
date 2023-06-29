using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class enemyGameManager : MonoBehaviour
{
    char[] guessGrid;
    List<int> potentialHit;
    List<int> currentHit;
    private int guess;
    public GameObject enemyMissilePrefab;
    public gameManager gameManager;
    public TextMeshProUGUI MainText;

    void Start()
    {
        potentialHit = new List<int>();
        currentHit = new List<int>();
        guessGrid = Enumerable.Repeat('o', 100).ToArray();
    }

    public List<int[]> PlaceEnemyShips()
    {
        //list of ships represented as integers (number of ints = amount of space the ship takes up)
        List<int[]> enemyShips = new List<int[]>
        {
        new int[] { -1, -1, -1, -1, -1 },
        new int[] { -1, -1, -1, -1 },
        new int[] { -1, -1, -1 },
        new int[] { -1, -1, -1 },
        new int[] { -1, -1 }
        };

        int[] gridNumbers = Enumerable.Range(1, 100).ToArray();
        bool TileUnavailable = true;

        foreach (int[] shipArray in enemyShips)
        {
            TileUnavailable = true;
            //while tile is unavailable, assign new tile
            while (TileUnavailable == true)
            {
                TileUnavailable = false;
                int shipNose = UnityEngine.Random.Range(0, 99); //assigns the head of the ship a random tile to be on
                int rotateBool = UnityEngine.Random.Range(0, 2); //assigns the ship a random rotation
                int minusAmount = rotateBool == 0? 10 : 1; //if rotateBool is 0, minusAmount is 10, otherwise its 1
                
                //checking the random placements are viable
                for(int i = 0; i < shipArray.Length; i++)
                {
                    //if ship is going to go off the board, make the placement unavailable
                    if((shipNose - (minusAmount *i)) < 0 || (gridNumbers[shipNose - i * minusAmount]) < 0)
                    {
                        TileUnavailable = true;
                        break;
                    }
                    //double checking calculations
                    else if (minusAmount == 0 && shipNose /10 != ((shipNose - i * minusAmount) -1) /10)
                    {
                        TileUnavailable = true;
                        break;
                    }
                }

                //after the checking is completed, assign the placement to the ship array
                if(TileUnavailable == false)
                {
                    for (int j = 0; j < shipArray.Length; j++)
                    {
                        shipArray[j] = gridNumbers[shipNose - j * minusAmount];
                        gridNumbers[shipNose - j * minusAmount] = -1;
                    }
                }
            }
        }
        foreach (int[] shipArray in enemyShips)
        {
            string temp = " ";
            for (int i = 0; i < shipArray.Length; i++)
            {
                temp += ", " + shipArray[i];
            }
            Debug.Log(temp);
        }
        return enemyShips;
    }

    public void NPCTurn()
    {
        //add the index of the hits into 'hit index'
        List<int> hitIndex = new List<int>();
        for (int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') // h for hit
            {
                hitIndex.Add(i);
            }
        }
        //if you have more than one hit next to each other, hit the next square in the row
        if(hitIndex.Count > 1)
        {
            int diff = hitIndex[1] - hitIndex[0];
            int posNeg = Random.Range(0, 2) * 2 - 1;
            int nextIndex = hitIndex[0] + diff;
            while (guessGrid[nextIndex] != 'o')
            {
                if (guessGrid[nextIndex] == 'm' || nextIndex > 100 || nextIndex < 0)
                {
                    diff *= -1;
                }
                nextIndex += diff;
            }
            guess = nextIndex;
        }
        //if you have one hit, choose a tile near it
        else if (hitIndex.Count == 1)
        {
            List<int> closeTiles = new List<int>();
            closeTiles.Add(1); closeTiles.Add(-1); closeTiles.Add(10); closeTiles.Add(-10); //define the close tiles
            int index = Random.Range(0, closeTiles.Count); //choose a random close tile
            int possibleGuess = hitIndex[0] + closeTiles[index];
            bool onGrid = possibleGuess > -1 && possibleGuess < 100; //check that the guess is on the board
            //if that guess is unavailable, choose a new close tile
            while((!onGrid || guessGrid[possibleGuess] != 'o') && closeTiles.Count > 0)
            {
                closeTiles.RemoveAt(index);
                index = Random.Range(0, closeTiles.Count);
                possibleGuess = hitIndex[0] + closeTiles[index];
                onGrid = possibleGuess > -1 && possibleGuess < 100; 
            }
            guess = possibleGuess;

        }
        //if you have no hits, choose a random tile
        else
        {
            int nextIndex = Random.Range(0, 100);
            while (guessGrid[nextIndex] != 'o')
            {
                nextIndex = Random.Range(0, 100);
            }
            guess = nextIndex;
        }
        GameObject targettile = GameObject.Find("Tile (" + (guess + 1) + ")");
        Vector3 vec = targettile.transform.position;
        guessGrid[guess] = 'm';
        vec.y += 15;
        GameObject enemymissile = Instantiate(enemyMissilePrefab, vec, enemyMissilePrefab.transform.rotation);
        enemymissile.GetComponent<enemymissileScript>().SetTarget(guess);
        enemymissile.GetComponent<enemymissileScript>().targetTileLocation = targettile.transform.position;
        Destroy(enemymissile, 2.1f);
    }

    //adds the hits to the grid
    public void MissileHit(int hit)
    {
        guessGrid[guess] = 'h';
        Invoke("EndTurn", 2.0f);
    }

    //adds the finished ships to the grid
    public void SunkPlayer()
    {
        for(int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h')
            {
                guessGrid[i] = 'x';
            }
        }
    }

    //removes the missed tiles and all tiles in that direction from potential future guesses for that ship
    public void EnemyMissPlayer(int missTile)
    {
        MainText.text = "MISS!";
        if (currentHit.Count > 0 && currentHit[0] > missTile)
        {
            foreach(int potential in potentialHit)
            {
                if (potential < missTile)
                {
                    potentialHit.Remove(potential);
                }
                else
                {
                    if (potential > missTile)
                    {
                        potentialHit.Remove(potential);
                    }
                }
            }
        }
        Invoke("EndTurn", 2.0f);
    }

    public void EndTurn()
    {
        gameManager.GetComponent<gameManager>().EndEnemyTurn();
    }
}
