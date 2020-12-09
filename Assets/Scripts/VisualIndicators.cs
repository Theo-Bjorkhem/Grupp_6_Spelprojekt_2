using System.Collections.Generic;
using UnityEngine;

public class VisualIndicators
{
    public class IndicatorData
    {
        public VisualIndicator myIndicator;
    }

    private static readonly int ourIndicatorBufferLength = 8;

    private List<IndicatorData> myUsedIndicators = new List<IndicatorData>(ourIndicatorBufferLength);
    private Queue<IndicatorData> myAvailableIndicators = new Queue<IndicatorData>(ourIndicatorBufferLength);

    private Material myVisualIndicatorMaterial;

    public void Initialize()
    {
        myVisualIndicatorMaterial = Resources.Load<Material>("Materials/visualIndicator_material");
        Debug.Assert(myVisualIndicatorMaterial != null, "Could not load visualIndicator_material at runtime!");

        for (int i = 0; i < ourIndicatorBufferLength; ++i)
        {
            myAvailableIndicators.Enqueue(CreateIndicator());
        }
    }

    public IndicatorData AddNextStepIndicator(Entity aSourceEntity, Vector2Int aCurrentPosition, Vector2Int aNextStepPosition)
    {
        IndicatorData indicatorData = GetNext();

        indicatorData.myIndicator.gameObject.SetActive(true);
        indicatorData.myIndicator.Create(aSourceEntity, aCurrentPosition, aNextStepPosition);

        myUsedIndicators.Add(indicatorData);

        return indicatorData;
    }

    public bool RemoveIndicator(IndicatorData anIndicatorData)
    {
        int index = myUsedIndicators.IndexOf(anIndicatorData);
        if (index >= 0)
        {
            myUsedIndicators.RemoveAt(index);
            Recycle(anIndicatorData);

            return true;
        }

        return false;
    }

    private void Recycle(IndicatorData anIndicatorData)
    {
        anIndicatorData.myIndicator.Reset();
        myAvailableIndicators.Enqueue(anIndicatorData);

        anIndicatorData.myIndicator.gameObject.SetActive(false);
    }

    private IndicatorData GetNext()
    {
        if (myAvailableIndicators.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Not enough available indicators, need to allocate more (might want to increase amount in VisualIndicators)!");
#endif

            return CreateIndicator();
        }

        return myAvailableIndicators.Dequeue();
    }

    private IndicatorData CreateIndicator()
    {
        GameObject indicatorGameObject = new GameObject("VisualIndicator");
        VisualIndicator visualIndicator = indicatorGameObject.AddComponent<VisualIndicator>();
        visualIndicator.Initialize(myVisualIndicatorMaterial, false);

        indicatorGameObject.SetActive(false);

        return new IndicatorData
        {
            myIndicator = visualIndicator,
        };
    }

}
