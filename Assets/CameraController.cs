using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameController gameController;
    public bool canDrag = true;
    bool selected = false;
    Vector2 positionAtGrab;
    Vector2 cursorPosAtGrab;

    Vector3 distanceFromCenter;
    bool zooming = false;
    Vector2 zoomPoint = Vector2.zero;
    // Update is called once per frame
    void Update()
    {
        if (!gameController.introComplete) {
            return;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel") * 5f;
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Mathf.Abs(scroll) > 0) {
            if (!zooming) {
                zoomPoint = cursorPos;
            }
            zooming = true;
        } else {
            zooming = false;
        }

        if (Camera.main.orthographicSize + scroll > 0 && Camera.main.orthographicSize + scroll < 35) {
            Camera.main.orthographicSize += scroll;
        } 

        if (!canDrag) { return; }
        Vector2 pos = transform.position;
        if (selected)
        {
            Vector2 newPos = (positionAtGrab - (cursorPos - cursorPosAtGrab));
            transform.Translate(new Vector3(newPos.x - distanceFromCenter.x, newPos.y - distanceFromCenter.y, 0), Space.Self);
        }
        if (Input.GetButtonDown("Fire1"))
        {
            selected = true;
            cursorPosAtGrab = cursorPos;
            positionAtGrab = pos;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            cursorPosAtGrab = cursorPos;
            positionAtGrab = pos;
            selected = false;
            distanceFromCenter = transform.position - new Vector3(0, 0, -10);
        }
    }
}
