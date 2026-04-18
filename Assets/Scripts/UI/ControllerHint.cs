using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerHint : MonoBehaviour
{
    [SerializeField] private Image controllerImage;
    [SerializeField] private float duration = 1.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = controllerImage.rectTransform.localPosition;
        StartCoroutine(HintLoop());
    }

    IEnumerator HintLoop()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(0f, 0.8f, 0.3f));  // Fade in
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float angle = Mathf.Lerp(0f, 90f, t);   // Rotate right (0 to 90 degrees)
                controllerImage.rectTransform.localRotation = Quaternion.Euler(0f, angle, 0f);
             
                if (t > 0.7f)    // Fade out in last 30%
                {
                    float alpha = Mathf.Lerp(0.8f, 0f, (t - 0.7f) / 0.3f);
                    SetAlpha(alpha);
                }

                yield return null;
            }
            controllerImage.rectTransform.localRotation = Quaternion.identity;
            SetAlpha(0f);
            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator Fade(float from, float to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / time));
            yield return null;
        }
    }

    void SetAlpha(float a)
    {
        Color c = controllerImage.color;
        c.a = a;
        controllerImage.color = c;
    }
}