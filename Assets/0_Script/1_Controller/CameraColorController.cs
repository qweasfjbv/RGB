using DG.Tweening;
using UnityEngine;

public class CameraColorController : MonoBehaviour
{
    private float duration = 1.0f;
    private Color targetColor = Color.white;



    public void SetTargetColor(Color tar)
    {
        GetComponent<Camera>().DOColor(tar, duration);
    }

}
