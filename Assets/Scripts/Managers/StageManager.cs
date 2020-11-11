using UnityEngine;

public class StageManager : MonoBehaviour
{
    // TODO: Remove
    public class TestPlayer: TestEntityBase
    {

    }

    // TODO: Remove
    public class TestEntityBase : MonoBehaviour
    {
        public Vector2Int myPosition;

        public void DoTurn() { }
    }

    // TODO: Remove
    public class TestTileBase : MonoBehaviour
    {
        public Vector2Int myPosition;

        public void OnEnter(TestEntityBase anEntity) { }
        public void OnExit(TestEntityBase anEntity) { }
        public bool CanEnter(TestEntityBase anEntity) => true;
    }

    public static StageManager ourInstance;

    [Header("Grid Configuration")]

    [SerializeField]
    [Tooltip("The width of the grid where tiles & entities can be placed!")]
    private int myGridWidth = 15;

    [SerializeField]
    [Tooltip("The height of the grid where tiles & entities can be placed!")]
    private int myGridHeight = 15;

    [SerializeField]
    [Tooltip("The size of one tile in the grid")]
    private float myTileSize = 1.0f;

    // TODO: Change to TileBase
    private TestTileBase[,] myTiles;

    // TODO: Change to EntityBase
    private TestEntityBase[,] myEntities;

    private TestPlayer myPlayer;

    public Vector3 GetTileCenterWorldPosition(Vector2Int aPosition)
    {
        return new Vector3((aPosition.x + 0.5f) * myTileSize, 0.0f, (aPosition.y + 0.5f) * myTileSize);
    }

    public Vector2Int GetTilePositionFromWorld(Vector3 aPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(aPosition.x / myTileSize), Mathf.FloorToInt(aPosition.z / myTileSize));
    }

    public void MoveEntity(TestEntityBase anEntity, Vector2Int aNewPosition)
    {
        Debug.Assert(CanEntityMoveToPosition(anEntity, aNewPosition), "Entity tried invalid move!");
        Debug.Assert(myEntities[anEntity.myPosition.x, anEntity.myPosition.y] == anEntity, "Entity position in grid and grid manager not in sync!");

        TestTileBase oldTile = myTiles[anEntity.myPosition.x, anEntity.myPosition.y];
        if (oldTile != null)
        {
            oldTile.OnExit(anEntity);
        }

        myEntities[anEntity.myPosition.x, anEntity.myPosition.y] = null;
        myEntities[aNewPosition.x, aNewPosition.y] = anEntity;

        // Keep entity position in sync with grid
        anEntity.myPosition = aNewPosition;

        TestTileBase newTile = myTiles[aNewPosition.x, aNewPosition.y];
        if (newTile != null)
        {
            newTile.OnEnter(anEntity);
        }
    }

    public bool CanEntityMoveToPosition(TestEntityBase anEntity, Vector2Int aNewPosition)
    {
        EnsurePositionInGrid(aNewPosition);

        TestTileBase tile = GetTile(aNewPosition);

        if (tile == null)
        {
            return false;
        }

        if (!tile.CanEnter(anEntity))
        {
            return false;
        }

        TestEntityBase entity = GetEntity(aNewPosition);

        // For now we allow moving to tile if there is no entity present!
        return entity == null;
    }

    // TODO: Change TileBase when available!
    public void RegisterTile(TestTileBase aTile, Vector2Int aPosition)
    {
        EnsureEmptyTile(aPosition);

        myTiles[aPosition.x, aPosition.y] = aTile;
    }

    // TODO: Change EntityBase when available!
    public void RegisterEntity(TestEntityBase anEntity, Vector2Int aPosition)
    {
        EnsureNoEntity(aPosition);

        myEntities[aPosition.x, aPosition.y] = anEntity;

        if (anEntity is TestPlayer)
        {
            myPlayer = anEntity as TestPlayer;
        }
    }

    public TestTileBase GetTile(Vector2Int aPosition)
    {
        EnsurePositionInGrid(aPosition);

        return myTiles[aPosition.x, aPosition.y];
    }

    public TestEntityBase GetEntity(Vector2Int aPosition)
    {
        EnsurePositionInGrid(aPosition);

        return myEntities[aPosition.x, aPosition.y];
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EnsurePositionInGrid(Vector2Int aPosition)
    {
        Debug.Assert(aPosition.x >= 0 && aPosition.x < myGridWidth, "Position not in grid!");
        Debug.Assert(aPosition.y >= 0 && aPosition.y < myGridHeight, "Position not in grid!");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EnsureEmptyTile(Vector2Int aPosition)
    {
        Debug.Assert(GetTile(aPosition) == null, "Tried registering tile at occupied position!");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EnsureNoEntity(Vector2Int aPosition)
    {
        Debug.Assert(GetEntity(aPosition) == null, "Tried registering entity at occupied position!");
    }

    private void AllocateGrid()
    {
        Debug.Assert(myGridWidth > 0 && myGridHeight > 0, "Grid dimensions invalid!");

        myTiles = new TestTileBase[myGridWidth, myGridHeight];
        myEntities = new TestEntityBase[myGridWidth, myGridHeight];
    }

    private void Awake()
    {
        Debug.Assert(ourInstance == null, "Multiple StageManagers loaded!");

        ourInstance = this;

        AllocateGrid();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (myGridWidth <= 0)
        {
            Debug.LogWarning("myGridWidth is invalid!");
        }

        if (myGridHeight <= 0)
        {
            Debug.LogWarning("myGridHeight is invalid!");
        }

        if (myTileSize <= 0.0f)
        {
            Debug.LogWarning("myTileSize is invalid!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (myGridWidth <= 0 || myGridHeight <= 0)
        {
            return;
        }

        for (int x = 0; x < myGridWidth; ++x)
        {
            for (int z = 0; z < myGridHeight; ++z)
            {
                Vector3 start = new Vector3(x * myTileSize, 0.0f, z * myTileSize);

                Gizmos.DrawLine(start, start + Vector3.right * myTileSize);
                Gizmos.DrawLine(start, start + Vector3.forward * myTileSize);
            }
        }

        Vector3 corner = new Vector3(myGridWidth * myTileSize, 0.0f, myGridHeight * myTileSize);

        Gizmos.DrawLine(Vector3.right * corner.x, corner);
        Gizmos.DrawLine(Vector3.forward * corner.z, corner);
    }

#endif
}
