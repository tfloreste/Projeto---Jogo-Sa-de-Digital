using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Actor Provider", menuName = "Dialogue Actor Provider")]
public class DialogueActorProvider : ScriptableObject
{
    [SerializeField] private List<DialogueActor> _dialogueActorList = null;
    private Dictionary<string, DialogueActor> _actorDictionary = null;

    private void Init()
    {
        if (_dialogueActorList == null)
            return;

        _actorDictionary = new Dictionary<string, DialogueActor>();
        foreach(DialogueActor actor in _dialogueActorList)
        {
            _actorDictionary.Add(actor.id, actor);
        }
    }

    public DialogueActor GetActor(string actorId)
    {
        if (_actorDictionary == null)
            Init();

        if (_actorDictionary == null)
        {
            Debug.LogError("Erro ao inicializar o dicionário de personagens");
            return null;
        }
            
        if (_dialogueActorList == null || !_actorDictionary.ContainsKey(actorId))
            return null;

        return _actorDictionary[actorId];
    }
}
