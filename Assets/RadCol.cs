using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadCol : MonoBehaviour {
    private bool isEmpty = true;
    public SphereScript Sphere;
    private Collider col;

    private void Start()
    {
        Sphere.RadColUsed = true;
        col = Sphere.transform.parent.gameObject.GetComponent<MeshStructurizer>().rigidbody.GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        if (isEmpty)
        {
            Sphere.RadColUsed = false;
        }
        isEmpty = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (other!=col) isEmpty = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other != col) Sphere.RadColUsed = true;
    }

}
