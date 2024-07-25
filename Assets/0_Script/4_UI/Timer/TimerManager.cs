using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : NetworkBehaviour
{
    [Networked] private float timerDuration { get; set; }
    [Networked] private float timerStartTime { get; set; }
    private bool isTimerRunning = false;



    public void StartTimer(float duration)
    {
        if (!isTimerRunning)
        {
            timerDuration = duration;
            timerStartTime = Time.time;
            isTimerRunning = true;
            StartCoroutine(UpdateTimerCoroutine());
        }
    }

    // 1초마다 클라이언트의 UI를 업데이트하는 RPC 호출
    private IEnumerator UpdateTimerCoroutine()
    {

        yield return new WaitForSeconds(1f);

        timerStartTime = Time.time;
        while (isTimerRunning)
        {
            float timeLeft = GetTimeLeft();

            // 모든 클라이언트의 UI 업데이트
            RPC_UpdateTimerUI(timeLeft);

            if (timeLeft <= 0)
            {
                isTimerRunning = false;
                // 타이머 종료시 추가 작업
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
    public void RPC_UpdateTimerUI(float timeLeft)
    {
        Debug.Log("UPDATE TIMER : +" + timeLeft);
        GameManagerEx.Instance.UpdateTimerUI(Mathf.RoundToInt(timeLeft));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UnlockControl()
    {
        BoxController.UnlockInputBlock();
    }


}
