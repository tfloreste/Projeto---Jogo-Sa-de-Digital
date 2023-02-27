using UnityEngine;

[CreateAssetMenu(fileName = "New String Variable", menuName = "String Variable")]
public class StringVariable : ScriptableObject
{
    [SerializeField]
    protected string value;

    public virtual string Value { get => value; set => this.value = value; }
}

