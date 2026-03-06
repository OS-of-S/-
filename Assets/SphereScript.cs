using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour {
    private Vector3 q;
    private bool Ending=false;
    bool m = true;
    private Mesh mesh;
    private float dist = 0;
    private const float RadPlus = 0.002f;
    private const float eps = 0.001f;
    private const float speed = 3;
    public bool RadColUsed;
    public bool TrigerZoneUsed;
    private float speed2 = 0.3f;
    private float aceleration;
    bool t;
    public Renderer turnOFF;
    /*
     * Тип проверки на завершение уровня:
     * 0 - вокруг детали в радиусе Rad нет объектов
     * 1 - деталь входит в заранее заготовленный коллайдер
     * 2 - всё вместе?
     */
    //public Collider EndCollder;

    // Use this for initialization
    void Start () {
        t = true;
        mesh = transform.parent.GetComponent<MeshFilter>().mesh;
        int b = mesh.vertices.Length;


        for (int i=0; i<b; i++)
        {
            float a = (mesh.vertices[i]-transform.localPosition).magnitude;
            if (a > dist) dist = a;
        }
        dist *= 2;

        dist += RadPlus;
        //if (dist > Rad) Debug.Log(this.name + " have dist = " + dist + " less than Rad!");
        q = new Vector3(dist, dist, dist);
        aceleration = (speed2 * speed2) / (2 * dist);
    }
	
	// Update is called once per frame
	void Update () {

        if (!Ending && !TrigerZoneUsed && !RadColUsed)
        {
            Ending = true;
            GetComponent<AudioSource>().Play();
        }
		
        if (Ending && t)
        {
            if (m)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, q, Time.deltaTime * speed);
                if (dist - transform.localScale.x < eps)
                {
                    if (transform.parent.GetComponent<Collider>()) transform.parent.GetComponent<Collider>().enabled = false;
                    if (transform.parent.GetComponent<MeshStructurizer>()) transform.parent.GetComponent<MeshStructurizer>().Reflag();
                    transform.parent.GetComponent<Renderer>().enabled=false;
                    if (turnOFF!=null) turnOFF.enabled = false;
                    speed2 = 0;
                    m = false;
                }
            }
            else
            {
                float z = speed2 * Time.deltaTime;
                if (transform.localScale.x > z) transform.localScale -= new Vector3(1, 1, 1) * z;
                else transform.localScale = Vector3.zero;
                speed2 += aceleration * Time.deltaTime;
                if (transform.localScale.x < eps)
                {
                    transform.localScale = Vector3.zero;
                    t = false;
                    //Ending = false;
                    //m = true;
                }
            }
        }



	}

    IEnumerator GetBack()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("RETURNING!");
    }
}
