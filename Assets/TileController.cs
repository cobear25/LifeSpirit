using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite windSprite;
    public Sprite stoneSprite;
    public Sprite electricitySprite;

    public bool selected = false;
    private Vector2 grabDif;
    private float touchDownTime;
    private Vector2 touchDownPosition = Vector2.zero;
    int prevSortingOrder = 0;
    public Vector2 startPosition;
    public bool inPlace = false;
    bool returnHome = false;
    public Element element;

    SpriteRenderer sprite;
    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cardPos = transform.position;
        if (selected)
        {
            transform.position = cursorPos - grabDif;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (inPlace) { return; }
            if (sprite.bounds.Contains(cursorPos))
            {
                selected = true;
                Camera.main.GetComponent<CameraController>().canDrag = false;
                prevSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                GetComponent<SpriteRenderer>().sortingOrder = 100;
                grabDif = cursorPos - cardPos;
                touchDownTime = Time.time;
                touchDownPosition = cursorPos;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (selected)
            {
                Invoke("RestoreSortingOrder", 0.2f);
                for (int i = 0; i <= 100; i++) {
                    for (int j = 0; j <= 100; j++)
                    {
                        Vector2 tileSpot = gameController.PositionForSpot(i, j);
                        if (sprite.bounds.Contains(tileSpot) && gameController.tileSpots[i, j] == 0 && gameController.SpotHasNeighbor(i, j)) {
                            transform.position = tileSpot;
                            gameController.tileSpots[i, j] = (int)element;
                            gameController.placedTiles.Add(new int[]{i, j});
                            inPlace = true;
                            gameController.GetNewElements();
                        }
                    }
                }
                if (!inPlace) {
                    returnHome = true;
                }
                selected = false;
                Camera.main.GetComponent<CameraController>().canDrag = true;
            }
        }
        if (returnHome) {
            // move to start position and then destroy
            float distance = Vector2.Distance(transform.position, startPosition);
            transform.position = Vector2.MoveTowards(transform.position, startPosition, 10 * distance * Time.deltaTime);
            Invoke("ReturnHome", 0.2f);
        }
    }

    void ReturnHome() {
        gameController.ReturnTile();
        Destroy(gameObject); 
    }

    void RestoreSortingOrder() {
        GetComponent<SpriteRenderer>().sortingOrder = prevSortingOrder;
    }

    public void SetElement(Element e) {
        element = e;
        switch (element) {
            case Element.fire:
                GetComponent<SpriteRenderer>().sprite = fireSprite;
                break;
            case Element.water:
                GetComponent<SpriteRenderer>().sprite = waterSprite;
                break;
            case Element.air:
                GetComponent<SpriteRenderer>().sprite = windSprite;
                break;
            case Element.stone:
                GetComponent<SpriteRenderer>().sprite = stoneSprite;
                break;
            case Element.electricity:
                GetComponent<SpriteRenderer>().sprite = electricitySprite;
                break;
            default:
                break;
        }
    }

    public void Eliminated() {
        Destroy(gameObject);
    }
}

public enum Element: int
{
    spirit, fire, water, air, stone, electricity
}

public struct ElementHelpers {
    public static Color ColorFor(Element element) {
        switch (element) {
            case Element.spirit:
                return Color.white;
            case Element.fire:
                return Color.red;
            case Element.water:
                return Color.blue;
            case Element.air:
                return Color.cyan;
            case Element.stone:
                return Color.gray;
            case Element.electricity:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    public static Element ElementResistedForElement(Element element) {
        switch (element) {
            case Element.spirit:
                return Element.spirit;
            case Element.fire:
                return Element.air;
            case Element.water:
                return Element.fire;
            case Element.air:
                return Element.water;
            case Element.stone:
                return Element.electricity;
            case Element.electricity:
                return Element.stone;
            default:
                return Element.spirit;
        } 
    }
}
