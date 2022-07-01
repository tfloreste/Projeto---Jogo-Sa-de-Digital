using UnityEngine;

[CreateAssetMenu(fileName = "New Actor", menuName = "Dialogue Actor")]
public class DialogueActor : ScriptableObject
{
    public string id;
    public string actorName;
    public AudioClip voiceSoundEffect;

}
