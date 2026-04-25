using System.Collections;
using UnityEngine;

public class HighlightPart : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    public float duration = 1.5f;
    
    private Renderer[] _renderers;
    private Color[] _originalColors;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].material.color;
    }

    public void Activate()
    {
        StopAllCoroutines();
        StartCoroutine(HighlightRoutine());
    }

    IEnumerator HighlightRoutine()
    {
        foreach (var r in _renderers) r.material.color = highlightColor;
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _originalColors[i];
    }
}