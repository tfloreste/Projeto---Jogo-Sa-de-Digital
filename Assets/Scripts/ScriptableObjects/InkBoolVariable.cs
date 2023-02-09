using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "New Bool Variable Negation", menuName = "Condition/InkBoolVariable")]
public class InkBoolVariable : BoolVariable
{
    [SerializeField] private string inkVariableName;

    [HideInInspector] public override bool Value {
        get
        {
            if (!DialogueManager.Instance)
            {
                Debug.LogWarning("No instance of DialogueManager on the scene");
                return false;
            }

            this.value = DialogueManager.Instance.GetDialogueVariable<bool>(inkVariableName);

            Debug.Log("Value obtained from GetDialogueVariable for inkVariable " + inkVariableName + " was: " + this.value);

            return this.value;
        }
        set {
            Debug.Log("Trying to set " + inkVariableName + " value to: " + value);
            Debug.Log("current value is: " + this.Value);
            Task.Delay(1000).ContinueWith(t => CheckValue());
            
        }
    }

    private void CheckValue()
    {
        Debug.Log("CheckValue fired. Current value is: " + this.Value);
    }
}
