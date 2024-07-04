using UnityEngine;

public class DisplayBoxController : MonoBehaviour
{

    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;

    [SerializeField] private float rotateDuration;

    private Vector3 startPosition;
    private float targetRotation;
    private float currentRotation;
    private float progress;

    void Start()
    {
        startPosition = transform.position;
        progress = 0f;
    }

    
    float rY= 0;

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        rY = Mathf.Lerp(currentRotation, targetRotation, progress / rotateDuration);
        transform.rotation = Quaternion.Euler(new Vector3(0, rY, 0));
        progress += Time.deltaTime;
    }


    public void RotateDisBox(int idx)
    {
        SoundManager.Instance.CreateAudioSource(transform.position, EffectClip.BOX_TURN);
        currentRotation = rY;
        progress = 0f;
        targetRotation = 90 * (idx - 1);
    }

}
