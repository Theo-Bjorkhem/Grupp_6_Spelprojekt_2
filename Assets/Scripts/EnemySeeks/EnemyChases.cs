using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChases : MonoBehaviour
{
    public Transform player;
    private Rigidbody rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - transform.position;
        Debug.Log(direction);
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }
}
