using UnityEngine;
using System.Collections;

public class Cam_control : MonoBehaviour {
    private float sensitivity = 100.0f;
    public GameObject CamYrot;
    //public GameObject player;
    public GameObject CamItself;
    //public GameObject CamPos;
    //public GameObject Tors;
    private float x;
    private float y;
    //private RaycastHit hit;
    //private float CamStandartPos;
    //public float CamAimPos;
    //private float CamNormPos;
    //private float size;
    //private float newCam;
    //private Vector3 CamDir;
    //public float CamRadius;
    //public bool guned;
    //public float ZoomSpeed;
    //public float ZoomSpeed2;
    //public Vector3 ZoomPlace;
    //public Vector3 StandartPlace;
    //private Vector3 NowPlace;

    void Start () {
        sensitivity = sensitivity / CamItself.transform.localPosition.magnitude;

        //size= CamYrot.transform.localScale.x;
        //CamStandartPos = CamItself.transform.localPosition.z;
        //CamNormPos = CamStandartPos;
        //StandartPlace = transform.localPosition;
    }

    void Update()
    {
        //Вращение камеры мышью:
        if (Input.GetButton("Fire3"))
        {
            x = Input.GetAxis("Mouse X") * sensitivity;
            y = -Input.GetAxis("Mouse Y") * sensitivity;
            //CamYrot.transform.Rotate(0, x, 0);
            //transform.Rotate(y, x, 0);

            transform.Rotate(new Vector3(y, x, 0), Space.Self);
        }

        //движение камеры за игроком:
        //CamPos.transform.position = player.transform.position;

        //Зум при перекрывании игрока стенами:
        //CamDir=transform.TransformDirection(Vector3.back);
        /*
        if (Physics.SphereCast(transform.position,CamRadius,CamDir, out hit))
        {
            newCam = -hit.distance / size;
            if (newCam > CamNormPos)
            {
                CamItself.transform.localPosition = new Vector3(0, 0, newCam);
            }
            else
            {
                CamItself.transform.localPosition = new Vector3(0, 0, CamNormPos);
            }
        }
        else
        {
            CamItself.transform.localPosition = new Vector3(0, 0, CamNormPos);
        }
        
        //Переход в режим прицела:
        if (guned)
        {
            CamNormPos = Mathf.MoveTowards(CamNormPos, CamAimPos,Time.deltaTime*ZoomSpeed);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, ZoomPlace, Time.deltaTime * ZoomSpeed2);
        }
        else
        {
            CamNormPos = Mathf.MoveTowards(CamNormPos, CamStandartPos, Time.deltaTime * ZoomSpeed);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, StandartPlace, Time.deltaTime * ZoomSpeed2);
        }*/
    }
    /*
    private void LateUpdate()
    {
        if (guned) Tors.transform.rotation = transform.rotation;
    }*/
}
