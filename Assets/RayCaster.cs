using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    Camera cam;
    Collider OldCol;
    long OldTriangle;
    int OldStrength;
    private bool z = false;
    private const float timeEps = 0.2f;

    private float DownClick1;
    private float DownClick2;

    public CounterScr YellowCounter;
    public CounterScr PerpleCounter;

    // Start is called before the first frame update
    void Start()
    {
        OldStrength = 0;
        OldTriangle = -1;
        OldCol = null;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        long Triangle = -1;
        Collider Col = null;
        int strength = 0;
        if (Input.GetButton("Fire1")) strength += 1;
        if (Input.GetButton("Fire2")) strength -= 1;

        bool FlagMode = false;
        if (Input.GetButtonDown("Fire1")) DownClick1 = Time.time;
        if (Input.GetButtonDown("Fire2")) DownClick2 = Time.time;
        if ((Input.GetButtonUp("Fire1")) && (Time.time - DownClick1 < timeEps)) { FlagMode = true; strength = 1; }
        if ((Input.GetButtonUp("Fire2")) && (Time.time - DownClick2 < timeEps)) { FlagMode = true; strength = -1; }

        if (strength!=0)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << 8))
            {
                Col=hit.collider;
                Triangle=hit.triangleIndex*3; //оно умножается на 1
            }
        }

        if ((OldTriangle != Triangle || OldCol != Col) && (OldTriangle > -1 && OldCol.GetComponent<MeshStructurizer>())) OldCol.GetComponent<MeshStructurizer>().ForceChanger(OldTriangle, 0, false);

        if (OldStrength != strength || OldTriangle != Triangle || OldCol != Col || FlagMode)
        {
            if (Triangle > -1 && Col.GetComponent<MeshStructurizer>())
            {
                Debug.Log("Force Sent!");
                Col.GetComponent<MeshStructurizer>().ForceChanger(Triangle, strength, FlagMode);
                Debug.Log("Force Sent 2!");
            }
            if (!FlagMode)
            {
                OldStrength = strength;
                OldTriangle = Triangle;
                OldCol = Col;
            }
        }


    }
}


/*
 * Dynamic batching: index buffer source is NULL (first IB byte 0 count 0 first vertex 0 offset 0)
 * 
 * 
 * 
 * Ответ от Jessespike · 21/08/16 11:40

Edit -> Project Settings -> Player

Under the Other Settings tab, disable Dynamic Batching.

That might get rid of the errors, Why the errors are even happening is what I'm curious about.

Добавить комментарий ·  1 скрыть · Предоставить общий доступ
avatar imageAlpha_ThePro · 21/08/16 11:51 0
Thanks, i was aware of that however i would rather keep it on incase i want it for other object in my scene. I ended up disabling batching in the shader tags ( "DisableBatching" = "True" ) that has returned my FPS to a playable level however i would still like to know what caused these errors, probably my newbie coding skills and the fact that decent tutorials on submesh's are hard to find.

 */
