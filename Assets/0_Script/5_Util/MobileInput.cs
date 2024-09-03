using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Class for Modifying Input (Keycode) Mobile(Touch, Swipe) to Keyboard
public class MobileInput : MonoBehaviour
{

    [Header("Swipe Settings")]
    [SerializeField] private float swipeThreshold;


    private KeyCode swipeFlag;
    public KeyCode SwipeFlag { get => swipeFlag; }


    private Vector2 prevTouchPos, curTouchPos;
    private bool isSwiping = false;

    private void Start()
    {
        isSwiping = false;
        prevTouchPos = Vector2.zero;
        curTouchPos = Vector2.zero;
    }

    void Update()
    {
        DetectSwipe();
    }

    // Convert Mobile Input to Keyboar Keycode;
    public KeyCode GetMoblileInput()
    {
        if (SwipeFlag == KeyCode.None)
        {
            return KeyCode.None;
        }

        KeyCode tmpK = swipeFlag;
        swipeFlag = KeyCode.None;
        return tmpK;
    }

    private void DetectSwipe()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);


        switch (touch.phase)
        {
            case TouchPhase.Began:
                prevTouchPos = touch.position;
                isSwiping = true;
                break;

            case TouchPhase.Moved:
                break;

            case TouchPhase.Stationary:     // Hold and Stop
                break;

            case TouchPhase.Ended:
                curTouchPos = touch.position;
                CheckSwipe();
                isSwiping = false;
                break;


        }

    }
    private void CheckSwipe()
    {
        Vector2 dragVector = prevTouchPos - curTouchPos;

        if (dragVector.x >= dragVector.y && dragVector.x > swipeThreshold)
        {
            swipeFlag = (dragVector.x > 0) ? KeyCode.RightArrow : KeyCode.LeftArrow;
        }
        else if(dragVector.x < dragVector.y && dragVector.y > swipeThreshold)
        {
            swipeFlag = (dragVector.y > 0) ? KeyCode.UpArrow : KeyCode.DownArrow;
        }
        else if(dragVector.x < swipeThreshold && dragVector.y < swipeThreshold)
        {
            swipeFlag = KeyCode.Space;
        }
    }
}
