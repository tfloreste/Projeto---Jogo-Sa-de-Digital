using UnityEngine;

[CreateAssetMenu(fileName = "New Starting Position Data", menuName = "Starting Position Data")]
public class StartingPositionData : ScriptableObject
{
    [SerializeField] private string gameObjectName = "";
    [SerializeField] private Vector3 vector3Position = Vector3.zero;
    [SerializeField] private Direction direction;


    public Vector3 GetPosition()
    {
        if (vector3Position.sqrMagnitude > 0)
            return vector3Position;

        if (gameObjectName != "")
        {
            GameObject target = GameObject.Find(gameObjectName);
            if (target)
                return target.transform.position;
        }

        return Vector3.zero;
    }


    public void SetPosition(string name)
    {
        gameObjectName = name;
    }

    public void SetPosition(Vector3 position)
    {
        vector3Position = position;
    }


    public Direction GetDirection()
    {
        return direction;
    }

    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
    }

}
