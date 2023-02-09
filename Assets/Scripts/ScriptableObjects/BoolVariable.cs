using UnityEngine;

[CreateAssetMenu(fileName = "New Bool Variable", menuName = "Condition/Bool Variable")]
public class BoolVariable : ScriptableObject
{
    [SerializeField]
    protected bool value;

    public virtual bool Value { get => value; set => this.value = value; }
}
