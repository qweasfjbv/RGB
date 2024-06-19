using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class BoxController : MonoBehaviour
{
    [Header("Jump Variables")]
    [SerializeField] private float jumpDistance = 1.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float jumpDuration = 0.3f;

    [SerializeField] private float scaleDuration = 0.1f;
    [SerializeField] private float scaleEffect = 0.3f;

    [Space(10)]


    [Header("Input Buffer")]
    [SerializeField, Range(0f, 1f)] private float inputThreshold;
    [Space(10)]

    [Header("Confuse")]
    [SerializeField, Tooltip("DO NOT SELECT BOTTOM, TOP")] private BoxDir forwardDir;
    [Space(10)]

    [Header("DEBUG")]
    [SerializeField] private BoxDir[] boxDirs;
    [SerializeField] private ColorSet[] boxColors;

    // Index sequence to rotate boxDirs
    readonly KeyCode[] arrowKeys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.Space};
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


        jumpDistance = Constant.GRID_SIZE;
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


    public void GetKeyInput(KeyCode key)
    {
        // Key Block during Jumping
        if (isJumping)
        {
            if (jumpProgress > inputThreshold) prevInputBuffer = key;
            return;
        }
        prevInputBuffer = KeyCode.None;
        direction = Vector3.zero;


        if (key == KeyCode.Space)
        {
            //GetComponent<BoxColorController>().TestToggle(boxDirs[(int)BoxDir.BACK]);

            StartCoroutine(StampCoroutine(jumpDuration));
            return;
        }

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

            StartCoroutine(JumpCoroutine(jumpDuration, jumpDistance, jumpHeight));
        }




        return;
    }

    #region GetScaleAxis
    public Vector3 GetScaleYAxis()
    {
        Vector3 tmpV = transform.worldToLocalMatrix * Vector3.up;
        return new Vector3(Mathf.Abs(tmpV.x), Mathf.Abs(tmpV.y), Mathf.Abs(tmpV.z));
    }

    public Vector3 GetScaleXAxis()
    {
        Vector3 tmpV = transform.worldToLocalMatrix * Vector3.right;
        return new Vector3(Mathf.Abs(tmpV.x), Mathf.Abs(tmpV.y), Mathf.Abs(tmpV.z));
    }

    public Vector3 GetScaleZAxis()
    {
        Vector3 tmpV = transform.worldToLocalMatrix * Vector3.forward;
        return new Vector3(Mathf.Abs(tmpV.x), Mathf.Abs(tmpV.y), Mathf.Abs(tmpV.z));
    }
    #endregion

    private IEnumerator JumpCoroutine(float duration, float jDis, float jHei)
    {
        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.H_JUMP);

        isJumping = true;
        jumpProgress = 0f;

        Vector3 scaleAxis = GetScaleYAxis();

        float elapsedTime = 0f;
        float scaleProgress = 0f;

        while (scaleProgress < 1.0f)
        {
            scaleProgress = elapsedTime / scaleDuration;
            float tmp = Mathf.Lerp(0, scaleEffect, scaleProgress);
            transform.localScale = Vector3.one - tmp * scaleAxis;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;


        elapsedTime = 0f;
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

    private IEnumerator StampCoroutine(float duration)
    {
        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.H_JUMP);

        isJumping = true;
        jumpProgress = 0f;

        float elapsedTime = 0f;
        float scaleProgress = 0f;

        Vector3 scaleAxis = GetScaleYAxis();

        Vector3 originalPosition = transform.position;
        Vector3 originalScale = Vector3.one;
        Vector3 stampScale = scaleAxis * scaleEffect + (GetScaleXAxis() +GetScaleZAxis())*1.15f;

        while (scaleProgress < 1.0f)
        {
            scaleProgress = elapsedTime / duration;
            Vector3 tmp = Vector3.Lerp(originalScale, stampScale, scaleProgress);

            // 주축 방향으로 줄이고 다른 축은 약간 늘리기
            transform.localScale = tmp;

            // 위치 조정 - y축 방향으로 이동
            float scaleOffset = MathF.Abs(Vector3.Dot(originalScale, scaleAxis) - Vector3.Dot(transform.localScale, scaleAxis)) / 2;

            
            transform.position = new Vector3(originalPosition.x, originalPosition.y - scaleOffset, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        transform.localPosition = originalPosition;

        isJumping = false;

        // 입력 버퍼가 존재하면 바로 실행
        if (prevInputBuffer != KeyCode.None) GetKeyInput(prevInputBuffer);
    }

}
