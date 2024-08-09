using System;
using System.Collections.Generic;
using UnityEngine;


public class Test_CamController : MonoBehaviour
{

    [SerializeField] private float cameraArmLength;
    [SerializeField, Range(0, 90f)] private float verticalRotate;
    [SerializeField, Range(0, 90f)] private float horizontalRotate;
    [SerializeField] private Vector3 target;

    private Vector3 camTarget;


    #region Singleton
    private static Test_CamController instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public static Test_CamController Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }

    #endregion


    private void Start()
    {
        Test_Spawn();
        /*
        var camera = GetComponent<Camera>();
        var r = camera.rect;
        var scaleH = ((float)Screen.width / Screen.height) / (9f / 16f);
        var scaleW = 1f / scaleH;

        if (scaleH < 1f)
        {
            r.height = scaleH;
            r.y = (1f - scaleH) / 2f;
        }
        else
        {
            r.width = scaleW;
            r.x = (1f - scaleW) / 2f;
        }

        camera.rect = r;
        UnsetCamera();
        */
    }

    private void OnPreCull()
    {
        //GL.Clear(true, true, Color.black);
    }


    // Calc Spherical to orthogonal coordinate
    private Vector3 CalcOrthoPos()
    {
        float hAngle = Mathf.Deg2Rad * horizontalRotate;
        float vAngle = Mathf.Deg2Rad * verticalRotate;

        float oz = cameraArmLength * Mathf.Cos(vAngle) * Mathf.Cos(hAngle);
        float oy = cameraArmLength * Mathf.Sin(vAngle);
        float ox = cameraArmLength * Mathf.Cos(vAngle) * Mathf.Sin(hAngle);

        return new Vector3(-ox, oy, -oz);
    }

    private void Update()
    {
        SetQuaterView();
    }

    public void SetQuaterView()
    {
        camTarget = target;
        transform.position = target + CalcOrthoPos();
        transform.LookAt(target);
        Quaternion cameraRotation = Quaternion.identity;
        cameraRotation = Quaternion.Euler(new Vector3(verticalRotate, horizontalRotate, 0f));
        RenderSettings.skybox.SetMatrix("_Rotation", Matrix4x4.Rotate(Quaternion.Inverse(cameraRotation)));
    }

    public void UnsetCamera()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        RenderSettings.skybox.SetMatrix("_Rotation", Matrix4x4.Rotate(Quaternion.Inverse(Quaternion.identity)));

    }

    [Header("Test")]
    public GameObject floorPrefab;

    public List<MeshRenderer> renderers =  new List<MeshRenderer>();

    public List<Texture2D> circles = new List<Texture2D>();

    public void Test_Spawn()
    {
        renderers.Add(Instantiate(floorPrefab).GetComponent<MeshRenderer>());
        renderers[0].transform.localScale = Vector3.one;
        renderers[0].transform.position = new Vector3(0,0, 1);
        renderers[0].material.mainTexture = circles[0];
        renderers.Add(Instantiate(floorPrefab).GetComponent<MeshRenderer>());
        renderers[1].transform.localScale = Vector3.one;
        renderers[1].transform.position = new Vector3(0, 0, 0);
        renderers[1].material.mainTexture = circles[1];

        renderers.Add(Instantiate(floorPrefab).GetComponent<MeshRenderer>());
        renderers[2].transform.localScale = Vector3.one;
        renderers[2].transform.position = new Vector3(1, 0, 0);
        renderers[2].material.mainTexture = circles[2];

        renderers.Add(Instantiate(floorPrefab).GetComponent<MeshRenderer>());
        renderers[3].transform.localScale = Vector3.one;
        renderers[3].transform.position = new Vector3(1, 0, 1);
        renderers[3].material.color = ColorConstants.BLACK;
    }


}
