using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // TODO: Remove
    public class TestTileBase : MonoBehaviour
    {
        public void OnEnter(Entity anEntity) { }
        public void OnExit(Entity anEntity) { }
        public bool CanEnter(Entity anEntity) => true;
    }

    public static StageManager ourInstance;

    [Header("Grid Configuration")]

    [SerializeField]
    [Tooltip("The width of the grid where tiles & entities can be placed!")]
    private int myGridWidth = 10;

    [SerializeField]
    [Tooltip("The height of the grid where tiles & entities can be placed!")]
    private int myGridHeight = 10;

    [SerializeField]
    [Tooltip("The size of one tile in the grid")]
    private float myTileSize = 1.0f;

    // TODO: Change to TileBase
    private TestTileBase[,] myTileGrid;

    private Entity[,] myEntityGrid;

    // Use HashSet for fast iterate, remove and insert (assuming order is irrelevant)
    private HashSet<Entity> myEntities = new HashSet<Entity>();

    private Player myPlayer;

    public Vector3 GetTileCenterWorldPosition(Vector2Int aPosition)
    {
        return new Vector3((aPosition.x + 0.5f) * myTileSize, 0.0f, (aPosition.y + 0.5f) * myTileSize);
    }

    public Vector2Int GetTilePositionFromWorld(Vector3 aPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(aPosition.x / myTileSize), Mathf.FloorToInt(aPosition.z / myTileSize));
    }

    public void MoveEntity(Entity anEntity, Vector2Int aNewPosition)
    {
        Vector2Int gridPosition = GetTilePositionFromWorld(anEntity.transform.position);

        Debug.Assert(CanEntityMoveToPosition(anEntity, aNewPosition), "Entity tried invalid move!");
        Debug.Assert(myEntityGrid[gridPosition.x, gridPosition.y] == anEntity, "Entity position in grid and grid manager not in sync!");

        TestTileBase oldTile = myTileGrid[gridPosition.x, gridPosition.y];
        if (oldTile != null)
        {
            oldTile.OnExit(anEntity);
        }

        myEntityGrid[gridPosition.x, gridPosition.y] = null;
        myEntityGrid[aNewPosition.x, aNewPosition.y] = anEntity;

        TestTileBase newTile = myTileGrid[aNewPosition.x, aNewPosition.y];
        if (newTile != null)
        {
            newTile.OnEnter(anEntity);
        }
    }

    public bool CanEntityMoveToPosition(Entity anEntity, Vector2Int aNewPosition)
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

        Entity entity = GetEntity(aNewPosition);

        // For now we allow moving to tile if there is no entity present!
        return entity == null;
    }

    // TODO: Change TileBase when available!
    public void RegisterTile(TestTileBase aTile)
    {
        Vector2Int gridPosition = GetTilePositionFromWorld(aTile.transform.position);

        EnsureEmptyTile(gridPosition);

        myTileGrid[gridPosition.x, gridPosition.y] = aTile;
    }

    public void RegisterEntity(Entity anEntity)
    {
        Vector2Int gridPosition = GetTilePositionFromWorld(anEntity.transform.position);

        EnsureNoEntity(gridPosition);

        myEntityGrid[gridPosition.x, gridPosition.y] = anEntity;
        myEntities.Add(anEntity);

        // TODO: When Player derives from Entity
        /*
        if (anEntity is Player)
        {
            myPlayer = anEntity as Player;
        }*/
    }

    public TestTileBase GetTile(Vector2Int aPosition)
    {
        EnsurePositionInGrid(aPosition);

        return myTileGrid[aPosition.x, aPosition.y];
    }

    public Entity GetEntity(Vector2Int aPosition)
    {
        EnsurePositionInGrid(aPosition);

        return myEntityGrid[aPosition.x, aPosition.y];
    }

    private IEnumerator TurnMachine()
    {
        // Wait 1 frame to allow all tiles & entities to be registered
        yield return null;

        // Allocate most (all if no reallocations are needed) memory needed by the turn machine at once before we start the main loop
        List<TurnEvent> incompleteNonPlayerTurnEvents = new List<TurnEvent>(myEntities.Count);
        TurnCache turnCache = new TurnCache(myEntities.Count);

        do
        {
            incompleteNonPlayerTurnEvents.Clear();

            // TODO: Uncomment when player derives from Entity
            /*TurnEvent playerTurnEvent = turnCache.Next();
            myPlayer.Action(playerTurnEvent);

            if (!playerTurnEvent.myIsTurnDone)
            {
                yield return playerTurnEvent.myTurnYield;
            }

            turnCache.Recycle(playerTurnEvent);*/

            foreach (Entity entity in myEntities)
            {
                if (entity == myPlayer)
                    continue; // Skip the player (handled above)

                TurnEvent turnEvent = turnCache.Next();
                entity.Action(turnEvent);

                // Optimization
                if (!turnEvent.myIsTurnDone)
                {
                    incompleteNonPlayerTurnEvents.Add(turnEvent);
                }
                else
                {
                    turnCache.Recycle(turnEvent);
                }
            }

            foreach (TurnEvent turnEvent in incompleteNonPlayerTurnEvents)
            {
                if (!turnEvent.myIsTurnDone)
                {
                    yield return turnEvent.myTurnYield;
                }

                turnCache.Recycle(turnEvent);
            }

            yield return null;

            // TODO: Check game should continue (player alive check possibly)
        } while (true);
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

        myTileGrid = new TestTileBase[myGridWidth, myGridHeight];
        myEntityGrid = new Entity[myGridWidth, myGridHeight];
    }

    private void Start()
    {
        StartCoroutine(TurnMachine());
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
