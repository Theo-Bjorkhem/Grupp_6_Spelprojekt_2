using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //void OnTriggerEnter(Collider col)
    //{
    //    Destroy(col.gameObject);
    //}

    //void OnTriggerEnter()
    //{
    //    Destroy(gameObject);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        // You probably want a check here to make sure you're hitting a zombie
        // Note that this is not the best method for doing so.
        //if (collision.gameObject.tag == "chest")
        //{
        //    Destroy(collision.gameObject);
        //}
        {
            Destroy(collision.gameObject);
        }
    }
}
