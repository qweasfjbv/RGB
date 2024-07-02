using System;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    [SerializeField] private float cameraArmLength;
    [SerializeField, Range(0, 90f)] private float verticalRotate;
    [SerializeField, Range(0, 90f)] private float horizontalRotate;


    private Vector3 camTarget;

    // Calc Spherical to orthogonal coordinate
    private Vector3 CalcOrthoPos(Vector3 target)
    {
        float hAngle = Mathf.Deg2Rad *horizontalRotate;
        float vAngle = Mathf.Deg2Rad * verticalRotate;

        float oz = cameraArmLength * Mathf.Cos(vAngle) * Mathf.Cos(hAngle);
        float oy = cameraArmLength * Mathf.Sin(vAngle);
        float ox = cameraArmLength * Mathf.Cos(vAngle) * Mathf.Sin(hAngle);

        return new Vector3(-ox, oy, -oz);
    }

    public void SetQuaterView(Vector3 target)
    {

        camTarget = target;
        transform.position = target + CalcOrthoPos(target);
        transform.LookAt(target);

        Quaternion cameraRotation = Quaternion.Euler(new Vector3(verticalRotate, horizontalRotate, 0f));
        RenderSettings.skybox.SetMatrix("_Rotation", Matrix4x4.Rotate(Quaternion.Inverse(cameraRotation)));

    }

    private void Update()
    {
        SetQuaterView(camTarget);
    }
}
