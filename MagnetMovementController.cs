using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetMovementController : MonoBehaviour
{
    [SerializeField] private float speed = .5f;

    Touch touch;
    [SerializeField] private bool isSwiped = false;
    private Vector3 touchStartPosition;
    private Vector3 touchEndPosition;
    private Vector3 nextPosition;
    public float power = 10f;
    [SerializeField] private float minDistanceForSwipe = 200;
    private Rigidbody rb;
    private LineRenderer lr;

    private float xBound;
    private float zBound;
    [SerializeField] private float zLimit;
    private Vector3 gravityToPlayer = Vector3.back / 55; //притяжение к игроку
    


    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        xBound = PlayableAreaController.Instance.XBound;
        zBound = PlayableAreaController.Instance.ZBound;
    }

    private void Update()
    {
        if (MainData.Instance.gameState == MainData.GameState.PlayingAnimation)
        {
            SwipeMagnet();
        }
        if (MainData.Instance.gameState == MainData.GameState.PlayingGame)
        {
            MoveMagnet ();
        }
    }
    private void MoveMagnet()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float touchZ = transform.position.z + (touch.deltaPosition.y * speed * Time.deltaTime);
            float touchX = transform.position.x  + touch.deltaPosition.x * speed * Time.deltaTime;

            zBound = transform.position.z;  // запрет на движение вверх 
            zLimit = zBound;
            if(zBound > zLimit)
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
        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                DragStart();
            }

            //if(touch.phase == TouchPhase.Moved)
            //{
            //    Dragging();
            //}
            if(touch.phase == TouchPhase.Ended)
            {
                DetectSwipe(isSwiped);
            }
        }
    }

    private void DragStart()
    {
        touchStartPosition = touch.position;
        touchStartPosition.z = touchStartPosition.y;
        touchStartPosition.y = 0;
        lr.positionCount = 1;
        lr.SetPosition(0, touchStartPosition);
    }

    private void Dragging()
    {
        //Vector3 touchingPos = touch.position;
        //touchingPos.z = touchingPos.y;
        //touchingPos.y /= 3;
        Vector3 touchingPos = touch.position;
        lr.positionCount = 2;
        lr.SetPosition(1, touchEndPosition);
    }
    private void DetectSwipe(bool isSwiped)
    {
        isSwiped = this.isSwiped;
        //touchEndPosition = touch.position;
        //touchEndPosition.z = touchEndPosition.y;
        //touchEndPosition.y /= 3;
        Debug.Log($"start X = {touchStartPosition.x}, end X = {touchEndPosition.x}");
        Debug.Log($"start Z = {touchStartPosition.z}, end Z = {touchEndPosition.z}");
        Debug.Log($"{VerticalMovementDistance()}, {HorizontalMovementDistance()}");
        if (SwipeDistanceCheck() == true)
        {
            Debug.Log("Pass");
            isSwiped = true;
            DragRelease();
        }
        else
        {
            Debug.Log("Not pass");
            isSwiped = false;
        }

    }

    private bool SwipeDistanceCheck() 
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(touchEndPosition.z - touchStartPosition.z);
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(touchEndPosition.x - touchStartPosition.x);
    }

    private void DragRelease()
    {

        touchEndPosition = touch.position;
        touchEndPosition.z = touchEndPosition.y;
        touchEndPosition.y /= 3;

        //lr.positionCount = 0;

        Vector3 force = (touchEndPosition - touchStartPosition) * power;

        rb.useGravity = true;
        rb.AddForce(force, ForceMode.Impulse);
        //MainData.Instance.gameState = MainData.GameState.PlayingGame;
    }
}
