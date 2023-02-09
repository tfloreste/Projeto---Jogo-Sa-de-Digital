using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
	public TextAsset dialogueTextAsset = null;

	public bool hasToPause = true;
	public int lineIndex = 0;
	public bool progressBasedOnDuration = false;
	public bool forceLinearTypingDelay = false;
	public bool forceCloseOnFinish = false;
	public bool ignoreVoiceEffect = false;
	public double timeBetweenVoiceEffect = 0.15;

	private bool clipStarted = false;
	private bool pauseScheduled = false;
	private PlayableDirector director;
	private bool typingCompleted = false;	// verdadeiro quando o di�logo � completo (inclusive quando o jogador pula o di�logo)
	private bool skipedTalking = false;		// verdadeiro quando o jogador pula o di�logo
	private bool closeDialogRegistered = false;
	private bool dialogueClosed = false;
	private double lastTimeVoiceWasPlayed = -1;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(info.weight > 0f && !dialogueClosed)
		{
			// Inicializa os dados da anima��o de di�logo
			if (!clipStarted)
			{

				Dialogue dialogue = new Dialogue(dialogueTextAsset);
				dialogue.SetCurrentLineByIndex(lineIndex);

				DialogueManager.Instance.InitDialogue(dialogue);
				DialogueManager.Instance.PrepareNextLine();
				InputManager.OnTouchStart += SkipTypingAnimation;

				clipStarted = true;
			}

			// Seta o progresso do di�logo baseado nos parametros setados
			if (progressBasedOnDuration || skipedTalking)
			{
				double progress = skipedTalking ? 1.0 : playable.GetTime() / playable.GetDuration();
				DialogueManager.Instance.SetDialogueProgress(progress, forceLinearTypingDelay);

				if (progress >= 1.0)
					typingCompleted = true;
			}
			else
			{
				typingCompleted = DialogueManager.Instance.SetDialogueOnTime(playable.GetTime(), forceLinearTypingDelay);
			}

			// Caso o jogador n�o tenha pulado a anima��o de di�logo
			// remove a fun��o de pular o di�logo e registra a
			// fun��o para fechar o UI
			if(typingCompleted && !closeDialogRegistered)
            {
				Debug.Log("Closing dialogue registered");
				InputManager.OnTouchStart -= SkipTypingAnimation;
				InputManager.OnTouchStart += CloseDialogue;
				closeDialogRegistered = true;
			}

			// Toca o efeito sonoro para simular as vozes dos personagens caso 
			// o di�logo n�o tenha terminado e tenha dado o tempo para isso
			if(	!typingCompleted && !ignoreVoiceEffect 
				&& (lastTimeVoiceWasPlayed < 0 || playable.GetTime() - lastTimeVoiceWasPlayed > timeBetweenVoiceEffect))
            {
				lastTimeVoiceWasPlayed = playable.GetTime();
				DialogueManager.Instance.PlayActorVoice();
            }

			if(Application.isPlaying)
			{
				if(hasToPause)
				{
					// Vari�vel para indicar que a timeline deve
					// pausar no fim do clipe de di�logo
					pauseScheduled = true;
				}
			}

		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if(pauseScheduled && director != null && !dialogueClosed)
		{
			pauseScheduled = false;
			//PauseTimelineUntilTouch();
			director.Pause();
		}
		else if(!pauseScheduled && director && !dialogueClosed)
        {
			InputManager.OnTouchStart -= CloseDialogue;
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
		
		if(forceCloseOnFinish)
			DialogueManager.Instance.CloseDialogue(null);
	}

	private void SkipTypingAnimation()
    {
		Debug.Log("Closing dialogue registered");
		skipedTalking = true;
		typingCompleted = true;
		closeDialogRegistered = true;
		InputManager.OnTouchStart -= SkipTypingAnimation;
		InputManager.OnTouchStart += CloseDialogue;
    }

	private void CloseDialogue()
    {
		dialogueClosed = true;
		//DialogueManager.Instance.CloseDialogue(null);
		DialogueManager.Instance.ExitDialogueMode();
		InputManager.OnTouchStart -= CloseDialogue;
	}

}
