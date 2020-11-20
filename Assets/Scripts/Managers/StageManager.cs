﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private class EntityData
    {
        public Vector2Int myGridPosition;
    }

    public static StageManager ourInstance;

    public int myCurrentTurnIndex { get; private set; } = 0;

    public int myTurnsLeft
    {
        get
        {
            return Mathf.Max(myStageTurnCount - myCurrentTurnIndex, 0);
        }
    }

    public StageMesssages myStageMessages { get; private set; } = new StageMesssages();
    private StageState myStageState { get; set; } = new StageState();

    [Header("Grid Configuration")]

    [Tooltip("The width of the grid where tiles & entities can be placed!")]
    public int myGridWidth = 10;

    [Tooltip("The height of the grid where tiles & entities can be placed!")]
    public int myGridHeight = 10;

    [Tooltip("The size of one tile in the grid")]
    public float myTileSize = 1.0f;

    [Header("Game Configuration")]
    
    [Tooltip("Amount of turns the player is allowed to make until the stage is lost")]
    public int myStageTurnCount = 10;

    private float myTileShift = -0.5f;

    private Tile[,] myTileGrid;

    private Entity[,] myEntityGrid;

    private FreezableDictionaryWrapper<Entity, EntityData> myEntities = new FreezableDictionaryWrapper<Entity, EntityData>();

    private FreezableHashSetWrapper<Tile> myTurnEventSubscribedTiles = new FreezableHashSetWrapper<Tile>();

    private FreezableValue<Player> myPlayer = new FreezableValue<Player>(null);

    private CriticalRegion myProtectedTurnRegion = new CriticalRegion();

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
        return new Vector3((aPosition.x + myTileShift) * myTileSize, 0.0f, (aPosition.y + myTileShift) * myTileSize);
    }

    public Vector2Int GetTilePositionFromWorldTile(Vector3 aPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(aPosition.x / myTileSize), Mathf.FloorToInt(aPosition.z / myTileSize));
    }

    public Vector2Int GetTilePositionFromWorldEntity(Vector3 aPosition)
    {
        return new Vector2Int(Mathf.FloorToInt((aPosition.x - myTileShift * myTileSize * 2.0f) / myTileSize), Mathf.FloorToInt((aPosition.z - myTileShift * myTileSize * 2.0f) / myTileSize));
    }

    public Vector2Int GetEntityGridPosition(Entity anEntity)
    {
        return GetEntityData(anEntity).myGridPosition;
    }

    /// <summary>
    /// Moves the specified <paramref name="anEntity"/> to <paramref name="aNewPosition"/>.
    /// Asserts if the move would be illegal by <see cref="CanEntityMoveToPosition(Entity, Vector2Int)"/>.
    /// </summary>
    public void MoveEntity(Entity anEntity, Vector2Int aNewPosition)
    {
        EntityData entityData = GetEntityData(anEntity);

        Vector2Int currentGridPosition = entityData.myGridPosition;

        Debug.Assert(CanEntityMoveToPosition(anEntity, aNewPosition), "Entity tried invalid move!");

        Debug.Assert(myEntityGrid[currentGridPosition.x, currentGridPosition.y] == anEntity, "Entity position in grid and grid manager not in sync!");

        Tile oldTile = myTileGrid[currentGridPosition.x, currentGridPosition.y];
        if (oldTile != null)
        {
            oldTile.OnExit(anEntity);
        }

        myEntityGrid[currentGridPosition.x, currentGridPosition.y] = null;
        myEntityGrid[aNewPosition.x, aNewPosition.y] = anEntity;
        entityData.myGridPosition = aNewPosition;

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

    public void RegisterTileForTurnEvents(Tile aTile)
    {
        myTurnEventSubscribedTiles.Add(aTile);
    }

    public void UnregisterTileForTurnEvents(Tile aTile)
    {
        myTurnEventSubscribedTiles.Remove(aTile);
    }

    public void RegisterTile(Tile aTile)
    {
        Vector2Int gridPosition = GetTilePositionFromWorldTile(aTile.transform.position);

        EnsureEmptyTile(gridPosition);

        myTileGrid[gridPosition.x, gridPosition.y] = aTile;
    }

    public void RegisterEntity(Entity anEntity)
    {
        Vector2Int gridPosition = GetTilePositionFromWorldEntity(anEntity.transform.position);

        EnsureNoEntity(gridPosition);

        myEntityGrid[gridPosition.x, gridPosition.y] = anEntity;
        myEntities.Add(anEntity, new EntityData { myGridPosition = gridPosition });

        if (anEntity is Player)
        {
            myPlayer.Set(anEntity as Player);
        }
    }

    public void UnregisterTile(Tile aTile)
    {
        Vector2Int gridPosition = GetTilePositionFromWorldTile(aTile.transform.position);

        EnsureNoEntity(gridPosition);

        myTileGrid[gridPosition.x, gridPosition.y] = null;

        UnregisterTileForTurnEvents(aTile);
    }

    public void UnregisterEntity(Entity anEntity)
    {
        Vector2Int currentGridPosition = GetEntityGridPosition(anEntity);

        Debug.Assert(myEntityGrid[currentGridPosition.x, currentGridPosition.y] == anEntity, "Entity location not in sync with grid!");

        myEntityGrid[currentGridPosition.x, currentGridPosition.y] = null;

        myEntities.Remove(anEntity);

        if (anEntity is Player)
        {
            myPlayer.Set(null);
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

    private EntityData GetEntityData(Entity anEntity)
    {
        Debug.Assert(myEntities.myValue.ContainsKey(anEntity), "Tried to get data from unregistered entity!");

        return myEntities.myValue[anEntity];
    }

    private void RunQueuedOperations()
    {
        Debug.Assert(!myProtectedTurnRegion.myIsLocked, "Tried running queued operations while entity region is locked!");
    }

    private IEnumerator TurnMachine()
    {
        // Wait 1 frame to allow all tiles & entities to be registered
        yield return null;

#if UNITY_EDITOR
        if (myPlayer.myValue == null)
        {
            Debug.LogWarning("No player registered when turn machine started!");

            yield break;
        }
#endif

        DoEntityTileInitialEnter();

        // Allocate most (all if no reallocations are needed) memory needed by the turn machine at once before we start the main loop
        List<TurnEvent> incompleteNonPlayerTurnEvents = new List<TurnEvent>(myEntities.myValue.Count + myTurnEventSubscribedTiles.myValue.Count);
        TurnCache turnCache = new TurnCache(myEntities.myValue.Count + myTurnEventSubscribedTiles.myValue.Count);

        do
        {
            incompleteNonPlayerTurnEvents.Clear();

            myStageMessages.TriggerTurnStart();

            // Freeze values used in loops etc.
            myEntities.Freeze();
            myTurnEventSubscribedTiles.Freeze();
            myPlayer.Freeze();

            myProtectedTurnRegion.Lock(); // Lock entity region

            // 1. Update player

            TurnEvent playerTurnEvent = turnCache.Next();
            myPlayer.myValue.Action(playerTurnEvent);

            if (!playerTurnEvent.myIsTurnDone)
            {
                yield return playerTurnEvent.myTurnYield;
            }

            turnCache.Recycle(playerTurnEvent);

            // 2. Update all entities

            foreach (Entity entity in myEntities.myValue.Keys)
            {
                if (entity == myPlayer.myValue)
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

            // 3. Update any tiles that have subscribed for turn events

            foreach (Tile tile in myTurnEventSubscribedTiles.myValue)
            {
                TurnEvent turnEvent = turnCache.Next();
                tile.OnTurnEvent(turnEvent);

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

            // 4. Wait for all incomplete non player turn events to finish

            foreach (TurnEvent turnEvent in incompleteNonPlayerTurnEvents)
            {
                if (!turnEvent.myIsTurnDone)
                {
                    yield return turnEvent.myTurnYield;
                }

                turnCache.Recycle(turnEvent);
            }

            myProtectedTurnRegion.Unlock(); // Unlock entity region

            // Unfreeze frozen values
            myPlayer.Unfreeze();
            myTurnEventSubscribedTiles.Unfreeze();
            myEntities.Unfreeze();

            RunQueuedOperations();

            // TODO: Better place?

            // If the current turn resulted in the player unregistering then the player has died => loss
            if (myPlayer.myValue == null)
            {
                OnPlayerLoss();
            }

            yield return null;

            ++myCurrentTurnIndex;
        } while (myStageState.myIsRunning);
    }

    private void DoEntityTileInitialEnter()
    {
        foreach (Entity entity in myEntities.myValue.Keys)
        {
            Vector2Int gridPosition = GetEntityGridPosition(entity);

            Tile tile = GetTile(gridPosition);

            if (tile != null)
            {
                Debug.Assert(tile.CanEnter(entity), "Initial entity location is on tile disallowing enter!");

                tile.OnEnter(entity);
            }
        }
    }

    public bool IsPositionInGrid(Vector2Int aPosition)
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
        if (GetTile(aPosition) != null)
        {
            Debug.LogWarning("Non empty tile: " + aPosition);
        }

        Debug.Assert(GetTile(aPosition) == null, "Tried registering tile at occupied position!");
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EnsureNoEntity(Vector2Int aPosition)
    {
        Debug.Assert(GetEntity(aPosition) == null, "Tried registering entity at occupied position! Click to highlight existing entity.", GetEntity(aPosition));
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

        for (int x = 0; x < myGridWidth; ++x)
        {
            for (int z = 0; z < myGridHeight; ++z)
            {
                Vector3 center = GetTileCenterWorldPosition(new Vector2Int(x, z));
                Vector3 start = center - new Vector3(myTileSize * 0.5f, 0.0f, myTileSize * 0.5f);

                Gizmos.DrawLine(start, start + Vector3.right * myTileSize);
                Gizmos.DrawLine(start, start + Vector3.forward * myTileSize);
            }
        }

        Vector3 shift = new Vector3(myTileSize * 0.5f, 0.0f, myTileSize * 0.5f);
        Vector3 startCorner = GetTileCenterWorldPosition(Vector2Int.zero) - shift;
        Vector3 endCorner = GetTileCenterWorldPosition(new Vector2Int(myGridWidth - 1, myGridHeight - 1)) + shift;

        Gizmos.DrawLine(new Vector3(startCorner.x, 0.0f, endCorner.z), endCorner);
        Gizmos.DrawLine(new Vector3(endCorner.x, 0.0f, startCorner.z), endCorner);
    }

#endif
}
