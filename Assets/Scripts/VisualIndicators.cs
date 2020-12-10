using System.Collections.Generic;
using UnityEngine;

public class VisualIndicators
{
    public class IndicatorData
    {
        public VisualIndicator myIndicator;
    }

    private class PooledIndicatorData : PooledObject<IndicatorData, VisualIndicators>
    {
        public PooledIndicatorData(VisualIndicators aRoot, int aCapacity) : base(aRoot, aCapacity)
        {
        }

        protected override IndicatorData PerformCreate()
        {
            GameObject indicatorGameObject = new GameObject("VisualIndicator");
            VisualIndicator visualIndicator = indicatorGameObject.AddComponent<VisualIndicator>();
            visualIndicator.Initialize(StageManager.ourInstance.myStepIndicatorGameObject);

            indicatorGameObject.SetActive(false);

            return new IndicatorData
            {
                myIndicator = visualIndicator,
            };
        }

        protected override void PerformDelete(IndicatorData anObject)
        {
            Object.Destroy(anObject.myIndicator.gameObject);
        }

        protected override void PerformRecycle(IndicatorData anObject)
        {
            anObject.myIndicator.Reset();
            anObject.myIndicator.gameObject.SetActive(false);
        }
    }

    private static readonly int ourIndicatorBufferLength = 8;

    private List<IndicatorData> myUsedIndicators = new List<IndicatorData>(ourIndicatorBufferLength);
    private PooledIndicatorData myAvailableIndicators;

    public void Initialize()
    {
        myAvailableIndicators = new PooledIndicatorData(this, ourIndicatorBufferLength);
    }

    public IndicatorData CreateNextStepIndicator()
    {
        IndicatorData indicatorData = myAvailableIndicators.GetNext();

        indicatorData.myIndicator.gameObject.SetActive(true);
        indicatorData.myIndicator.Build();

        myUsedIndicators.Add(indicatorData);

        return indicatorData;
    }

    public bool RemoveIndicator(IndicatorData anIndicatorData)
    {
        int index = myUsedIndicators.IndexOf(anIndicatorData);
        if (index >= 0)
        {
            myUsedIndicators.RemoveAt(index);
            myAvailableIndicators.Recycle(anIndicatorData);

            return true;
        }

        return false;
    }
}
