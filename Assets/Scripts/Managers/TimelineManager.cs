using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    public PlayableDirector playableDirector; // Reference to the PlayableDirector


    private void PauseTimeline()
    {
        // Pause the timeline
        if (playableDirector != null)
        {
            playableDirector.Pause();
        }
    }

    private void ResumeTimeline()
    {
        // Resume the timeline
        if (playableDirector != null)
        {
            playableDirector.Resume();
        }
    }
}
