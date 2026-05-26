using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class BufferingVibrationController : MonoBehaviour
{
    private HapticImpulsePlayer leftHaptic;
    private HapticImpulsePlayer rightHaptic;
    private ModuleManager moduleManager;

    public float vibrationIntensity = 1.0f;
    public float vibrationDuration = 8f;

    // Start time is resolved dynamically from the subtitle track rather than hardcoded.
    // It gets set once when S2_Timeline_2 is first detected.
    private float vibrationStartTime = -1f;
    private bool timelineScanned = false;
    private bool hasTriggered = false;

    private void Start()
    {
        moduleManager = FindObjectOfType<ModuleManager>();
    }

    private void Update()
    {
        if (moduleManager == null) return;

        PlayableDirector currentDirector = moduleManager.GetActiveDirector;
        if (currentDirector == null) return;

        string timelineName = currentDirector.playableAsset != null ? currentDirector.playableAsset.name : "";

        // Only run while S2_Timeline_2 is active
        if (!timelineName.Contains("S2_Timeline_2"))
        {
            // Reset so the scan and trigger happen fresh next time this timeline plays
            if (hasTriggered) hasTriggered = false;
            if (timelineScanned) timelineScanned = false;
            return;
        }

        // Scan the timeline once to find the start time of the "feel buffetting" subtitle clip
        if (!timelineScanned)
        {
            FindBuffettingStartTime(currentDirector);
            timelineScanned = true;
        }

        // If no matching subtitle was found, nothing to trigger
        if (vibrationStartTime < 0f) return;

        if (leftHaptic == null && rightHaptic == null)
            InitializeControllers();

        if (!currentDirector.isActiveAndEnabled) return;

        // Trigger the vibration once when the timeline reaches the buffetting subtitle
        if (!hasTriggered && currentDirector.time >= vibrationStartTime)
        {
            StartCoroutine(SustainedVibration());
            hasTriggered = true;
        }
    }

    // Scans all subtitle tracks in the given timeline for a clip whose text mentions
    // buffetting and caches its start time. This avoids hardcoding the timestamp.
    private void FindBuffettingStartTime(PlayableDirector director)
    {
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null) return;

        foreach (TrackAsset track in timeline.GetOutputTracks())
        {
            foreach (TimelineClip clip in track.GetClips())
            {
                SubtitleClip subtitleClip = clip.asset as SubtitleClip;
                if (subtitleClip != null && subtitleClip.subtitleText.ToLower().Contains("buffett"))
                {
                    vibrationStartTime = (float)clip.start;
                    return;
                }
            }
        }
    }

    // Finds the left and right haptic impulse players from the XR controllers
    private void InitializeControllers()
    {
        HapticImpulsePlayer[] haptics = FindObjectsOfType<HapticImpulsePlayer>();

        foreach (HapticImpulsePlayer haptic in haptics)
        {
            if (haptic.gameObject.name.Contains("Left"))
                leftHaptic = haptic;
            else if (haptic.gameObject.name.Contains("Right"))
                rightHaptic = haptic;
        }

        // Fall back to index order if name-based matching fails
        if (leftHaptic == null && haptics.Length > 0)
            leftHaptic = haptics[0];
        if (rightHaptic == null && haptics.Length > 1)
            rightHaptic = haptics[1];
    }

    // Pulses both controllers repeatedly for vibrationDuration seconds
    private IEnumerator SustainedVibration()
    {
        float elapsed = 0f;
        float pulseDuration = 0.1f;

        while (elapsed < vibrationDuration)
        {
            if (leftHaptic != null)
                leftHaptic.SendHapticImpulse(vibrationIntensity, pulseDuration);
            if (rightHaptic != null)
                rightHaptic.SendHapticImpulse(vibrationIntensity, pulseDuration);
            yield return new WaitForSeconds(pulseDuration);
            elapsed += pulseDuration;
        }
    }
}
