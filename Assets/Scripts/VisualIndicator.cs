using UnityEngine;

public class VisualIndicator : MonoBehaviour
{
    private Entity mySourceEntity;
    private Vector2Int myStartPosition;
    private Vector2Int myNextPosition;

    private Material myMaterial;

    private bool myUseModel = false;

    public void Initialize(Material aMaterial, bool aUseModel)
    {
        myMaterial = aMaterial;
        myUseModel = aUseModel;
    }

    public void Create(Entity aSourceEntity, Vector2Int aStartPosition, Vector2Int aNextPosition)
    {
        mySourceEntity = aSourceEntity;
        myStartPosition = aStartPosition;
        myNextPosition = aNextPosition;

        Build();
    }

    public void UpdatePosition(Vector2Int aStartPosition, Vector2Int aNextPosition)
    {
        myStartPosition = aStartPosition;
        myNextPosition = aNextPosition;

        transform.position = StageManager.ourInstance.GetEntityWorldPositionFromTilePosition(myNextPosition);
    }

    public void Reset()
    {
        mySourceEntity = null;
        transform.position = Vector3.zero;

        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void CreateModel()
    {
        GameObject sourceCopy = Instantiate(mySourceEntity.gameObject);
        sourceCopy.transform.SetParent(transform, false);
        sourceCopy.transform.localPosition = Vector3.zero;

        sourceCopy.GetComponent<Entity>().enabled = false;

        MeshRenderer meshRenderer = sourceCopy.GetComponentInChildren<MeshRenderer>();
        Debug.Assert(meshRenderer != null, "No MeshRenderer in copy!");

        meshRenderer.material = myMaterial;
    }

    private void CreateCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(transform, false);
        cube.transform.localPosition = Vector3.up * 0.1f * 0.5f - new Vector3(1.0f, 0.0f, 1.0f) * 0.5f * StageManager.ourInstance.myTileSize;
        cube.transform.localScale = new Vector3(StageManager.ourInstance.myTileSize, 0.1f, StageManager.ourInstance.myTileSize);

        MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = myMaterial;
    }

    private void Build()
    {
        transform.position = StageManager.ourInstance.GetEntityWorldPositionFromTilePosition(myNextPosition);

        if (myUseModel)
        {
            CreateModel();
        }
        else
        {
            CreateCube();
        }
    }
}
