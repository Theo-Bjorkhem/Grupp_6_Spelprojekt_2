﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager ourInstance;

    public StageMesssages myStageMessages { get; private set; } = new StageMesssages();
    private StageState myStageState { get; set; } = new StageState();

    [Header("Grid Configuration")]

    [Tooltip("The width of the grid where tiles & entities can be placed!")]
    public int myGridWidth = 10;

    [Tooltip("The height of the grid where tiles & entities can be placed!")]
    public int myGridHeight = 10;

    [Tooltip("The size of one tile in the grid")]
    public float myTileSize = 1.0f;

    private Tile[,] myTileGrid;

    private Entity[,] myEntityGrid;

    // Use HashSet for fast iterate, remove and insert (assuming order is irrelevant)
    private HashSet<Entity> myEntities = new HashSet<Entity>();

    private Player myPlayer;

    /// <summary>
    /// Called when the player wins the current stage. Will start the victory sequence.
    /// </summary>
    public void OnPlayerWon()
    {
        // Stop turn machine
        StopAllCoroutines();

        myStageState.myIsStageWon = true;

        // TODO: Update stage score through GameManager
        GameManager.ourInstance.UpdateStageScore();
        
        myStageMessages.TriggerPlayerWon();

        // Rest will be handled by VictoryDefeatUI (signaled through event)
    }

    /// <summary>
    /// Called when the player loses. Will start the defeated sequence.
    /// </summary>
    public void OnPlayerLoss()
    {
        // Stop turn machine
        StopAllCoroutines();

        myStageState.myIsPlayerAlive = false;
        myStageMessages.TriggerPlayerDefeated();
    }

    public Vector3 GetTileCenterWorldPosition(Vector2Int aPosition)
    {
        return new Vector3(aPosition.x * myTileSize, 0.0f, aPosition.y * myTileSize);
    }

    public Vector2Int GetTilePositionFromWorld(Vector3 aPosition)
    {
        return new Vector2Int(Mathf.FloorToInt((aPosition.x + myTileSize * 0.5f) / myTileSize), Mathf.FloorToInt((aPosition.z + myTileSize * 0.5f) / myTileSize));
    }

    /// <summary>
    /// Moves the specified <paramref name="anEntity"/> to <paramref name="aNewPosition"/>.
    /// Asserts if the move would be illegal by <see cref="CanEntityMoveToPosition(Entity, Vector2Int)"/>.
    /// </summary>
    public void MoveEntity(Entity anEntity, Vector2Int aNewPosition)
    {
        Vector2Int gridPosition = GetTilePositionFromWorld(anEntity.transform.position);

        Debug.Assert(CanEntityMoveToPosition(anEntity, aNewPosition), "Entity tried invalid move!");

        // TODO: Might need a separate field in Entity to keep track of current grid position if this is triggered while an entity is animating to its new location.
        Debug.Assert(myEntityGrid[gridPosition.x, gridPosition.y] == anEntity, "Entity position in grid and grid manager not in sync!");

        Tile oldTile = myTileGrid[gridPosition.x, gridPosition.y];
        if (oldTile != null)
        {
            oldTile.OnExit(anEntity);
        }

        myEntityGrid[gridPosition.x, gridPosition.y] = null;
        myEntityGrid[aNewPosition.x, aNewPosition.y] = anEntity;

        Tile newTile = myTileGrid[aNewPosition.x, aNewPosition.y];
        if (newTile != null)
        {
            newTile.OnEnter(anEntity);
        }
    }

    /// <summary>
    /// Check if the specified <paramref name="anEntity"/> can go to <paramref name="aNewPosition"/>.
    /// </summary>
    public bool CanEntityMoveToPosition(Entity anEntity, Vector2Int aNewPosition)
    {
        if (!IsPositionInGrid(aNewPosition))
        {
            return false;
        }

        Tile tile = GetTile(aNewPosition);

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

    public void RegisterTile(Tile aTile)
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

        if (anEntity is Player)
        {
            myPlayer = anEntity as Player;
        }
    }

    public void UnregisterEntity(Entity anEntity)
    {
        Vector2Int gridPosition = GetTilePositionFromWorld(anEntity.transform.position);

        Debug.Assert(myEntityGrid[gridPosition.x, gridPosition.y] == anEntity, "Entity location not in sync with grid!");

        myEntityGrid[gridPosition.x, gridPosition.y] = null;
        myEntities.Remove(anEntity); // O(1)

        if (anEntity is Player)
        {
            myPlayer = null;
        }
    }

    public Tile GetTile(Vector2Int aPosition)
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

        Debug.Assert(myPlayer != null, "No player has registered itself with the StageManager in time!");

        // Allocate most (all if no reallocations are needed) memory needed by the turn machine at once before we start the main loop
        List<TurnEvent> incompleteNonPlayerTurnEvents = new List<TurnEvent>(myEntities.Count);
        TurnCache turnCache = new TurnCache(myEntities.Count);

        do
        {
            incompleteNonPlayerTurnEvents.Clear();

            TurnEvent playerTurnEvent = turnCache.Next();
            myPlayer.Action(playerTurnEvent);

            if (!playerTurnEvent.myIsTurnDone)
            {
                yield return playerTurnEvent.myTurnYield;
            }

            turnCache.Recycle(playerTurnEvent);

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
        } while (myStageState.myIsRunning);
    }

    private bool IsPositionInGrid(Vector2Int aPosition)
    {
        return aPosition.x >= 0 && aPosition.x < myGridWidth && aPosition.y >= 0 && aPosition.y < myGridHeight;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EnsurePositionInGrid(Vector2Int aPosition)
    {
        Debug.Assert(IsPositionInGrid(aPosition), "Position not in grid!");
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

        myTileGrid = new Tile[myGridWidth, myGridHeight];
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

        Vector3 origin = new Vector3(-0.5f * myTileSize, 0.0f, -0.5f * myTileSize);

        for (int x = 0; x < myGridWidth; ++x)
        {
            for (int z = 0; z < myGridHeight; ++z)
            {
                Vector3 start = new Vector3(x * myTileSize, 0.0f, z * myTileSize) + origin;

                Gizmos.DrawLine(start, start + Vector3.right * myTileSize);
                Gizmos.DrawLine(start, start + Vector3.forward * myTileSize);
            }
        }

        Vector3 corner = origin + new Vector3(myGridWidth * myTileSize, 0.0f, myGridHeight * myTileSize);

        Gizmos.DrawLine(new Vector3(corner.z, 0.0f, origin.z), corner);
        Gizmos.DrawLine(new Vector3(origin.x, 0.0f, corner.z), corner);
    }

#endif
}
