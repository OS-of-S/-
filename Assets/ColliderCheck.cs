using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour {
    public SphereScript Sphere;
    private Collider col;

    private void Start()
    {
        col = Sphere.transform.parent.gameObject.GetComponent<MeshStructurizer>().rigidbody.GetComponent<Collider>();
        Sphere.TrigerZoneUsed = true;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Worked " + other);
        if (other == col) Sphere.TrigerZoneUsed = true;
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Worked " + other);
        if (other == col) Sphere.TrigerZoneUsed = false;
    }
}
