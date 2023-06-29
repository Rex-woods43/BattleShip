using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymissileScript : MonoBehaviour
{
    private gameManager gameManager;
    private enemyGameManager enemygameManager;
    public Vector3 targetTileLocation;
    private int targetTile = -1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<gameManager>();
        enemygameManager = GameObject.Find("EnemyGameManager").GetComponent<enemyGameManager>();
    }

    //when the missile hits something
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            gameManager.EnemyHitPlayer(targetTileLocation, targetTile, collision.gameObject);
        }
        else
        {
            enemygameManager.EnemyMissPlayer(targetTile);
        }
    }

    public void SetTarget(int tileNumber)
    {
        targetTile = tileNumber;
    }
}
