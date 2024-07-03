using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBoxController : MonoBehaviour
{

    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;

    [SerializeField] private float rotateDuration;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

    }


    public void RotateDisBox(int idx)
    {
        transform.DORotate(new Vector3(0, 90 * (idx-1), 0), rotateDuration);
    }

}
