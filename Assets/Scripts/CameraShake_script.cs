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
    private float myTimeElapsed = 0.0f;
    private bool myShake;
    private Vector3 originalPos;

    public void ShakeCamera()
    {
        Debug.Log("In ShakeCamera Script");
        originalPos = transform.position;
        myShake = true;
        myTimeElapsed = 0.0f;
        Debug.Log(myShake);
    }
    private void UpdateThisPlease()
    {
        Debug.Log(myShake);

        if (myShake == true)
        {
            float x = myMagnitude * Time.deltaTime;
            float y = myMagnitude * Time.deltaTime;
            transform.position = new Vector3(Random.insideUnitSphere.x + x, Random.insideUnitSphere.x + y, transform.position.z);
            myTimeElapsed += Time.deltaTime;
            transform.localPosition = originalPos;
            if (myTimeElapsed >= myDuration)
            {
                ResetCamera();
            }
            Debug.Log("Camera Should Shake");
        }

        if (Input.GetKeyDown("p"))
        {
            transform.position = new Vector3(Random.insideUnitSphere.x, Random.insideUnitSphere.x, transform.position.z);
            Debug.Log("Pressed P");
        }
    }
    
    //
    private void ResetCamera()
    {
        myTimeElapsed = 0.0f;
        myShake = false;
    }
}
