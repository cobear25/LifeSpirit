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

    public int[,] tileSpots;
    public GameObject tilePrefab;
    public Button[] tileButtons;
    public GameObject tileButtonPanel;
    public List<int[]> placedTiles = new List<int[]>{};
    public GameObject spirit;
    public SpriteRenderer spiritTile;

    public bool introComplete = false;

    Element[] availableElements;
    // Start is called before the first frame update
    void Start()
    { 
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

    void StartGame() {
        introComplete = true;
        GetComponent<DialogueController>().DisplayMessages(new string[] {
            "Drag an element to surround the spirit and keep it safe",
            "Each element will resist a particular force of nature",
            "They will also protect surrounding element tiles",
            "If the spirit is unprotected, all life will die",
            "A force of nature will not destroy its own element",
            "And a resisting element will protect itself and its neighbors"
        });
        tileButtonPanel.SetActive(true);
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
        Debug.Log("at " + NeighborCountAtSpot(50, 50));
        if (NeighborCountAtSpot(50, 50) < 6) {
            SpiritDestroyed();
            return;
        }
        Element element = (Element)e;
        Debug.Log("attacking with element: " + element);
        foreach (int[] spot in ExposedSpots()) {
            Element elementAtSpot = (Element)tileSpots[spot[0], spot[1]];
            if (elementAtSpot == element || element == ElementHelpers.ElementResistedForElement(elementAtSpot)) {
                Debug.Log("safe because I'm: " + elementAtSpot);
            } else {
                // potentially unsafe, now check neighbors for protection
                bool isProtected = false;
                foreach (int[] neighborSpot in NeighborsAtSpot(spot[0], spot[1]))
                {
                    if (element == ElementHelpers.ElementResistedForElement((Element)tileSpots[neighborSpot[0], neighborSpot[1]])) {
                        isProtected = true; 
                        break;
                    }
                }
                if (!isProtected) {
                    // destroy the tile
                    if (TileAtSpot(spot[0], spot[1]) != null) {
                        tileSpots[spot[0], spot[1]] = 0;
                        placedTiles.Remove(spot);
                        TileAtSpot(spot[0], spot[1]).Eliminated();
                    }
                }
            }
        }
    }

    void SpiritDestroyed() {
        Debug.Log("Game over!");
    }
}
