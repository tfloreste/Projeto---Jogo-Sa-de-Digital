using UnityEngine;

[System.Serializable]
public class DialogueLine 
{
    [SerializeField][TextArea] private string lineText;                   // Texto de dialogo da linha
    [SerializeField] private DialogueCharacter character;       // Guarda informa��es do personagem
    [SerializeField] private BoolVariable lineCondition = null; // Se for vazio ou verdadeiro a linha de dialogo � exibida


    public string GetLineText()
    {
        if (lineCondition != null && !lineCondition.value) 
            return "";

        return lineText;
    }

    public DialogueCharacter GetCharacterData()
    {
        return character;
    }

    
}
