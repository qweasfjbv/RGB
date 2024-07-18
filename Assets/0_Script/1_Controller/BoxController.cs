using DG.Tweening;
using Fusion;
using System;
using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


public class BoxController : NetworkBehaviour
{
    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;

    [SerializeField] private float scaleDuration;
    [SerializeField] private float scaleEffect;

    [SerializeField] private float appearDuration;

    [Space(10)]

    [Header("Input Buffer")]
    [SerializeField, Range(0f, 1f)] private float inputThreshold;
    [Space(10)]

    [Header("Confuse")]
    [SerializeField, Tooltip("DO NOT SELECT BOTTOM, TOP")] private BoxDir forwardDir;
    [Space(10)]

    [Header("DEBUG (READONLY)")]
    [SerializeField] private BoxDir[] boxDirs;
    [SerializeField] private Vector2Int boxPosition;
    [SerializeField] private int boxHeight;



    public BoxDir[] BoxDirs { get { return boxDirs; } }     

    private float jumpDistance = Constant.GRID_SIZE;

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

    private bool isJumping = true;

    private float jumpProgress = 0f;
    private Vector3 jumpStart;
    private Vector3 jumpTarget;
    private Vector3 rotateAxis;
    private Vector3 direction;
    private Quaternion targetRotation;
    private Quaternion startRotation;

    // Previous input buffer
    private KeyCode inputBuffer;
    private bool isInputBlock;

    private void Awake()
    {
        isInputBlock = true;
        //gameObject.SetActive(false);
    }

    private void Start()
    {
        boxDirs = new BoxDir[6] { BoxDir.FORWARD, BoxDir.BOTTOM, BoxDir.BACK, BoxDir.TOP, BoxDir.LEFT, BoxDir.RIGHT };
        direction = Vector3.zero;
        inputBuffer = KeyCode.None;

        jumpTarget = transform.position;
        targetRotation = transform.rotation;
        isJumping = false;
    }

    public void SetBoxController(Vector2Int pos, int height)
    {
        boxPosition = pos;
        boxHeight = height;

        transform.position = new Vector3(pos.x, height, pos.y) * Constant.GRID_SIZE;
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        gameObject.SetActive(true);

        transform.DOScale(new Vector3(1, 1, 1), appearDuration).SetEase(Ease.OutElastic).OnComplete(() => isInputBlock = false);
        return;
    }

    public void UnsetBoxController()
    {
        isInputBlock = true;
        transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), appearDuration).SetEase(Ease.InElastic).OnComplete(() => Destroy(gameObject));
    }

    private void MoveBoxPos(KeyCode k, int dis)
    {
        Vector2Int tmp = Vector2Int.zero;
        switch (k) {
            case KeyCode.UpArrow:
                tmp = new Vector2Int(1, 0); break;
            case KeyCode.DownArrow:
                tmp = new Vector2Int(-1, 0); break;
            case KeyCode.RightArrow:
                tmp = new Vector2Int(0, 1); break;
            case KeyCode.LeftArrow:
                tmp = new Vector2Int(0, -1); break;
        }

        boxPosition += tmp * dis;
    }
    public Vector2Int GetBoxPos()
    {
        return boxPosition;
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


    private KeyCode _pressedKeyCode = KeyCode.None;
    private void Update()
    {
        foreach (KeyCode key in arrowKeys)
        {
            if (Input.GetKey(key))
            {
                _pressedKeyCode = key; return;
            }
        }
    }

    private bool isSynced = false;
    private bool isScaleSynced = false;
    public override void FixedUpdateNetwork()
    {

        if (HasStateAuthority == false) return;

        if (!isScaleSynced)
        {
            transform.localScale = Vector3.one;
            isScaleSynced = true;
        }

        if (!isJumping)
        {
            if (!isSynced)
            {
                transform.position = jumpTarget;
                transform.rotation = targetRotation;
                isSynced = true;
            }

            if (_pressedKeyCode != KeyCode.None)
            {
                isSynced = false;
                RPC_GetKeyInput(_pressedKeyCode);
            }
        }
        
        _pressedKeyCode = KeyCode.None;

    }

    int jDis = 1;
    int hDis = 0; 
    
    public void RPC_GetKeyInput(KeyCode key)
    {
        // Key Block during Jumping
        if (isJumping)
        {
            if (jumpProgress > inputThreshold) inputBuffer = key;
            return;
        }
        inputBuffer = KeyCode.None;
        direction = Vector3.zero;



        Color tmpC = GetComponent<BoxColorController>().GetBlendColorWithFloor();


        forwardDir = BoxDir.FORWARD;
        jDis = 1;
        hDis = 0;
        // Color Func
        switch (tmpC)
        {
            case var _ when tmpC.Equals(ColorConstants.RED):
                forwardDir = BoxDir.RIGHT;
                break;
            case var _ when tmpC.Equals(ColorConstants.BLUE):
                forwardDir = BoxDir.LEFT;
                break;
            case var _ when tmpC.Equals(ColorConstants.YELLOW):
                forwardDir = BoxDir.BACK;
                break;
            case var _ when tmpC.Equals(ColorConstants.ORANGE):
                jDis = 2;
                break;
            case var _ when tmpC.Equals(ColorConstants.GREEN):
                hDis = 1;
                break;
            case var _ when tmpC.Equals(ColorConstants.PURPLE):
                jDis = 0; hDis = 0;
                break;
        }

        if (key == KeyCode.Space)
        {
            StartCoroutine(StampCoroutine(jumpDuration));
            return;
        }

        key = ConfuseDirection(key);
        switch (key) {
            case KeyCode.UpArrow:
                direction = Vector3.forward;
                rotateAxis = new Vector3(1, 0, 0);
                break;

            case KeyCode.DownArrow:
                direction = Vector3.back;
                rotateAxis = new Vector3(-1, 0, 0);
                break;

            case KeyCode.LeftArrow:
                direction = Vector3.left;
                rotateAxis = new Vector3(0, 0, 1);
                break;

            case KeyCode.RightArrow:
                direction = Vector3.right;
                rotateAxis = new Vector3(0, 0, -1);
                break;
        }


        if (direction != Vector3.zero)
        {
            // Jump Start
            jumpStart = new Vector3(boxPosition.y , boxHeight, boxPosition.x) * Constant.GRID_SIZE;

            startRotation = transform.rotation;


            targetRotation = Quaternion.AngleAxis(90, rotateAxis) * startRotation;
            Vector3 euler = targetRotation.eulerAngles;
            euler.x = Mathf.Round(euler.x / 90) * 90;
            euler.y = Mathf.Round(euler.y / 90) * 90;
            euler.z = Mathf.Round(euler.z / 90) * 90;
            targetRotation = Quaternion.Euler(euler);


            JumpBox(key);
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

    private void JumpBox(KeyCode key)
    {

        jumpTarget = jumpStart + direction * jumpDistance * jDis;
        jumpTarget.y = jumpStart.y;

        // Calc Position in advance
        Vector2Int tmp = Vector2Int.zero;
        switch (key)
        {
            case KeyCode.UpArrow:
                tmp = new Vector2Int(1, 0); break;
            case KeyCode.DownArrow:
                tmp = new Vector2Int(-1, 0); break;
            case KeyCode.RightArrow:
                tmp = new Vector2Int(0, 1); break;
            case KeyCode.LeftArrow:
                tmp = new Vector2Int(0, -1); break;
        }

        MapGrid mForGrid = MapGenerator.Instance.GetMapGrid(boxPosition + tmp);
        tmp *= jDis;

        MapGrid mGrid = MapGenerator.Instance.GetMapGrid(boxPosition + tmp);

        // exceptions

        if (jDis == 1)
        {
            if (mGrid == null) // null -> fall
            {
                Debug.Log("mGrid is null");
                MoveBoxPos(key, jDis);
                StartCoroutine(JumpFallCoroutine(jumpDuration)); return;
            }

            if (mGrid.NetworkedGridInfo.Height >= boxHeight + 2) { 
                StartCoroutine(JumpBlockCoroutine(jumpDuration));
                return;
            }

            if (mGrid.NetworkedGridInfo.Height == boxHeight + 1 && hDis <= 0)
            {
                StartCoroutine(JumpBlockCoroutine(jumpDuration));
                return;
            }
        }
        else
        {
            if (mForGrid!= null &&mForGrid.NetworkedGridInfo.Height >= boxHeight + 1)
            {
                StartCoroutine(JumpBlockCoroutine(jumpDuration));
                return;
            }

            if (mGrid == null) // null -> fall
            {
                MoveBoxPos(key, jDis);
                StartCoroutine(JumpFallCoroutine(jumpDuration)); return;
            }
            else if (mGrid.NetworkedGridInfo.Height >= boxHeight + 1)
            {
                StartCoroutine(JumpOneBlockCoroutine(jumpDuration, mForGrid.NetworkedGridInfo.Pos));
                return;
            }
        }

        switch (key)
        {
            case KeyCode.UpArrow:
                RotateBox(BoxDir.FORWARD); break;
            case KeyCode.DownArrow:
                RotateBox(BoxDir.BACK); break;
            case KeyCode.RightArrow:
                RotateBox(BoxDir.RIGHT); break;
            case KeyCode.LeftArrow:
                RotateBox(BoxDir.LEFT); break;
        }

        MoveBoxPos(key, jDis);

        if (mGrid.NetworkedGridInfo.Height == boxHeight + 1 && hDis == 1) // JumpUp or Block
        {
            jumpTarget.y += Constant.GRID_SIZE;
            StartCoroutine(JumpUpDownCoroutine(jumpDuration, true));
            return;
        }
        else if (mGrid.NetworkedGridInfo.Height == boxHeight - 1)
        {
            jumpTarget.y -= Constant.GRID_SIZE;
            StartCoroutine(JumpUpDownCoroutine(jumpDuration, false));
            return;
        }
        else
        { 
            StartCoroutine(JumpCoroutine(jumpDuration));
            return;
        }



    }

    #region JumpCoroutines

    // Jump
    private IEnumerator JumpCoroutine(float duration)
    {

        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.BASIC_JUMP);

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

        isScaleSynced = false;

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

        // JumpComplete


        isJumping = false;

    }

    

    // Jump_1 block up/down
    private IEnumerator JumpUpDownCoroutine(float duration, bool up)
    {

        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.BASIC_JUMP);

        if (up) boxHeight++;
        else boxHeight--;

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

        isScaleSynced = false;


        elapsedTime = 0f;
        float height = 0;
        while (jumpProgress < 1.0f)
        {
            jumpProgress = elapsedTime / duration;
            // Calculate parabola
            if (up) height = Mathf.Sin(5f / 6 * Mathf.PI * jumpProgress) * jumpHeight * 2;
            else
            {
                height = (Mathf.Sin(1f / 6 * Mathf.PI + 5f / 6 * Mathf.PI * jumpProgress) - 0.5f);
                if (height >= 0) height *= 2;
            }
            transform.position = Vector3.Lerp(jumpStart, jumpTarget, jumpProgress) + new Vector3(0, height, 0);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, jumpProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Jump Complete
        if (up) transform.position = jumpTarget + new Vector3(0, Constant.GRID_SIZE, 0);
        else transform.position = jumpTarget - new Vector3(0, Constant.GRID_SIZE, 0);
        transform.rotation = targetRotation;

        isJumping = false;

    }

    // Jump_block by wall
    private IEnumerator JumpBlockCoroutine(float duration)
    {
        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.BASIC_JUMP);

        isJumping = true;
        jumpProgress = 0f;

        Vector3 scaleAxis = GetScaleYAxis();

        float elapsedTime = 0f;
        float scaleProgress = 0f;

        while (scaleProgress < 1.0f)
        {
            scaleProgress = elapsedTime / scaleDuration;
            float tmpf = Mathf.Lerp(0, scaleEffect, scaleProgress);
            transform.localScale = Vector3.one - tmpf * scaleAxis;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isScaleSynced = false;

        elapsedTime = 0f;
        while (jumpProgress < 0.2f)
        {
            jumpProgress = elapsedTime / duration;
            // Calculate parabola
            float height = Mathf.Sin(Mathf.PI * jumpProgress) * jumpHeight;
            transform.position = Vector3.Lerp(jumpStart, jumpTarget, jumpProgress) + new Vector3(0, height, 0);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, jumpProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 tmp = jumpTarget;
        jumpTarget = jumpStart;
        jumpStart = tmp;

        Quaternion tmpQ = targetRotation;
        targetRotation = startRotation;
        startRotation = tmpQ;

        elapsedTime =  duration * 0.8f;

        while (jumpProgress < 1f)
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

    }
    // Jump_block by wall
    private IEnumerator JumpOneBlockCoroutine(float duration, Vector2Int forPos)
    {
        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.BASIC_JUMP);

        isJumping = true;
        jumpProgress = 0f;

        Vector3 scaleAxis = GetScaleYAxis();

        float elapsedTime = 0f;
        float scaleProgress = 0f;

        while (scaleProgress < 1.0f)
        {
            scaleProgress = elapsedTime / scaleDuration;
            float tmpf = Mathf.Lerp(0, scaleEffect, scaleProgress);
            transform.localScale = Vector3.one - tmpf * scaleAxis;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isScaleSynced = false;


        elapsedTime = 0f;
        while (jumpProgress < 0.6f)
        {
            jumpProgress = elapsedTime / duration;
            // Calculate parabola
            float height = Mathf.Sin(Mathf.PI * jumpProgress) * jumpHeight;
            transform.position = Vector3.Lerp(jumpStart, jumpTarget, jumpProgress) + new Vector3(0, height, 0);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, jumpProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 tmp = jumpTarget;
        jumpTarget = jumpStart;
        jumpStart = tmp;

        Quaternion tmpQ = targetRotation;
        targetRotation = startRotation;
        startRotation = tmpQ;

        elapsedTime = duration * 0.4f;

        while (jumpProgress < 1f)
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

    }




    // Jump_fall'in hole
    private IEnumerator JumpFallCoroutine(float duration)
    {

        yield return StartCoroutine(JumpCoroutine(duration));

        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.FALL);


        isJumping = true;
        Vector3 tmpP = transform.position;

        float elapsedTime = 0;
        while (elapsedTime < duration * 3)
        {
            tmpP.y -= Time.deltaTime * 10;
            transform.position = tmpP;

            transform.localScale *= 0.95f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        RPC_GameFail();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_GameFail()
    {
        GameManagerEx.Instance.GameFail();
    }


    // Stamp!
    private IEnumerator StampCoroutine(float duration)
    {
        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.STAMP);

        isJumping = true;
        GetComponent<BoxColorController>().StampColor(boxDirs[(int)BoxDir.BOTTOM]);
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


        elapsedTime = 0f;
        scaleProgress = 0f;

        while (scaleProgress < 1.0f)
        {
            scaleProgress = elapsedTime*3 / duration;
            Vector3 tmp = Vector3.Lerp( stampScale, originalScale, scaleProgress);

            // 주축 방향으로 줄이고 다른 축은 약간 늘리기
            transform.localScale = tmp;

            // 위치 조정 - y축 방향으로 이동
            float scaleOffset = MathF.Abs(Vector3.Dot(originalScale, scaleAxis) - Vector3.Dot(transform.localScale, scaleAxis)) / 2;


            transform.position = new Vector3(originalPosition.x, originalPosition.y - scaleOffset, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isScaleSynced = false;
        transform.localPosition = originalPosition;

        if (MapGenerator.Instance.CheckMapClear() && GetComponent<BoxColorController>().CheckBoxClear())
        {
            GameManagerEx.Instance.GameSuccess();
            isInputBlock = true;
        }

        isJumping = false;

    }

    #endregion

}
