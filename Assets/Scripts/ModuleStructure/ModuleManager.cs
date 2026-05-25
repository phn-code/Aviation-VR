using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using System.Collections.Generic;

/** 
Central driver class to control the flow of logic on which Modules/sections/timelines/activities should play, and in sequence.

@author Caleb Martin | marcy066@mymail.unisa.edu.au
*/

public class ModuleManager : MonoBehaviour
{
    /**
    Modules are the collections of sections that form learning content within the application. They possess a name, and are made up of a collection of Sections.
    */
    [System.Serializable]
    public class Module
    {
        public string name; /**< The name of the Module. */
        public List<Section> sections; /**< A collection of Sections that make up a Module. */
    }

    /**
    Sections are combinations of timelines and activities that form a Module.
    */
    [System.Serializable]
    public class Section
    {
        public string name;
        public List<PlayableDirector> timelines; /**< A collection of timelines that make up a Section. Each timeline contains animations, subtitles and narration, and signals. */
        public List<ModuleActivities> activities; /**< A collection of activities that make up a Section. Note that for an activity to be paired with a timeline, it MUST be in the same index slot in this List as the relevant timeline in the timelines List (e.g. for an activity to play after the timeline in the [3] index of the timelines List, the activity must also be in the [3] slot of this List). */
    }

    [SerializeField] private ModuleActivityScheduler activityScheduler; /**< Reference to the ModuleActivityScheduler singleton. */
    public List<Module> modules = new List<Module>(); /**< A list of Modules that form the application as a whole. */

    private int currentModuleIndex = 0; /**< The current Module index. */
    private int currentSectionIndex = 0; /**< The current Section index. */
    private int currentTimelineIndex = 0; /**< The current timeline index. */
    private PlayableDirector activeDirector; /**< Reference to the current timeline. */

    private bool waitingForActivity = false; /**< Flag that determines if an activity is currently playing or not, used to yield moving to the next timeline in the Section. */
    private GameObject da40DuplicateCached; /**< Cached reference to DA_40_Duplicate so it can be reactivated even when inactive (GameObject.Find only finds active objects). */
    private GameObject aircraftLabelsCached; /**< Cached reference to AircraftLabels for the same reason. */
    private GameObject da40Cached; /**< Cached reference to DA_40 for position restore after section 5. */
    private Vector3 da40PositionBeforeSection5; /**< DA_40 world position cached before section 5 animates it. */
    private Quaternion da40RotationBeforeSection5; /**< DA_40 world rotation cached before section 5 animates it. */
    
    /// Public property to access the currently active PlayableDirector.
    /// Used by systems that need to sync with the timeline (e.g., haptic feedback).
    public PlayableDirector GetActiveDirector => activeDirector;

    void Start()
    {

    }

    /**
    Plays the timeline in the currently-selected section.
    @return void
    */
    void PlayCurrentTimeline()
    {
        var module = modules[currentModuleIndex];
        var section = module.sections[currentSectionIndex];

        // If there are no timelines in the currently selected section, skip it
        if (section.timelines.Count == 0)
        {
            NextSection();
            return;
        }

        //uses my stoptimeline function
        StopTimeline();

        // Re-enable DA_40_Duplicate before the stall comparison timeline plays.
        // It gets disabled when leaving section 5, but needs to be active when Play() is called
        // otherwise the activation track inside the timeline can't bind to it and the animation freezes.
        if (currentModuleIndex == 1 && currentSectionIndex == 5 && currentTimelineIndex == 1 && da40DuplicateCached != null)
            da40DuplicateCached.SetActive(true);

        // Reassign current director, and play it
        activeDirector = section.timelines[currentTimelineIndex];
        activeDirector.stopped += OnTimelineFinished; // This is a subscription to an event! When the timeline finishes (the timeline being a PlayableDirector) it will automatically call OnTimelineFinished() below
        activeDirector.Play();
    }

    /**
    When a timeline finishes, this method is called to check for activities to determine what to play next.
    @param director Passed here to ensure that an unsubscription to the event is performed, mitigating event stacking.
    @return void
    */
    void OnTimelineFinished(PlayableDirector director)
    {
        if (waitingForActivity) return;

        Debug.Log($"Timeline finished: {director}");
        Debug.Log("waitingForActivity: " + waitingForActivity); //

        director.stopped -= OnTimelineFinished; // This is an unsubscription to an event! This is just to ensure that we don't end up with multiple stacking event listeners. It's good practice

        var section = modules[currentModuleIndex].sections[currentSectionIndex];

        // Check to see if there exists an activity for this timeline
        ModuleActivities activityForTimeline = null;

        if (section.activities != null && currentTimelineIndex < section.activities.Count)
        {
            activityForTimeline = section.activities[currentTimelineIndex];
            Debug.Log("activityForTimeline: " + activityForTimeline);
        }

        if (activityForTimeline != null && !waitingForActivity)
        {
            waitingForActivity = true;
            Debug.Log($"Activity has started: {activityForTimeline}");
            StartActivity(activityForTimeline);
            return;
        }

        // If we get here, there is no activity and we can skip directly to the next timeline
        waitingForActivity = false;
        NextTimeline();
    }

    /**
    Communicate with the ModuleActivityScheduler to initiate a passed-in activity.
    @param activity The activity to be commenced.
    @return void
    */
    void StartActivity(ModuleActivities activity)
    {
        // Skip if no activities are assigned
        if (activity == null)
        {
            OnActivityComplete();
            return;
        }

        // Talk to the activity scheduler and send the activity for this section
        activityScheduler.StartActivity(activity, this);
    }

    /**
    Called by the ModuleActivityScheduler when the current activity is completed, starting the next timeline.
    @return void
    */
    public void OnActivityComplete()
    {
        waitingForActivity = false;
        NextTimeline();
    }

    /**
    Progress to the next timeline, skipping instead to the next Section if there are no more timelines in this Section.
    @return void
    */
    void NextTimeline()
    {
        var section = modules[currentModuleIndex].sections[currentSectionIndex];
        currentTimelineIndex++;

        if (currentTimelineIndex >= section.timelines.Count)
        {
            NextSection();
            return;
        }

        PlayCurrentTimeline();
    }

    /**
    Progress to the next Section, skipping instead to the next Module if there are no more Sections in this Module.
    @return void
    */
    void NextSection()
    {
        currentTimelineIndex = 0;
        currentSectionIndex++;

        var module = modules[currentModuleIndex];
        if (currentSectionIndex >= module.sections.Count)
        {
            NextModule();
            return;
        }

        PlayCurrentTimeline();
    }

    /**
    Progress to the next Module. If there are no more Modules, we have reached the end of the learning content.
    @return void
    */
    void NextModule()
    {
        currentSectionIndex = 0;
        currentTimelineIndex = 0;
        currentModuleIndex++;

        if (currentModuleIndex >= modules.Count)
        {
            // TODO
            // Last module completed.
            return;
        }

        PlayCurrentTimeline();
    }

    //playing specific module and section
    public void PlayModuleSection(int moduleIndex, int sectionIndex)
    {
        // If the pause menu is open when switching sections, force resume so timescale resets
        var pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null) pauseMenu.ForceResume();

        waitingForActivity = false;
        if (activeDirector != null)
            activeDirector.extrapolationMode = DirectorWrapMode.None;

        // This bit of code below is primarily for Section 5 Coordinated vs Uncoordinated animaiton

        // Leaving section 5: restore DA_40 to its pre-section-5 position so it doesn't freeze mid-animation in the next section
        if (currentModuleIndex == 1 && currentSectionIndex == 5 && da40Cached != null)
        {
            da40Cached.transform.position = da40PositionBeforeSection5;
            da40Cached.transform.rotation = da40RotationBeforeSection5;
            da40Cached.SetActive(true);
        }

        // Entering section 5: cache DA_40's current position before the animation moves it
        // This is needed because otherwise when you switch out mid-animation the plane will
        // Stay frozen in it's animation state until that section's timeline animation kicks in
        if (moduleIndex == 1 && sectionIndex == 5)
        {
            if (da40Cached == null) da40Cached = GameObject.Find("DA_40");
            if (da40Cached != null)
            {
                da40PositionBeforeSection5 = da40Cached.transform.position;
                da40RotationBeforeSection5 = da40Cached.transform.rotation;
            }
        }

        StopTimeline();

        // When switching away from section 5, manually hide DA_40_Duplicate and AircraftLabels.
        // Stopping a timeline mid-play doesn't deactivate objects controlled by its activation track,
        // so we have to do it ourselves. We also cache the references here while they're still active
        // since Find won't work on inactive objects later.
        if (currentModuleIndex == 1 && currentSectionIndex == 5)
        {
            var found = GameObject.Find("DA_40_Duplicate");
            if (found != null) da40DuplicateCached = found;
            if (da40DuplicateCached != null) da40DuplicateCached.SetActive(false);

            var foundLabels = GameObject.Find("AircraftLabels");
            if (foundLabels != null) aircraftLabelsCached = foundLabels;
            if (aircraftLabelsCached != null) aircraftLabelsCached.SetActive(false);
        }

        //for checklists for each section to reset
        if (activityScheduler != null)
        {
            activityScheduler.ActivityReset();
        }
        //resets the indexes on moving to next section
        currentModuleIndex = moduleIndex;
        currentSectionIndex = sectionIndex;
        currentTimelineIndex = 0;

        PlayCurrentTimeline();
    }


    /*
    function used to stop timeline when selecting a section from menu (note for future developers activeDirector
    is a reference to the PlayableDirector in a timeline
    */
    private void StopTimeline()
    {
        if (activeDirector != null)
        {
            activeDirector.stopped -= OnTimelineFinished; // unsubscribe before Stop() to prevent OnTimelineFinished call

            activeDirector.Stop();
            activeDirector.time = 0;
        }
    }

}
