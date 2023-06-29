using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using TMPro;

public class gameManager : MonoBehaviour
{
    [Header("Scripts")]
    //access scripts
    private ShipScript shipScript;
    public enemyGameManager enemygameManager;
    public List<TileScript> allTileScript;

    //administration
    private bool setupFinished = false;
    private bool userTurn = true;

    [Header("3D Objects")]
    //objects (3D)
    public GameObject[] ships;
    public GameObject[] tiles;
    public GameObject missilePrefab;
    public GameObject enemymissilePrefab;
    public GameObject arsenal;
    public GameObject firePrefab;
    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    int enemyShipCount = 5;
    int playerShipCount = 5;
    int finishedShips = 0;

    [Header("2D Objects")]
    //main canvas
    public Button rotateButton;
    public Button nextButton;
    public Button finishedButton;
    public TextMeshProUGUI MainText;
    public TextMeshProUGUI PlayerShipText;
    public TextMeshProUGUI PlayerCountText;
    public TextMeshProUGUI EnemyShipText;
    public TextMeshProUGUI EnemyCountText;

    //options canvas
    public GameObject optionsScreen;
    public Toggle fullScreenToggle;
    public Toggle vSyncToggle;
    public Toggle musicToggle;
    public Slider volumeSlider;
    public GameObject musicObject;
    private bool musicOn;
    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;
    public TextMeshProUGUI resText;

    //paused canvas
    public GameObject pausedCanvas;
    public Scene currentScene;
    public string sceneName;
    public bool pausedActive;

    //game over canvas
    public GameObject gameoverCanvas;
    public TextMeshProUGUI winnerText;

    //runs once at start
    void Start()
    {
        //set current scene
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        //only initialise the shipScript if you are on the board scene
        if (sceneName == "board1")
        {
            //get the script in reference to the currently selected ship
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
        }

        //start with options and pause screens closed
        optionsScreen.SetActive(false);
        pausedCanvas.SetActive(false);
        gameoverCanvas.SetActive(false);

        //start with counter text deactivated
        PlayerShipText.gameObject.SetActive(false);
        PlayerCountText.gameObject.SetActive(false);
        EnemyShipText.gameObject.SetActive(false);
        EnemyCountText.gameObject.SetActive(false);

        
        enemyShips = enemygameManager.PlaceEnemyShips();

        setOptionsState();

    }

    //runs every frame update
    void Update()
    {
        PauseOnEscape();
    }


    //change scene
    public void Gotoboard1()
    {
        SceneManager.LoadScene("board1");
        Time.timeScale = 1;
        rotateButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        finishedButton.gameObject.SetActive(true);
    }

    public void GoToTitlePage()
    {
        SceneManager.LoadScene("myTitlePage");
        optionsScreen.SetActive(false);
    }

    public void openOptions()
    {
        setOptionsState();
        optionsScreen.SetActive(true);
    }

    public void closeOptions()
    {
        optionsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        pausedCanvas.SetActive(true);
        Time.timeScale = 0;
        if (sceneName == "board1")
        {
            rotateButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            finishedButton.gameObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        pausedCanvas.SetActive(false);
        Time.timeScale = 1;
        if (sceneName == "board1" && !setupFinished)
        {
            rotateButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            finishedButton.gameObject.SetActive(true);
        }
    }

    private void PauseOnEscape()
    {
        //pause and unpause when escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneName = currentScene.name;
            if (sceneName == "board1")
            {
                if (!pausedCanvas.activeSelf)
                {
                    PauseGame();
                }
                else if (pausedCanvas.activeSelf)
                {
                    ResumeGame();
                }
            }
        }
    }

    private void GameOver(string Winner)
    {
        gameoverCanvas.SetActive(true);
        winnerText.text = Winner;
        Time.timeScale = 0;
    }

    //options controls
    public void applyChanges()
    {
        if (vSyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        musicOn = musicToggle.isOn;
        musicObject.SetActive(musicOn);
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullScreenToggle.isOn);
    }

    public void setOptionsState()
    {
        fullScreenToggle.isOn = Screen.fullScreen;
        if (QualitySettings.vSyncCount == 0)
        {
            vSyncToggle.isOn = false;
        }
        else
        {
            vSyncToggle.isOn = true;
        }
        musicOn = musicObject.activeSelf;
        musicToggle.isOn = musicOn;
        volumeSlider.value = AudioListener.volume;

        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if(Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundRes = true;
                selectedResolution = i;
                UpdateResLabel();
            }
        }

        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            selectedResolution = resolutions.Count - 1;
            UpdateResLabel();
        }
    }

    public void Changevolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void ResLeft()
    {
        selectedResolution--;
        if(selectedResolution < 0)
        {
            selectedResolution = 0;
        }
        UpdateResLabel();
    }

    public void ResRight()
    {
        selectedResolution++;
        if(selectedResolution > resolutions.Count - 1)
        {
            selectedResolution = resolutions.Count - 1;
        }
        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        resText.text = resolutions[selectedResolution].horizontal.ToString() + "x" + resolutions[selectedResolution].vertical.ToString();
    }

    [System.Serializable]
    public class ResItem
    {
        public int horizontal, vertical;
    }


    //game functions
    public void TileClicked(GameObject Tile)
    {
        pausedActive = pausedCanvas.activeSelf;
        if (setupFinished && userTurn && !pausedActive)
        {
            //send a missile to the clicked tile
            Vector3 tilepos = Tile.transform.position;
            tilepos.y += 15;
            userTurn = false;
            missilePrefab.gameObject.SetActive(true);
            GameObject instantiatedObject = Instantiate(missilePrefab, tilepos, missilePrefab.transform.rotation);
            Destroy(instantiatedObject, 2.1f);
        }
        else if (!setupFinished)
        {
            PlaceShip(Tile);
            shipScript.SetClickedTile(Tile);
        }
    }

    ////!setupFinshed 
    public void PlaceShip(GameObject Tile)
    {
        if (!pausedCanvas.activeSelf)
        {
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            shipScript.ClearTileList();
            Vector3 moveShipToHere = shipScript.getOffset(Tile.transform.position);
            ships[shipIndex].transform.position = moveShipToHere;
        }
    }

    public void increaseShipIndex()
    {
        if (shipIndex == 4)
        {
            shipIndex = 0;
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            shipScript.FlashColour(Color.yellow);
        }
        else
        {
            shipIndex++;
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            shipScript.FlashColour(Color.yellow);
        }
    }

    public void rotateShip()
    {
        shipScript.rotate();
    }

    public void RecordTilePlacement(Collision collision)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.RecordTilePlacement(collision);
    }

    public void FinishPlacement()
    {
        for (int i = 0; i < ships.Length; i ++ )
        {
            shipScript = ships[i].GetComponent<ShipScript>();
            if (shipScript.OnGameBoard())
            {
                finishedShips++;
            }
        }

        if (finishedShips != 5) 
        {
            for (int i = 0; i < ships.Length; i++)
            {
                shipScript = ships[i].GetComponent<ShipScript>();
                if (!shipScript.OnGameBoard())
                {
                    shipScript.FlashColour(Color.red);
                }
            }
            finishedShips = 0;
        }
        else
        {
            setupFinished = true;
            rotateButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            finishedButton.gameObject.SetActive(false);
            arsenal.gameObject.SetActive(false);

            PlayerShipText.gameObject.SetActive(true);
            PlayerCountText.gameObject.SetActive(true);
            EnemyShipText.gameObject.SetActive(true);
            EnemyCountText.gameObject.SetActive(true);
            MainText.text = "Make a Guess";
            for(int i = 0; i < ships.Length; i++)
            {
                ships[i].SetActive(false);
            }
        }
    }

    ////setupFinshed 
    public void CheckYourGuess(GameObject tile)
    {
        //retrives the integer from the name
        int tileNumber = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitcount = 0;
        foreach(int[] shipArray in enemyShips)
        {
            //if you guessed correctly
            if (shipArray.Contains(tileNumber))
            {
                for (int i = 0; i < shipArray.Length; i++)
                {
                    if (shipArray[i] == tileNumber)
                    {
                        shipArray[i] = -5;
                        hitcount++;
                    }
                    //adds the already hit tiles to the hit count
                    else if (shipArray[i] == -5)
                    {
                        hitcount++;
                    }
                }
                //if the amount of hits on that ship = the length, the ship has sunk
                if (hitcount == shipArray.Length)
                {
                    enemyShipCount--;
                    EnemyCountText.text = enemyShipCount.ToString();
                    Vector3 newtilepos = tile.transform.position;
                    newtilepos.y += 0.5f;
                    enemyFires.Add(Instantiate(firePrefab, newtilepos, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColour(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColours(1);
                    MainText.text = "SUNK!";
                }
                else
                {
                    tile.GetComponent<TileScript>().SetTileColour(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColours(1);
                    MainText.text = "HIT!";
                }
                //break;
            }
        }
        //miss
        if(hitcount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColour(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColours(1);
            MainText.text = "MISS!";
        }
        Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNumber, GameObject hitObject)
    {
        enemygameManager.MissileHit(tileNumber);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (hitObject.GetComponent<ShipScript>().CheckIfSank())
        {
            playerShipCount--;
            PlayerCountText.text = playerShipCount.ToString();
            MainText.text = "SUNK!";
            enemygameManager.SunkPlayer();
        }
        else
        {
            MainText.text = "HIT!";
        }
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(true);
        }
        foreach(GameObject fire in playerFires)
        {
            fire.SetActive(true);
        }
        foreach (GameObject fire in enemyFires)
        {
            fire.SetActive(false);
        }
        ColourAllTiles(0);
        if(enemyShipCount < 1)
        {
            GameOver("YOU WON!");
        }
        userTurn = true;
        enemygameManager.NPCTurn();
        MainText.text = "Enemy's Turn";
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(false);
        }
        foreach (GameObject fire in playerFires)
        {
            fire.SetActive(false);
        }
        foreach (GameObject fire in enemyFires)
        {
            fire.SetActive(true);
        }
        ColourAllTiles(1);
        if (playerShipCount < 1)
        {
            GameOver("YOU LOST!");
        }
        userTurn = true;
        MainText.text = "Make a Guess";
    }

    private void ColourAllTiles(int colourIndex)
    {
        foreach (TileScript tileScript in allTileScript)
        {
            tileScript.SwitchColours(colourIndex);
        }
    }
}
