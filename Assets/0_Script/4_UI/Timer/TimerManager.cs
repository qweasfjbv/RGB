using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : NetworkBehaviour
{
    [Networked] private float timerDuration { get; set; }
    [Networked] private float timerStartTime { get; set; }
    private bool isTimerRunning = false;

    private Coroutine runningCoroutine;

    private int curScore = 0;
    public int CurScore { get { return curScore; } set {  curScore = value; } }
    public void AddScore(PlayerRef player, int score)
    {
        curScore += score;
        GameManagerEx.Instance.UpdateScore(1, curScore);
        RPC_UpdateScoreText(player, curScore);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateScoreText(PlayerRef player, int score)
    {
        if (player == GameManagerEx.Instance.newRunner.LocalPlayer) return;

        GameManagerEx.Instance.UpdateScore(2, score);
    }

    public void StartCounter(float duration)
    {
        if (!isTimerRunning)
        {
            timerDuration = duration;
            timerStartTime = Time.time;
            isTimerRunning = true;
            runningCoroutine = StartCoroutine(UpdateCounterCoroutine());
        }
    }

    // 1초마다 클라이언트의 UI를 업데이트하는 RPC 호출
    private IEnumerator UpdateCounterCoroutine()
    {

        yield return new WaitForSeconds(1f);

        timerStartTime = Time.time;
        while (isTimerRunning)
        {
            float timeLeft = GetTimeLeft();
            RPC_UpdateCounterUI(timeLeft);

            if (timeLeft <= 0)
            {
                isTimerRunning = false;

                // TODO : when timer expired
            }

            yield return new WaitForSeconds(0.1f);
        }

        RPC_UnlockControl();

    }

    private float GetTimeLeft()
    {
        float elapsedTime = Time.time - timerStartTime;
        return Mathf.Max(timerDuration - elapsedTime, 0);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateCounterUI(float timeLeft)
    {
        GameManagerEx.Instance.UpdateCounterUI(Mathf.RoundToInt(timeLeft));
    }

    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UnlockControl()
    {
        Debug.Log("UNLOCK");
        BoxController.UnlockInputBlock();
    }


    public void StartTimer(float duration)
    {
        if (!isTimerRunning)
        {
            timerDuration = duration;
            timerStartTime = Time.time;
            isTimerRunning = true;
            runningCoroutine = StartCoroutine(UpdateTimerCoroutine());
        }
    }

    private IEnumerator UpdateTimerCoroutine()
    {

        yield return new WaitForSeconds(1f);

        timerStartTime = Time.time;
        while (isTimerRunning)
        {
            float timeLeft = GetTimeLeft();
            RPC_UpdateTimerUI(timeLeft);

            if (timeLeft <= 0)
            {
                isTimerRunning = false;

                // TODO : when timer expired
            }

            yield return new WaitForSeconds(0.1f);
        }



    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateTimerUI(float timeLeft)
    {
        GameManagerEx.Instance.UpdateTimerUI(Mathf.RoundToInt(timeLeft));
    }


    public void StopAllCoroutine()
    {
        if (isTimerRunning)
        {
            StopCoroutine(runningCoroutine);
        }
    }
}
