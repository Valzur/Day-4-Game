using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // For once in my life, i do not have my game manager as a singleton <3
    [Tooltip("Time is in milli-seconds")]
    [SerializeField] int timeStepSize;
    [SerializeField] Vector2Int gridSize = new Vector2Int(100,100);
    [SerializeField] Transform cellsParent;
    [SerializeField] CellController cellPrefab;
    Queue<Request> requests = new Queue<Request>();
    CellController[,] cells;
    [SerializeField] Camera camera;
    int _timeStepsElapsed = 0;
    int timeStepsElapsed
    {
        get{return _timeStepsElapsed;}
        set{ _timeStepsElapsed = value; UIManager.Instance.UpdateTimeSteps(value);}
    }
    Vector2Int[] neighbourPositions =
    {
       Vector2Int.up,
       new Vector2Int(1,1),
       Vector2Int.right,
       new Vector2Int(1,-1),
       Vector2Int.down,
       new Vector2Int(-1,-1),
       Vector2Int.left,
       new Vector2Int(-1,1)
    };
    
    bool isSimulating = false;
    void Start() => GenerateCells();

    public void StartSimulation()
    {
        requests.Clear();
        // Initiate Loop
        ChangeCellInteractability(false);
        isSimulating = true;
        SimulateTimestep();
    }

    public void StopSimulation()
    {
        timeStepsElapsed = 0;
        ChangeCellInteractability(true);
        isSimulating = false;
    }

    public void CleanGrid()
    {
        foreach (var cell in cells)
        {
            cell.IsOn = false;
        }
    }

    async void SimulateTimestep()
    {
        timeStepsElapsed ++;
        if (!isSimulating)
            return;

        Execute();
        ComputeNext();

        await Task.Delay(timeStepSize);
        SimulateTimestep();
    }

    void Execute()
    {
        Request currentRequest;
        while(requests.Count > 0)
        {
            currentRequest = requests.Dequeue();
            CellController currentCell = cells[currentRequest.position.x, currentRequest.position.y];
            currentCell.IsOn = currentRequest.type == RequestType.Death? false : true;
        }
    }

    CellController[] GetNeighbours(Vector2Int pos)
    {
        List<CellController> neighbours = new List<CellController>();
        foreach (var neighbourPos in neighbourPositions)
        {
            Vector2Int correctedPos = CorrectPosition(pos + neighbourPos);
            CellController neighbourCell = cells[correctedPos.x, correctedPos.y];
            neighbours.Add(neighbourCell);
        }
        return neighbours.ToArray();
    }

    Vector2Int CorrectPosition(Vector2Int pos)
    {
        Vector2Int correctedPos = pos;
        if(correctedPos.x >= gridSize.x) correctedPos.x = 0;
        if(correctedPos.x < 0) correctedPos.x = gridSize.x - 1;
        if(correctedPos.y >= gridSize.y) correctedPos.y = 0;
        if(correctedPos.y < 0) correctedPos.y = gridSize.y - 1;
        return correctedPos;
    }

    int GetNumberOfAliveNeighbours(CellController[] neighbours)
    {
        int no = 0;
        foreach (var neighbour in neighbours)
        {
            if(neighbour.IsOn)
                no++;
        }
        return no;
    }
    
    void ComputeNext()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int cellPos = new Vector2Int(x,y);
                CellController cell = cells[x, y];
                CellController[] cellNeighbours = GetNeighbours(cellPos);
                int aliveNeighbours = GetNumberOfAliveNeighbours(cellNeighbours);
                if(cell.IsOn)
                {
                    // Alive
                    if(aliveNeighbours != 2 && aliveNeighbours != 3)
                        requests.Enqueue(new Request(RequestType.Death, cellPos));
                }
                else
                {
                    // Dead or non existent
                    if(aliveNeighbours == 3)
                        requests.Enqueue(new Request(RequestType.Birth, cellPos));
                }
            }
        }
    }

    void GenerateCells()
    {
        // Clear previous
        if (cells != null)
        {
            foreach (var item in cells)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
        }

        cells = new CellController[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                cells[x, y] = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity, cellsParent);
            }
        }
        
        // Setup Cam
        camera.transform.position = new Vector3( (gridSize.x -1)/(float)2.0, (gridSize.y-1)/(float)2.0, -2);
        camera.orthographicSize = (float)(0.7) * gridSize.y;
    }

    void ChangeCellInteractability(bool interactable)
    {
        foreach (var cell in cells)
            cell.isInteractable = interactable;
    }
}