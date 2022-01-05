using UnityEngine;
[System.Serializable]
public enum RequestType
{
    Death,
    Birth
}

[System.Serializable]
public class Request
{
    public RequestType type;
    public Vector2Int position;

    public Request(RequestType type, Vector2Int pos)
    {
        this.type = type;
        this.position = pos;
    }

}