using UnityEngine;

public class VisualIndicator : MonoBehaviour
{
    private Vector2Int myStartPosition;
    private Vector2Int myNextPosition;

    private GameObject myStepIndicatorGameObject;

    public void Initialize(GameObject aStepIndicatorGameObject)
    {
        myStepIndicatorGameObject = aStepIndicatorGameObject;
    }

    public void UpdatePosition(Vector2Int aStartPosition, Vector2Int aNextPosition)
    {
        myStartPosition = aStartPosition;
        myNextPosition = aNextPosition;

        transform.position = StageManager.ourInstance.GetEntityWorldPositionFromTilePosition(myNextPosition);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Build()
    {
        CreateIndicator();
    }

    public void Reset()
    {
        transform.position = Vector3.zero;

        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void CreateIndicator()
    {
        GameObject particleGo = Instantiate(myStepIndicatorGameObject, Vector3.zero, Quaternion.identity);
        particleGo.transform.SetParent(transform, false);
    }
}
