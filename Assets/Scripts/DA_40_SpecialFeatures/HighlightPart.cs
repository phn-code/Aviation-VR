using System.Collections;
using UnityEngine;

// Causes a GameObject and all of its child renderers to flash a specified highlight colour for a configurable number of pulses.
// Used to draw the user's attention to important aircraft components during tutorial activities.

public class HighlightPart : MonoBehaviour
{
    public Color highlightColor = Color.red;
    public float pulseDuration = 0.3f; // Duration (in seconds) that each highlighted and restored state lasts.
    public int pulseCount = 3;

    private Renderer[] _renderers; // All renderers attached to this GameObject and its children.
    private Color[] _originalColors;

    // Collects all child renderers and stores their original colours when the object is initialised.
    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].material.color;
    }

    public void Activate() // Starts the highlight effect.
    {
        StopAllCoroutines();
        StartCoroutine(HighlightRoutine());
    }

    IEnumerator HighlightRoutine() // Alternates between the highlight colour and the original colours for the configured number of pulses.
    {
        for (int p = 0; p < pulseCount; p++)
        {
            foreach (var r in _renderers) r.material.color = highlightColor;
            yield return new WaitForSeconds(pulseDuration);

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].material.color = _originalColors[i];
            yield return new WaitForSeconds(pulseDuration);
        }
    }
}