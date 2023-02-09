using UnityEngine;

[CreateAssetMenu(fileName = "New Bool Variable Negation", menuName = "Condition/Negation")]
public class BoolVariableNegation : BoolVariable
{
    [SerializeField] private BoolVariable negatedVariable;

    public override bool Value { get => !negatedVariable.Value; set => negatedVariable.Value = !value; }
}
