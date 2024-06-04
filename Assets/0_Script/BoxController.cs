using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum BoxDir{FORWARD = 0, BOTTOM, BACK, TOP, LEFT, RIGHT}

public class BoxController : MonoBehaviour
{

    [SerializeField] private float jumpDistance = 1.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float jumpDuration = 0.3f;
    [SerializeField] private BoxDir[] boxDirs;

    readonly int[] vertIdx = new int[4] { 3, 0, 1, 2 };
    readonly int[] horzIdx = new int[4] { 3, 5, 1, 4 };

    private bool isJumping = false;
    private Vector3 jumpStart;
    private Vector3 jumpTarget;
    private Vector3 rotateAxis;
    private Quaternion targetRotation;
    private Quaternion startRotation;
    
    private float jumpStartTime;


    private void Start()
    {
        boxDirs = new BoxDir[6] { BoxDir.FORWARD, BoxDir.BOTTOM, BoxDir.BACK, BoxDir.TOP, BoxDir.LEFT, BoxDir.RIGHT };
    }

    private void RotateBox(BoxDir rotateDir)
    {
        // Error Control
        if (rotateDir == BoxDir.TOP || rotateDir == BoxDir.BOTTOM)
        {
            Debug.Log("ERROR : BOXROTATE GET WRONG INPUT"); return;
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
        if (isJumping)  // Jump in progress
        {
            float progress = (Time.time - jumpStartTime) / jumpDuration;
            if (progress < 1.0f)
            {
                // Calculate parabola
                float height = Mathf.Sin(Mathf.PI * progress) * jumpHeight;
                transform.position = Vector3.Lerp(jumpStart, jumpTarget, progress) + new Vector3(0, height, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, progress);
            }
            else
            {
                // Jump Complete
                transform.position = jumpTarget;
                transform.rotation = targetRotation;
                isJumping = false;
            }
        }
        else
        {
            // Get Input Event

            Vector3 direction = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction = Vector3.forward;
                RotateBox(BoxDir.FORWARD);
                rotateAxis = new Vector3(1, 0, 0);

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                direction = Vector3.back;
                RotateBox(BoxDir.FORWARD);
                rotateAxis = new Vector3(-1, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = Vector3.left;
                RotateBox(BoxDir.FORWARD);
                rotateAxis = new Vector3(0, 0, 1);

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = Vector3.right;
                RotateBox(BoxDir.FORWARD);
                rotateAxis = new Vector3(0, 0, -1);
            }


            if (direction != Vector3.zero)
            {
                // Jump Start
                jumpStart = transform.position;
                jumpTarget = jumpStart + direction * jumpDistance;
                jumpTarget.y = jumpStart.y;  // y 좌표는 동일하게 유지
                startRotation = transform.rotation;
                targetRotation = Quaternion.AngleAxis(90, rotateAxis) * startRotation;
                jumpStartTime = Time.time;
                isJumping = true;
            }
        }
    }
}
