using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
	public TextAsset dialogueTextAsset = null;

	public bool hasToPause = false;
	public int lineIndex = 0;
	public bool progressBasedOnDuration = false;
	public bool forceLinearTypingDelay = false;

	private bool clipStarted = false;
	private bool pauseScheduled = false;
	private PlayableDirector director;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(info.weight > 0f)
		{
			if (!clipStarted)
			{

				Dialogue dialogue = new Dialogue(dialogueTextAsset);
				dialogue.SetCurrentLineByIndex(lineIndex);

				DialogueManager.Instance.InitDialogue(dialogue);
				DialogueManager.Instance.PrepareNextLine();
				DialogueManager.Instance.OpenDialogue(null);
				clipStarted = true;
			}

			if (progressBasedOnDuration)
			{
				double progress = playable.GetTime() / playable.GetDuration();
				DialogueManager.Instance.SetDialogueProgress(progress, forceLinearTypingDelay);
			}
			else
			{
				DialogueManager.Instance.SetDialogueOnTime(playable.GetTime(), forceLinearTypingDelay);
			}

			if(Application.isPlaying)
			{
				if(hasToPause)
				{
					pauseScheduled = true;
				}
			}

		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if(pauseScheduled)
		{
			pauseScheduled = false;
			PauseTimelineUntilTouch();
		}

		clipStarted = false;
	}

	private void PauseTimelineUntilTouch()
	{
		director.playableGraph.GetRootPlayable(0).SetSpeed(0d);
		InputManager.OnTouchStart += ResumeTimelineOnTouchEvent;
	}

	private void ResumeTimelineOnTouchEvent()
    {
		director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
		InputManager.OnTouchStart -= ResumeTimelineOnTouchEvent;
		DialogueManager.Instance.CloseDialogue(null);

	}

}
