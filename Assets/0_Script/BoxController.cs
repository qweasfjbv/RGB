using System;
using System.Collections;
using UnityEngine;

public enum BoxDir{FORWARD = 0, BOTTOM, BACK, TOP, LEFT, RIGHT}

public class BoxController : MonoBehaviour
{
    [Header("Jump Variables")]
    [SerializeField] private float jumpDistance = 1.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float jumpDuration = 0.3f;
    [Space(10)]

    [Header("Input Buffer")]
    [SerializeField, Range(0f, 1f)] private float inputThreshold;
    [Space(10)]

    [Header("Confuse")]
    [SerializeField, Tooltip("DO NOT SELECT BOTTOM, TOP")] private BoxDir forwardDir;
    [Space(10)]

    [Header("DEBUG")]
    [SerializeField] private BoxDir[] boxDirs;

    // Index sequence to rotate boxDirs
    readonly KeyCode[] arrowKeys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow };
    readonly int[] vertIdx = new int[4] { 3, 0, 1, 2 };
    readonly int[] horzIdx = new int[4] { 3, 5, 1, 4 };
    readonly int[,] confuseIdx = new int[6, 4] {
        { 0, 1, 2, 3 },
        { 0, 1, 2, 3 },
        { 1, 0, 3, 2 },
        { 0, 1, 2, 3 },
        { 3, 2, 0, 1 },
        { 2, 3, 1, 0 } };


    private float jumpProgress = 0f;
    private bool isJumping;
    private Vector3 jumpStart;
    private Vector3 jumpTarget;
    private Vector3 rotateAxis;
    private Vector3 direction;
    private Quaternion targetRotation;
    private Quaternion startRotation;

    // Previous input buffer
    private KeyCode prevInputBuffer;

    private void Start()
    {
        boxDirs = new BoxDir[6] { BoxDir.FORWARD, BoxDir.BOTTOM, BoxDir.BACK, BoxDir.TOP, BoxDir.LEFT, BoxDir.RIGHT };

        direction = Vector3.zero;
        prevInputBuffer = KeyCode.None;
        isJumping = false;
    }

    private KeyCode ConfuseDirection(KeyCode dir)
    {
        return arrowKeys[confuseIdx[(int)forwardDir, (int)dir - (int)KeyCode.UpArrow]];
    }

    private void RotateBox(BoxDir rotateDir)
    {
        // Error Control
        if (rotateDir == BoxDir.TOP || rotateDir == BoxDir.BOTTOM)
        {
            Debug.LogError("ERROR : BOXROTATE GET WRONG INPUT"); return;
        }

        // Direction Update
        BoxDir tmpDir;
        switch (rotateDir) {

            case BoxDir.FORWARD:
                tmpDir = boxDirs[vertIdx[3]];
                for (int i = 3; i > 0; i--)
                    boxDirs[vertIdx[i]] = boxDirs[vertIdx[(i - 1) < 0 ? 3 : i - 1]];
                boxDirs[vertIdx[0]] = tmpDir;
                break;

            case BoxDir.BACK:
                tmpDir = boxDirs[vertIdx[0]];
                for (int i = 0; i < 3; i++)
                    boxDirs[vertIdx[i]] = boxDirs[vertIdx[(i + 1) % 4]];
                boxDirs[vertIdx[3]] = tmpDir;
                break;

            case BoxDir.RIGHT:
                tmpDir = boxDirs[horzIdx[3]];
                for (int i = 3; i > 0; i--)
                    boxDirs[horzIdx[i]] = boxDirs[horzIdx[(i - 1) < 0 ? 3 : i - 1]];
                boxDirs[horzIdx[0]] = tmpDir;
                break;

            case BoxDir.LEFT:
                tmpDir = boxDirs[horzIdx[0]];
                for (int i = 0; i < 3; i++)
                    boxDirs[horzIdx[i]] = boxDirs[horzIdx[(i + 1) % 4]];
                boxDirs[horzIdx[3]] = tmpDir;
                break;

        }

        return;

    }


    void Update()
    {

        foreach (KeyCode key in arrowKeys)
        {
            if (Input.GetKey(key))
            {
                GetKeyInput(key); return;
            }
        }

    }


    private void GetKeyInput(KeyCode key)
    {
        // Key Block during Jumping
        if (isJumping)
        {
            if (jumpProgress > inputThreshold) prevInputBuffer = key;
            return;
        }

        prevInputBuffer = KeyCode.None;
        direction = Vector3.zero;

        key = ConfuseDirection(key);
        switch (key) {
            case KeyCode.UpArrow:
                direction = Vector3.forward;
                RotateBox(BoxDir.FORWARD);
                rotateAxis = new Vector3(1, 0, 0);
                break;

            case KeyCode.DownArrow:
                direction = Vector3.back;
                RotateBox(BoxDir.BACK);
                rotateAxis = new Vector3(-1, 0, 0);
                break;

            case KeyCode.LeftArrow:
                direction = Vector3.left;
                RotateBox(BoxDir.LEFT);
                rotateAxis = new Vector3(0, 0, 1);
                break;

            case KeyCode.RightArrow:
                direction = Vector3.right;
                RotateBox(BoxDir.RIGHT);
                rotateAxis = new Vector3(0, 0, -1);
                break;
        }


        if (direction != Vector3.zero)
        {
            // Jump Start
            jumpStart = transform.position;
            jumpTarget = jumpStart + direction * jumpDistance;
            jumpTarget.y = jumpStart.y;

            startRotation = transform.rotation;
            targetRotation = Quaternion.AngleAxis(90, rotateAxis) * startRotation;

            StartCoroutine(JumpCoroutine(jumpDuration));
        }


        return;
    }

    private IEnumerator JumpCoroutine(float duration)
    {
        jumpProgress = 0f;
        isJumping = true;
        float elapsedTime = 0f;

        while (jumpProgress < 1.0f)
        {
            jumpProgress = elapsedTime / duration;
            // Calculate parabola
            float height = Mathf.Sin(Mathf.PI * jumpProgress) * jumpHeight;
            transform.position = Vector3.Lerp(jumpStart, jumpTarget, jumpProgress) + new Vector3(0, height, 0);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, jumpProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Jump Complete
        transform.position = jumpTarget;
        transform.rotation = targetRotation;
        isJumping = false;

        // If inputBuffer exist -> direct execute
        if (prevInputBuffer != KeyCode.None) GetKeyInput(prevInputBuffer);
    }


}
