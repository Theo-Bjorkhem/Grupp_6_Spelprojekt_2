using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake_script : MonoBehaviour
{
    [Header("Camera Shake Options")]
    [Tooltip("Duration of Camera Shake (in seconds).")]
    [SerializeField] float myDuration;
    [Tooltip("Maximum length the Camera can move from its original position (in pixels).")]
    [SerializeField] float myMagnitude;

    public IEnumerator CameraShake()
    {
        Vector3 originalPos = transform.localPosition;
        float timeElapsed = 0.0f;

        while(timeElapsed < myDuration)
        {
            float x = Random.Range(-1f, 1f) * myMagnitude;
            float y = Random.Range(-1f, 1f) * myMagnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
