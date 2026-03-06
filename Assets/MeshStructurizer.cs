using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshStructurizer : MonoBehaviour
{
    public Renderer Kostil; // Костыль. Для нулевого уровня, чтобы менять цвет у изображения мыши на кубике.
    Mesh mesh;
    private Dictionary<long, int> Trngls; // По первой вершине треугольника возвращается сторона, которой принадлежит этот треугольник.
    private List<List<int>> Parts;
    private Vector3[] Centers;
    private Vector3[] Normals;
    private int[] PartMode;
    private Renderer[] Flags;
    private HashSet<int> Yelows;
    private HashSet<int> Perples;
    public Material WMat;
    public Material YMat;
    public Material PMat;
    public PhysicMaterial ZeroFrictionMaterial;

    private int MeshSize;
    private const float eps = 0.0001f;
    private const float SpeedLimit = 100f;
    private int WhiteSubmwshNum = 0;
    private int YellowSubmeshNum;

    public Rigidbody rigidbody;
    public GameObject FlagSample;
    private Transform Transf;

    //Настройки Риджидбади
    public float mass=1f;
    public float drag=20f;
    public float angularDrag=20f;
    public RigidbodyConstraints C;
    private CounterScr YScrpt;
    private CounterScr PScrpt;
    private GameObject Cam;

    private bool activ;
    private long SubMeshOffset;

    private int[] WhiteTris;
    //public bool ready=false;

    /*
    void DeepSearch(int PrevTriangle)
    {
        int PartNumber = Parts.Count-1;

        for (int i = 0; i < MeshSize; i+=3)
        {
            int TriangleIndex = mesh.GetTriangles(WhiteSubmwshNum)[i];

            if ((!Trngls.ContainsKey(i)) &&(Vector3.Cross(mesh.normals[TriangleIndex], mesh.normals[mesh.GetTriangles(WhiteSubmwshNum)[PrevTriangle]]).magnitude<eps))
            {
                int CommonDots = 0;
                for (int j = 0; j < 9; j++)
                {
                    int j1 = j / 3;
                    int j2 = j % 3;

                    if (mesh.GetTriangles(WhiteSubmwshNum)[i + j1] == mesh.GetTriangles(WhiteSubmwshNum)[PrevTriangle + j2])
                    {
                        CommonDots++;
                        if (CommonDots > 1) break;
                    }
                }
                if (CommonDots>1)
                {
                    Trngls[i] = PartNumber;
                    Parts[PartNumber].Add(i);
                    DeepSearch(i);
                }
            }
        }
    }*/

    // Start is called before the first frame update
    void Awake()
    {
        Cam = Camera.main.gameObject;
        YScrpt = Cam.GetComponent<RayCaster>().YellowCounter;
        PScrpt = Cam.GetComponent<RayCaster>().PerpleCounter;
        //ready = false;
        this.gameObject.layer = 8;

        mesh = GetComponent<MeshFilter>().mesh;

        if (rigidbody == null)
        {
            //создание физической болванки с риджидбади.
            GameObject FisicalCopy = new GameObject();
            FisicalCopy.transform.SetParent(null);
            FisicalCopy.transform.position = transform.position;
            FisicalCopy.transform.rotation = transform.rotation;
            FisicalCopy.transform.localScale = transform.lossyScale;
            Transf = FisicalCopy.transform;
            MeshFilter MF = FisicalCopy.AddComponent(typeof(MeshFilter)) as MeshFilter;
            MF.mesh = mesh;
            MeshCollider MC = FisicalCopy.AddComponent(typeof(MeshCollider)) as MeshCollider;
            MC.material = ZeroFrictionMaterial;
            MC.convex = true;
            rigidbody = FisicalCopy.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rigidbody.useGravity = false;
            rigidbody.mass = mass;
            rigidbody.drag = drag;
            rigidbody.angularDrag = angularDrag;
            rigidbody.constraints = C;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        else
        {
            Transf = rigidbody.gameObject.transform;
        }

        Renderer rend = GetComponent<Renderer>();

        //Debug.Log(rend.materials[0] + " UWU "+ WMat);
        WhiteSubmwshNum = -1;
        for (int i=0; i< rend.materials.Length;i++)
        {
            if (rend.materials[i].color == WMat.color)
            {
                WhiteSubmwshNum = i;
                break;
            }
        }
        if (WhiteSubmwshNum>=0) activ = true;
        else activ = false;

        if (activ)
        {
            SubMeshOffset = mesh.GetIndexStart(WhiteSubmwshNum);
            WhiteTris = mesh.GetTriangles(WhiteSubmwshNum);

            
            //копируем меш
            Mesh newmesh = new Mesh();
            newmesh.name = mesh.name+"Copy";
            newmesh.vertices = mesh.vertices;
            newmesh.subMeshCount = mesh.subMeshCount;
            //newmesh.triangles = mesh.triangles;
            for (int i = 0; i < rend.materials.Length; i++)
            {
                newmesh.SetTriangles(mesh.GetTriangles(i), i, false);
            }
            newmesh.uv = mesh.uv;
            newmesh.normals = mesh.normals;
            newmesh.colors = mesh.colors;
            newmesh.tangents = mesh.tangents;
            mesh = newmesh;
            GetComponent<MeshFilter>().mesh = mesh;
            //Thanks to cannon from here: https://forum.unity.com/threads/how-do-i-duplicate-a-mesh-asset.35639/
            

            Trngls = new Dictionary<long, int>();
            Yelows = new HashSet<int>();
            Perples = new HashSet<int>(); ;
            Parts = new List<List<int>>();
            MeshSize = WhiteTris.Length;
            Queue<int> SearchTris = new Queue<int>();

            //Добавляем два цвета - для толкания и притягивания.
            YellowSubmeshNum = mesh.subMeshCount;
            Material[] mats = new Material[YellowSubmeshNum + 2];
            for (int i = 0; i < YellowSubmeshNum; i++) mats[i] = rend.materials[i];
            mats[YellowSubmeshNum] = YMat;
            mats[YellowSubmeshNum + 1] = PMat;
            rend.materials = mats;

            mesh.subMeshCount = YellowSubmeshNum + 2;
            //int[] arr = new int[0];
            //mesh.SetTriangles(arr, YellowSubmeshNum, false);
            //mesh.SetTriangles(arr, YellowSubmeshNum + 1, false);

            int PartNumber = -1;

            for (int i = 0; i < MeshSize; i += 3)
            {
                int TriangleIndex = WhiteTris[i];

                if (!Trngls.ContainsKey(i))
                {
                    Parts.Add(new List<int>());
                    PartNumber++;
                    Trngls[i] = PartNumber;
                    Parts[PartNumber].Add(i);
                    //DeepSearch(i);
                    SearchTris.Enqueue(i);
                    while (SearchTris.Count > 0)
                    {
                        int PrevTriangle = SearchTris.Dequeue();
                        for (int k = 0; k < MeshSize; k += 3)
                        {
                            TriangleIndex = WhiteTris[k];

                            if ((!Trngls.ContainsKey(k)) && (Vector3.Cross(mesh.normals[TriangleIndex], mesh.normals[WhiteTris[PrevTriangle]]).magnitude < eps))
                            {
                                int CommonDots = 0;
                                for (int j = 0; j < 9; j++)
                                {
                                    int j1 = j / 3;
                                    int j2 = j % 3;

                                    if (WhiteTris[k + j1] == WhiteTris[PrevTriangle + j2])
                                    {
                                        CommonDots++;
                                        if (CommonDots > 1) break;
                                    }
                                }
                                if (CommonDots > 1)
                                {
                                    Trngls[k] = PartNumber;
                                    Parts[PartNumber].Add(k);
                                    //DeepSearch(k);
                                    SearchTris.Enqueue(k);
                                }
                            }
                        }
                    }
                }
            }

            PartMode = new int[Parts.Count];
            Centers = new Vector3[Parts.Count];
            Normals = new Vector3[Parts.Count];
            Flags = new Renderer[Parts.Count];
            for (int i = 0; i < Parts.Count; i++)
            {
                PartMode[i] = 0;
                Centers[i] = Vector3.zero;
                for (int j = 0; j < Parts[i].Count; j++)
                {
                    Centers[i] += mesh.vertices[WhiteTris[Parts[i][j]]] + mesh.vertices[WhiteTris[Parts[i][j] + 1]] + mesh.vertices[WhiteTris[Parts[i][j] + 2]];
                }
                Centers[i] /= 3 * Parts[i].Count;

                Normals[i] = mesh.normals[WhiteTris[Parts[i][0]]];

                Flags[i] = Instantiate(FlagSample, transform).GetComponent<Renderer>();
                Flags[i].gameObject.transform.localPosition = Centers[i];
                Flags[i].gameObject.transform.localRotation = Quaternion.LookRotation(Normals[i]);


            }
        }

        //ready = true;
    }

    void Recolor()
    {
        Debug.Log("Recolor " + Yelows.Count * 3);
        int q = 0;
        int[] tW = new int[MeshSize - Yelows.Count * 3 - Perples.Count * 3];
        for (int k=0; k< MeshSize;k+=3)
        {
            if (!Yelows.Contains(k) && !Perples.Contains(k))
            {
                tW[q] = WhiteTris[k];
                tW[q + 1] = WhiteTris[k + 1];
                tW[q + 2] = WhiteTris[k + 2];
                q += 3;
            }
        }
        mesh.SetTriangles(tW, WhiteSubmwshNum, false);
        q = 0;
        int[] tY = new int[Yelows.Count * 3];
        Yelows.CopyTo(tY);
        foreach (int k in Yelows)
        {
            tY[q] = WhiteTris[k];
            tY[q + 1] = WhiteTris[k + 1];
            tY[q + 2] = WhiteTris[k + 2];
            q += 3;
        }
        mesh.SetTriangles(tY, YellowSubmeshNum, false);
        int[] tP = new int[Perples.Count * 3];
        Perples.CopyTo(tP);
        q = 0;
        foreach (int k in Perples)
        {
            tP[q] = WhiteTris[k];
            tP[q + 1] = WhiteTris[k + 1];
            tP[q + 2] = WhiteTris[k + 2];
            q += 3;
        }
        mesh.SetTriangles(tP, YellowSubmeshNum + 1, false);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void ForceChanger(long TriNumber, int Strength, bool FlagMode)
    {
        if (!activ) return;
        //Strength * mesh.normals[TriNumber];
        //for (int i=0; i< Parts[TriNumber].Count; i++)
        //foreach (KeyValuePair<long, int> i in Trngls) Debug.Log(i);
        TriNumber -= SubMeshOffset;
        if (!Trngls.ContainsKey(TriNumber)) return;

        int part=Trngls[TriNumber];

        if (FlagMode)
        {
            int z = Strength * 2;
            if (PartMode[part] == -z)
            {
                if ((PartMode[part] == 2) && (PScrpt.GetFlag()))
                {
                    Flags[part].transform.GetChild(0).GetComponent<AudioSource>().Play();
                    Flags[part].GetComponent<AudioSource>().Play();
                    YScrpt.AddFlag();
                }
                if ((PartMode[part] == -2) && (YScrpt.GetFlag()))
                {
                    Flags[part].transform.GetChild(0).GetComponent<AudioSource>().Play();
                    Flags[part].GetComponent<AudioSource>().Play();
                    PScrpt.AddFlag();
                }
            }
            else if (PartMode[part] == z)
            {
                z = 0;
                Flags[part].transform.GetChild(0).GetComponent<AudioSource>().Play();
                if (PartMode[part] == 2) YScrpt.AddFlag();
                else PScrpt.AddFlag();
            }
            else
            {
                if (z == 2)
                {
                    if (!YScrpt.GetFlag()) z = 0;
                    else Flags[part].GetComponent<AudioSource>().Play();
                }
                else
                {
                    if (!PScrpt.GetFlag()) z = 0;
                    else Flags[part].GetComponent<AudioSource>().Play();
                }
            }

            switch (z)
            {
                case 2:
                    Flags[part].material = YMat;
                    Flags[part].enabled = true;
                    break;
                case 0:
                    Flags[part].enabled = false;
                    break;
                case -2:
                    Flags[part].material = PMat;
                    Flags[part].enabled = true;
                    break;
            }
            PartMode[part] = z;
        }
        else
        {
            if (Mathf.Abs(PartMode[part]) != 2) PartMode[part] = Strength;
            else return;
        }

        foreach (int i in Parts[part])
        {

            Debug.Log(i);
            Yelows.Remove(i);
            Perples.Remove(i);

            switch (Strength)
            {
                case 1:
                    Yelows.Add(i);
                    break;
                case -1:
                    Perples.Add(i);
                    break;
            }
        }

        Recolor();

        if (Kostil != null)
        {
            switch (Strength)
            {
                case 1:
                    Kostil.material = YMat;
                    break;
                case -1:
                    Kostil.material = PMat;
                    break;
                default:
                    Kostil.material = WMat;
                    break;

            }
        }


    }

    private void LateUpdate()
    {
        transform.position = Transf.position;
        transform.rotation = Transf.rotation;
    }

    void FixedUpdate()
    {

        if (!activ) return;
        //rigidbody.AddForceAtPosition(Vector3.up * 10000, Vector3.zero, ForceMode.Force);
        bool q = false;
        //Vector3 Veloc = Vector3.zero;
        for (int part=0; part< PartMode.Length; part++)
        {
            int z = 0;
            if (PartMode[part] > 0) z = 1;
            if (PartMode[part] < 0) z = -1;
            if (PartMode[part] != 0) q = true;

            rigidbody.AddForceAtPosition(-SpeedLimit * z *(rigidbody.gameObject.transform.rotation* Normals[part]), Transf.rotation * Centers[part]);
            //Veloc += -SpeedLimit * PartMode[part] * (rigidbody.gameObject.transform.rotation * mesh.normals[mesh.GetTriangles(0)[Parts[part][0]]]);
        }
        if (q) GetComponent<AudioSource>().volume = Mathf.Min(rigidbody.velocity.magnitude * 0.05f, 0.7f);
        else GetComponent<AudioSource>().volume = 0;


        //rigidbody.velocity = Veloc;

            /*if (rigidbody.velocity.magnitude > SpeedLimit)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * SpeedLimit;
            }*/
    }

    public void Reflag()
    {
        if (!activ) return;
        for (int part=0; part< PartMode.Length; part++)
        {
            if (Mathf.Abs(PartMode[part])==2)
            {
                PartMode[part] = 0;
                Flags[part].transform.GetChild(0).GetComponent<AudioSource>().Play();
                Flags[part].GetComponent<Renderer>().enabled = false;
            }
        }
        rigidbody.GetComponent<Collider>().enabled = false;
        //Recolor();
    }
}
