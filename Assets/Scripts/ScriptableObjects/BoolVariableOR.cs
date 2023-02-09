using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bool Variable Negation", menuName = "Condition/OR")]
public class BoolVariableOR : BoolVariable
{
    [SerializeField] private BoolVariable[] variables;

    public override bool Value
    {
        get {
            if (variables == null || variables.Length == 0)
                return false;

            foreach(BoolVariable variable in variables)
            {
                if (!variable.Value)
                    return false;
            }

            return true;
        }
        set { }
    }
}
