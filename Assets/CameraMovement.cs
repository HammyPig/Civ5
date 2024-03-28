using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    // In Civ 5, you can move the camera by using either your arrow keys, clicking and dragging, or edge scrolling.
    // While clicking and dragging, you cannot start edge scrolling, but you can start using arrow keys if your mouse is not moving (still pressed down).
    // While edge scrolling, you cannot click and drag, but arrow keys completely override and cancel edge scrolling (even after arrow keys unpressed).
    // While using arrow keys, you can start clicking and dragging, but arrow keys resume if the mouse stops moving. You can edge scroll unless it is in
    // the opposite direction - even after arrow keys are unpressed.
    // In this system, we will simply prioritise arrow keys, then drag, then edge scrolling.

    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private int edgeSize = 30;
    [SerializeField] private float minZoom = 10f / 2f;
    [SerializeField] private float maxZoom = 10f * 2f;

    private Camera cam;
    private Vector3 moveDirection;
    private Vector3 mousePositionOrigin = Vector3.zero;

    void Start() {
        cam = gameObject.GetComponent<Camera>();
    }

    void Update() {
        if (IsUsingArrowKeys()) HandleKeyMovement();
        else if (IsDragging()) HandleDragMovement();
        else if (IsEdgeScrolling()) HandleEdgeMovement();

        mousePositionOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        
        HandleZoom();
    }

    private bool IsUsingArrowKeys() {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    private bool IsDragging() {
        return Input.GetMouseButton(0);
    }

    private bool IsEdgeScrolling() {
        return Input.mousePosition.x < edgeSize || Input.mousePosition.y < edgeSize || Input.mousePosition.x >= Screen.width - edgeSize || Input.mousePosition.y >= Screen.height - edgeSize;
    }

    private void HandleKeyMovement() {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection.y += 1f;
        if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1f;
        if (Input.GetKey(KeyCode.S)) moveDirection.y -= 1f;
        if (Input.GetKey(KeyCode.D)) moveDirection.x += 1f;

        cam.transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void HandleDragMovement() {
        moveDirection = mousePositionOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position += moveDirection;
    }

    private void HandleEdgeMovement() {
        moveDirection = Vector3.zero;

        if (Input.mousePosition.x < edgeSize) moveDirection.x -= 1f;
        if (Input.mousePosition.y < edgeSize) moveDirection.y -= 1f;
        if (Input.mousePosition.x >= Screen.width - edgeSize) moveDirection.x += 1f;
        if (Input.mousePosition.y >= Screen.height - edgeSize) moveDirection.y += 1f;

        cam.transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void HandleZoom() {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y, minZoom, maxZoom);

        moveDirection = mousePositionOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position += moveDirection;
    }
}
