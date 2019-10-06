using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite windSprite;
    public Sprite stoneSprite;
    public Sprite electricitySprite;
    public GameObject[] helperOutlines;

    public int[,] tileSpots;
    public GameObject tilePrefab;
    public Button[] tileButtons;
    public GameObject tileButtonPanel;
    public List<int[]> placedTiles = new List<int[]>{};
    public GameObject spirit;
    public SpriteRenderer spiritTile;
    public GameObject gameOverPanel;

    public AudioClip gameMusic;
    public AudioClip fireSound;
    public AudioClip waterSound;
    public AudioClip windSound;
    public AudioClip electricSound;
    public AudioClip stoneSound;
    public AudioClip incomingWaveSound;
    public AudioClip longWaveSound;

    public bool introComplete = false;
    public bool introDialogueComplete = false;

    public float timeBetweenWaves = 10.0f;
    public float sendWaveDelay = 6.0f;

    bool isGameOver = false;

    Element[] availableElements;
    // Start is called before the first frame update
    void Start()
    { 
        gameOverPanel.SetActive(false);
        spirit.SetActive(false);
        spiritTile.enabled = false;
        tileButtonPanel.SetActive(false);
        tileSpots = new int[100, 100];
        tileSpots[50, 50] = -1;
        GetNewElements();

        Invoke("StartIntro", 1.0f);
    }

    void StartIntro() {
        GetComponent<DialogueController>().DisplayMessages(new string[]
            {"In the beginning there was nothing...", 
            "But when the Life Spirit came, there was hope for something new",
            "With your help, the spirit can bring the elements together to form life"
            });
        Invoke("StartSpirit", 5);
        Invoke("ShowSpiritTile", 15);
    }

    void Update()
    {
    }

    void StartSpirit() {
        spirit.SetActive(true);
    }

    void ShowSpiritTile() {
        spiritTile.enabled = true;
        spirit.GetComponent<Animator>().Play("SpiritToCenter");
        Invoke("StartGame", 2);
    }

    public void GetNewElements() {
        availableElements = new Element[3];
        for (int i = 0; i < 3; i++) {
            Element element = (Element)Random.Range(1, 6);
            availableElements[i] = element;
            switch (element)
            {
                case Element.fire:
                    tileButtons[i].image.sprite = fireSprite;
                    break;
                case Element.water:
                    tileButtons[i].image.sprite = waterSprite;
                    break;
                case Element.air:
                    tileButtons[i].image.sprite = windSprite;
                    break;
                case Element.stone:
                    tileButtons[i].image.sprite = stoneSprite;
                    break;
                case Element.electricity:
                    tileButtons[i].image.sprite = electricitySprite;
                    break;
                default:
                    break;
            }
            tileButtons[i].image.enabled = true;
        }
    }

    public void StartGame() {
        // restart everything
        Camera.main.orthographicSize = 12;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        introComplete = false;
        introDialogueComplete = false;
        gameOverPanel.SetActive(false);
        placedTiles = new List<int[]> { };
        tileSpots = new int[100, 100];
        tileSpots[50, 50] = -1;
        timeBetweenWaves = 9.0f;
        sendWaveDelay = 5.0f;
        spiritTile.GetComponent<Animator>().Play("New State");
        spirit.GetComponent<Animator>().Play("SpiritToCenter");
        foreach (TileController tilecontroller in GameObject.FindObjectsOfType<TileController>())
        {
            Destroy(tilecontroller.gameObject); 
        }
        GetNewElements();

        // start from the beginning with flashing helpers
        foreach (GameObject helper in helperOutlines)
        {
            helper.GetComponent<Animator>().Play("HelpersFlash");
        }
        GetComponent<DialogueController>().DisplayMessages(new string[] {
            "Drag an element to surround the spirit and keep it safe",
            "Each element will resist a particular force of nature",
            "They will also protect surrounding element tiles",
            "If the spirit is unprotected, all life will die",
        });
        tileButtonPanel.SetActive(true);
        Invoke("CompleteIntroDialogue", 23);
        GetComponent<AudioSource>().clip = gameMusic;
        GetComponent<AudioSource>().loop = true; 
        GetComponent<AudioSource>().Play();
    }

    void CompleteIntroDialogue() {
        isGameOver = false;
        introDialogueComplete = true;
        SendFirstWave();
    }

    // Tile spot helper methods

    public Vector2 PositionForSpot(int i, int j) {
        int ii = i - 50;
        int jj = j - 50;
        float yOffset = 0.0f;
        float xOffset = 0.0f;
        if (jj % 2 != 0) {
            xOffset = 1.5f; 
        }
        return new Vector2((ii * 3.0f) + xOffset, (jj * 2.6f) + yOffset);
    }

    public bool SpotHasNeighbor(int i, int j) {
        if (tileSpots[i + 1, j] != 0 || tileSpots[i - 1, j] != 0) { return true; }
        // return true;
        // even col odd row
        if (i % 2 == 0 && j % 2 != 0) {
            if (tileSpots[i, j + 1] != 0 || // upleft
                tileSpots[i, j - 1] != 0 || // downleft
                tileSpots[i + 1, j + 1] != 0 || // upright
                tileSpots[i + 1, j - 1] != 0) // downright
            {
                return true;
            }
            // odd col odd row
        } else if (i % 2 != 0 && j % 2 != 0) {
            if (tileSpots[i + 1, j + 1] != 0 || //upright
                tileSpots[i + 1, j - 1] != 0 || //downright
                tileSpots[i, j + 1] != 0 || //upleft
                tileSpots[i, j - 1] != 0) //downleft
            {
                return true;
            }
            // even col even row
        } else if (i % 2 == 0 && j % 2 == 0) {
            if (tileSpots[i, j + 1] != 0 || //upright
                tileSpots[i, j - 1] != 0 || //downright
                tileSpots[i - 1, j + 1] != 0 || //upleft
                tileSpots[i - 1, j - 1] != 0) //downleft
            {
                return true;
            }
            // odd col even row
        } else if (i % 2 != 0 && j % 2 == 0) {
            if (tileSpots[i + 1, j + 1] != 0 || //upright
                tileSpots[i - 1, j - 1] != 0 || //downright
                tileSpots[i, j + 1] != 0 || //upleft
                tileSpots[i, j - 1] != 0) //downleft
            {
                return true;
            }
        }
        return false;
    }

    public int NeighborCountAtSpot(int i, int j)
    {
        int count = 0;
        if (tileSpots[i + 1, j] != 0) { count++; }
        if (tileSpots[i - 1, j] != 0) { count++; }


        // even col odd row
        if (i % 2 == 0 && j % 2 != 0) {
            if (tileSpots[i, j + 1] != 0) { count ++; }
            if (tileSpots[i, j - 1] != 0) { count ++; }
            if (tileSpots[i + 1, j + 1] != 0) { count++; }
            if (tileSpots[i + 1, j - 1] != 0) { count++; }
            // odd col odd row
        } else if (i % 2 != 0 && j % 2 != 0) {
            if (tileSpots[i + 1, j + 1] != 0) { count++; }
            if (tileSpots[i + 1, j - 1] != 0) { count++; }
            if (tileSpots[i, j + 1] != 0) { count++; }
            if (tileSpots[i, j - 1] != 0) { count++; }
            // even col even row
        } else if (i % 2 == 0 && j % 2 == 0) {
            if (tileSpots[i, j + 1] != 0) { count++; }
            if (tileSpots[i, j - 1] != 0) { count++; }
            if (tileSpots[i - 1, j + 1] != 0) { count++; }
            if (tileSpots[i - 1, j - 1] != 0) { count++; }
            // odd col even row
        } else if (i % 2 != 0 && j % 2 == 0) {
            if (tileSpots[i + 1, j + 1] != 0) { count++; }
            if (tileSpots[i - 1, j - 1] != 0) { count++; }
            if (tileSpots[i, j + 1] != 0) { count++; }
            if (tileSpots[i, j - 1] != 0) { count++; }
        }
        return count;
    }

    public List<int[]> NeighborsAtSpot(int i, int j) {
        List<int[]> neighbors = new List<int[]>{};
        if (tileSpots[i - 1, j] != 0) { neighbors.Add(new int[]{i - 1, j}); }
        if (tileSpots[i + 1, j] != 0) { neighbors.Add(new int[]{i + 1, j}); }

        // even col odd row
        if (i % 2 == 0 && j % 2 != 0) {
            if (tileSpots[i, j + 1] != 0) { neighbors.Add(new int[]{i, j + 1});}
            if (tileSpots[i, j - 1] != 0) { neighbors.Add(new int[]{i, j - 1});}
            if (tileSpots[i + 1, j + 1] != 0) { neighbors.Add(new int[]{i + 1, j + 1});}
            if (tileSpots[i + 1, j - 1] != 0) { neighbors.Add(new int[]{i + 1, j - 1});}
            // odd col odd row
        } else if (i % 2 != 0 && j % 2 != 0) {
            if (tileSpots[i + 1, j + 1] != 0) { neighbors.Add(new int[]{i + 1, j + 1});}
            if (tileSpots[i + 1, j - 1] != 0) { neighbors.Add(new int[]{i + 1, j - 1});}
            if (tileSpots[i, j + 1] != 0) { neighbors.Add(new int[]{i, j + 1});}
            if (tileSpots[i, j - 1] != 0) { neighbors.Add(new int[]{i, j - 1});}
            // even col even row
        } else if (i % 2 == 0 && j % 2 == 0) {
            if (tileSpots[i, j + 1] != 0) { neighbors.Add(new int[]{i, j + 1});}
            if (tileSpots[i, j - 1] != 0) { neighbors.Add(new int[]{i, j - 1});}
            if (tileSpots[i - 1, j + 1] != 0) { neighbors.Add(new int[]{i - 1, j + 1});}
            if (tileSpots[i - 1, j - 1] != 0) { neighbors.Add(new int[]{i - 1, j - 1});}
            // odd col even row
        } else if (i % 2 != 0 && j % 2 == 0) {
            if (tileSpots[i + 1, j + 1] != 0) { neighbors.Add(new int[]{i + 1, j + 1});}
            if (tileSpots[i - 1, j - 1] != 0) { neighbors.Add(new int[]{i - 1, j - 1});}
            if (tileSpots[i, j + 1] != 0) { neighbors.Add(new int[]{i, j + 1});}
            if (tileSpots[i, j - 1] != 0) { neighbors.Add(new int[]{i, j - 1});}
        }
        return neighbors;
    }

    public List<int[]> ExposedSpots() {
        List<int[]> exposedSpots = new List<int[]>{};
        foreach(int[] spot in placedTiles) {
            if (NeighborCountAtSpot(spot[0], spot[1]) < 6) {
                exposedSpots.Add(spot);
            }
        }
        return exposedSpots;
    }

    Button selectedTileButton;
    public void NewTileButtonDown(Button button) {
        Camera.main.GetComponent<CameraController>().canDrag = false;
        selectedTileButton = button;
        button.image.enabled = false;
        TileController newTile = Instantiate(tilePrefab).GetComponent<TileController>();
        newTile.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newTile.gameController = this;
        newTile.selected = true;
        newTile.startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (button == tileButtons[0]) {
            newTile.SetElement(availableElements[0]);
        }
        if (button == tileButtons[1]) {
            newTile.SetElement(availableElements[1]);
        }
        if (button == tileButtons[2]) {
            newTile.SetElement(availableElements[2]);
        }
    }

    public TileController TileAtSpot(int i, int j) {
        foreach (TileController tile in GameObject.FindObjectsOfType<TileController>())
        {
            if ((Vector2)tile.transform.position == PositionForSpot(i, j)) {
                return tile;
            } 
        }
        return null;
    }

    public void ReturnTile() {
        selectedTileButton.image.enabled = true;
    }


    public void WaveHitWithElement(int e) {
        if (isGameOver) { return; }
        GetComponent<AudioSource>().PlayOneShot(incomingWaveSound);
        if (NeighborCountAtSpot(50, 50) < 6) {
            SpiritDestroyed();
            return;
        }
        List<int[]> tilesToRemove = new List<int[]>{};
        Element element = (Element)e;
        Debug.Log("attacking with element: " + element);
        foreach (int[] spot in ExposedSpots()) {
            Element elementAtSpot = (Element)tileSpots[spot[0], spot[1]];
            if (element == ElementHelpers.ElementResistedForElement(elementAtSpot)) {
                Debug.Log("safe because I'm: " + elementAtSpot);
            } else {
                // potentially unsafe, now check neighbors for protection
                bool isProtected = false;
                foreach (int[] neighborSpot in NeighborsAtSpot(spot[0], spot[1]))
                {
                    // protected next to resisting element
                    if (element == ElementHelpers.ElementResistedForElement((Element)tileSpots[neighborSpot[0], neighborSpot[1]])) {
                        isProtected = true; 
                        break;
                    }
                }
                if (!isProtected) {
                    // destroy the tile
                    if (TileAtSpot(spot[0], spot[1]) != null) {
                        tileSpots[spot[0], spot[1]] = 0;
                        // placedTiles.Remove(spot);
                        tilesToRemove.Add(spot);
                        TileAtSpot(spot[0], spot[1]).Eliminated(element);
                    }
                }
            }
        }
        foreach(int[] t in tilesToRemove) {
            placedTiles.Remove(t);
        }
        // destroy any non-protected tiles that are weak to the element
        foreach(int[] spot in placedTiles) {
            Element elementAtSpot = (Element)tileSpots[spot[0], spot[1]];
            // check if weak to the coming element
            if (elementAtSpot == ElementHelpers.ElementResistedForElement(element)) {
                bool isProtected = false;
                foreach (int[] neighborSpot in NeighborsAtSpot(spot[0], spot[1]))
                {
                    // protected next to resisting element
                    if (element == ElementHelpers.ElementResistedForElement((Element)tileSpots[neighborSpot[0], neighborSpot[1]])) {
                        isProtected = true; 
                        break;
                    }
                }
                if (!isProtected) {
                    // destroy the tile
                    if (TileAtSpot(spot[0], spot[1]) != null) {
                        tileSpots[spot[0], spot[1]] = 0;
                        tilesToRemove.Add(spot);
                        TileAtSpot(spot[0], spot[1]).Eliminated(element);
                    }
                }
            }
        }
        foreach (int[] t in tilesToRemove)
        {
            placedTiles.Remove(t);
        }
        Invoke("EliminateLoners", 1.0f);
    }

    public void TilePlaced(int i, int j, Element e) {
        tileSpots[i, j] = (int)e;
        placedTiles.Add(new int[] { i, j });

        if (!introComplete && NeighborCountAtSpot(50, 50) == 6) {
            introComplete = true;
            foreach (GameObject helper in helperOutlines)
            {
                helper.GetComponent<Animator>().Play("HelpersHidden");
            }
        }
        switch (e) {
            case Element.fire:
                GetComponent<AudioSource>().PlayOneShot(fireSound);
                break;
            case Element.water:
                GetComponent<AudioSource>().PlayOneShot(waterSound);
                break;
            case Element.stone:
                GetComponent<AudioSource>().PlayOneShot(stoneSound);
                break;
            case Element.air:
                GetComponent<AudioSource>().PlayOneShot(windSound);
                break;
            case Element.electricity:
                GetComponent<AudioSource>().PlayOneShot(electricSound);
                break;
        }
    }

    void SendFirstWave() {
       GetComponent<DialogueController>().DisplayMessages(new string[] {
           "A wave of FIRE is incoming, place WATERS to protect the spirit!"
       }); 
       Invoke("SendWave", 7);
    }

    Element nextWaveElement = Element.fire;

    void InitiateNextWave() {
        if (isGameOver) { return; }
        nextWaveElement = (Element)Random.Range(1, 6);
        GetComponent<DialogueController>().DisplayMessages(new string[] {
           "A wave of " + ElementHelpers.StringForElement(nextWaveElement) + " is incoming! Protect with " + ElementHelpers.StringForElement(ElementHelpers.ElementWeaknessForElement(nextWaveElement))
        });
        Invoke("SendWave", sendWaveDelay);
        if (sendWaveDelay > 2.5f) {
            sendWaveDelay -= 1.0f;
        }
    }

    void SendWave() {
        if (isGameOver) { return; }
        switch (nextWaveElement) {
            case Element.fire:
                GameObject.Find("FireWave").GetComponent<Animator>().Play("wave");
                break;
            case Element.water:
                GameObject.Find("WaterWave").GetComponent<Animator>().Play("wave");
                break;
            case Element.air:
                GameObject.Find("WindWave").GetComponent<Animator>().Play("wave");
                break;
            case Element.stone:
                GameObject.Find("StoneWave").GetComponent<Animator>().Play("wave");
                break;
            case Element.electricity:
                GameObject.Find("ElectricWave").GetComponent<Animator>().Play("wave");
                break;
        }
        Invoke("PlayWaveSound", 1);
        Invoke("ApplyWaveEffects", 4);
    }

    void PlayWaveSound() {
        if (isGameOver) { return; }
        GetComponent<AudioSource>().PlayOneShot(longWaveSound);
    }

    void ApplyWaveEffects() {
        if (isGameOver) { return; }
        WaveHitWithElement((int)nextWaveElement);
        Invoke("InitiateNextWave", timeBetweenWaves);
        if (timeBetweenWaves > 3) {
            timeBetweenWaves -= 1.1f;
        }
    }

    void EliminateLoners() {
        foreach (int[] spot in ExposedSpots()) {
            if (NeighborCountAtSpot(spot[0], spot[1]) == 0) {
                if (TileAtSpot(spot[0], spot[1]) != null)
                {
                    tileSpots[spot[0], spot[1]] = 0;
                    placedTiles.Remove(spot);
                    TileAtSpot(spot[0], spot[1]).Eliminated(nextWaveElement);
                }
            }
        }
    }

    void SpiritDestroyed() {
        GetComponent<DialogueController>().DisplayMessages(new string[] {
           "You have failed to protect the Life Spirit, it will now fade away to nothingness..."
        });
        spiritTile.GetComponent<Animator>().Play("CenterTileFade");
        spirit.GetComponent<Animator>().Play("SpiritFadeAway");
        Invoke("ShowGameOver", 5.5f);
        isGameOver = true;
        foreach (TileController tilecontroller in GameObject.FindObjectsOfType<TileController>())
        {
            tilecontroller.Eliminated(Element.spirit);
            Destroy(tilecontroller.gameObject); 
        }
    }

    void ShowGameOver() {
        gameOverPanel.SetActive(true);
    }
}
