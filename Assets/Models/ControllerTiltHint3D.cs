using UnityEngine;

public class ControllerTiltHint3D : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float maxTiltAngle = 45f;
    public float tiltSpeed = 1.5f;
    public float holdDuration = 0.5f;

    private float timer = 0f;
    private bool holding = false;
    private float holdTimer = 0f;
    private bool returning = false;
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (!holding && !returning)
        {
            timer += Time.deltaTime * tiltSpeed;
            ApplyRotation(timer);

            if (timer >= 1f)
            {
                timer = 1f;
                holding = true;
                holdTimer = 0f;
            }
        }
        else if (holding)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdDuration)
            {
                holding = false;
                returning = true;
            }
        }
        else if (returning)
        {
            timer -= Time.deltaTime * tiltSpeed;
            ApplyRotation(timer);

            if (timer <= 0f)
            {
                timer = 0f;
                returning = false;
            }
        }
    }

    void ApplyRotation(float t)
    {
        float smoothT = Mathf.SmoothStep(0f, 1f, t);
        float angle = Mathf.Lerp(0f, maxTiltAngle, smoothT);
        transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, angle);
    }

    // public void ShowHint()
    // {
    //     timer = 0f;
    //     holding = false;
    //     returning = false;
    //     gameObject.SetActive(true);
    // }

    // public void HideHint()
    // {
    //     gameObject.SetActive(false);
    // }
}