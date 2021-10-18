using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetMovementController : MonoBehaviour
{
    [SerializeField] private float speed = .5f;
    [SerializeField] private float power = 10f;

    [SerializeField] private bool isSwiped = false;
    [SerializeField] private float minDistanceForSwipe = 200;
    [SerializeField] private float distanceBetweenStartandEnd;

    [SerializeField] private float lerpSpeed = .02f;
    private bool isUsingClickbar = false;
    private float lerpMultiplier = 0f;
    private Vector3 lerpNextPos;
    private Vector3 lerpStartPos;
    private Vector3 lerpTargetPos;


    private Vector3 touchStartPosition;
    private Vector3 touchEndPosition;
    private Vector3 nextPosition;
    [SerializeField] private float distanceBetweenPos;

    private LineRenderer lineRenderer;
    private Touch touch;
    private Rigidbody rb;
    [SerializeField] private Camera mainCamera;

    private float xBound;
    private float zBound;


    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        rb = GetComponent<Rigidbody>();
        GetComponent<MagnetCollisionController>().OnCollision += ConfigureLerp;
        lerpTargetPos = transform.position;

        Vector3 boundsVector = PlayableAreaController.Instance.MaxBoundsVector;
        xBound = boundsVector.x;
        zBound = boundsVector.z;
    }

    private void Update()
    {
        if (MainData.Instance.gameState == MainData.GameState.PlayingAnimation)
        {
            SwipeMagnet();
        }

        if (MainData.Instance.gameState == MainData.GameState.PlayingGame)
        {
            if (!isUsingClickbar)
            {
                rb.velocity = Vector3.zero;
                rb.drag = 0f;

                MoveMagnet();
            }
            else
            {
                LerpMagnet();
            }
        }

        if (transform.position.y < .2f)
        {
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);

            if (MainData.Instance.gameState != MainData.GameState.PlayingGame)
            {
                MainData.Instance.gameState = MainData.GameState.PlayingGame;
            }
        }
    }

    private void LerpMagnet()
    {
        if (lerpNextPos != Vector3.zero)
        {
            if (lerpStartPos == Vector3.zero)
            {
                lerpStartPos = transform.position;
            }

            transform.position = Vector3.Lerp(transform.position, lerpNextPos, lerpSpeed * Time.deltaTime);
        }
    }

    public void TickLerp()
    {

        Vector3 delta = lerpNextPos == Vector3.zero 
            ? transform.position - lerpTargetPos 
            : lerpNextPos - lerpTargetPos;
        Debug.Log(delta);
        lerpNextPos = new Vector3(delta.x * lerpMultiplier, transform.position.y, delta.z  * (1-2 * lerpMultiplier));
    }

    public void ConfigureLerp(float value)
    {
        isUsingClickbar = true;
        lerpMultiplier = value;
    }

    private void MoveMagnet()
    {
        float zLimit;
        Vector3 gravityToPlayer = Vector3.back / 55;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float touchZ = transform.position.z + (touch.deltaPosition.y * speed * Time.deltaTime);
            float touchX = transform.position.x + touch.deltaPosition.x * speed * Time.deltaTime;

            zBound = transform.position.z; 
            zLimit = zBound;
            if (zBound > zLimit)
            {
                zBound = zLimit;
            }

            touchX = Mathf.Clamp(touchX, -xBound, xBound);
            touchZ = Mathf.Clamp(touchZ, -zBound, zBound);
            nextPosition.Set(touchX, transform.position.y, touchZ);

            if (touch.phase == TouchPhase.Moved)
            {
                transform.position = nextPosition + gravityToPlayer;
            }
        }
    }

    private void SwipeMagnet()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                DragStart();
            }
            if(touch.phase == TouchPhase.Moved)
            {
                Dragging();
            }
            if (touch.phase == TouchPhase.Ended)
            {
                DetectSwipe(isSwiped);
            }
        }
    }

    private void DragStart()
    {
        lineRenderer.positionCount = 1;
        Vector3 tSPos = Camera.main.ScreenToViewportPoint(touch.position);
        tSPos.z = 0f;
        touchStartPosition = touch.position;
        touchStartPosition.z = touchStartPosition.y;
        touchStartPosition.y = 0;

        lineRenderer.SetPosition(0, tSPos);
    }
    private void Dragging()
    {
        lineRenderer.positionCount = 2;
        Vector3 touchDragPosition = Camera.main.ScreenToViewportPoint(touch.position);
        touchDragPosition.z = 0f;
        //touchDragPosition.z = touch.position.y;
        //touchDragPosition.y = 0;
        lineRenderer.SetPosition(1, touchDragPosition);
    }
    private void DetectSwipe(bool isSwiped)
    {
        Vector3 tEPos = Camera.main.ScreenToViewportPoint(touch.position);
        tEPos.z = 0f;
        isSwiped = this.isSwiped;
        touchEndPosition = touch.position;
        touchEndPosition.z = touch.position.y;
        touchEndPosition.y /= 3;

        lineRenderer.positionCount = 0;

        float angle = Vector3.Angle(touchStartPosition, touchEndPosition);
        Debug.Log(angle);

        //var touchDeltaPositionX = touchEndPosition.x - touchStartPosition.x; 
        //var touchDeltaPositionZ = touchEndPosition.z - touchStartPosition.z;
        //Debug.Log($" startX = {touchStartPosition.x}, endX = {touchEndPosition.x}, distance = {touchDeltaPositionX}");
        //Debug.Log($" startZ = {touchStartPosition.z}, endZ = {touchEndPosition.z}, distance = {touchDeltaPositionZ}");
        distanceBetweenStartandEnd = Vector3.Distance(touchStartPosition, touchEndPosition);
        Debug.Log($"start = {touchStartPosition}, end = {touchEndPosition}, delta = {distanceBetweenStartandEnd}");

        if (distanceBetweenStartandEnd > minDistanceForSwipe)
        {
            isSwiped = true;
            DragRelease();
        }
        else
        {
            isSwiped = false;
        }
    }

    private void DragRelease()
    {

        //touchEndPosition = touch.position;
        //touchEndPosition.z = touchEndPosition.y;
        //touchEndPosition.y /= 3;

        Vector3 force = (touchEndPosition - touchStartPosition) * power;

        rb.useGravity = true;
        rb.AddForce(force, ForceMode.Impulse);
    }
}