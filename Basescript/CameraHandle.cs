using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandle : MonoBehaviour
{
    public PlayerInput pi;
    public float horizontalSpeed = 50.0f;
    public float verticalSpeed = 20.0f;
    public float cameraDampValue = 1f;
    public Image lockDot; //����ͼ��
    public bool lockState;
    public bool isAI = false;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX; //ʹ��ŷ����������ͬλ������ 
    private GameObject model;
    private GameObject camera;
    private LockTarget lockTarget; //����Ŀ��

    private Vector3 cameraDampVelocity;
    void Start()
    {
        cameraHandle = transform.parent.gameObject; //����transform��ȡĸGameObject
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        model = playerHandle.GetComponent<ActorController>().model; //��ȡģ��ŷ����
        if (!isAI)
        {
           camera = Camera.main.gameObject;
           lockDot.enabled = false;
         //Cursor.lockState = CursorLockMode.Locked;//����ʾ��Ļ�еĹ��
        }
        lockState = false;
    }

    void FixedUpdate()
    {
        if (lockTarget == null) {
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.view_x * horizontalSpeed * Time.fixedDeltaTime); //�ý�������Y����תʵ������ͷˮƽ��ת,�ý�������X����תʵ������ͷ��ֱ��ת
                                                                                                          //cameraHandle.transform.Rotate(Vector3.right, pi.small_y * -verticalSpeed * Time.deltaTime);  
            tempEulerX -= pi.view_y * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -15, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;
        }

        else
        {
            //ʹ�ӽ����ĸ��汻����������
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
        }

        if (!isAI) { 
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, cameraDampValue); //���׷��ģ���ƶ�
        //camera.transform.eulerAngles = transform.eulerAngles;
        camera.transform.LookAt(cameraHandle.transform);
        }
    }

    private void Update()
    {
        if (lockTarget != null)
        {
            if (!isAI)
            {
                lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            }
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f)
            {
                lockProcessA(null, false, false, isAI);
            }

            if (lockTarget.am != null && lockTarget.am.sm.isDie)
            {
                lockProcessA(null, false, false, isAI);
            }
        }
    }

    private void lockProcessA(LockTarget target,bool lockDotenable,bool lockstate,bool isai)
    {
        lockTarget = target;
        if (!isai)
        {
            lockDot.enabled = lockDotenable;
        }
        lockState = lockstate;
    }


    //����ԭ��:�����ǰ�������а뾶������,��������������������巢����ײ
    public void Lock()
    {
        Vector3 playerFootPos = model.transform.position;
        Vector3 playerMidPos = playerFootPos + new Vector3(0, 1, 0);
        Vector3 boxCenter = playerMidPos + model.transform.forward * 5f; //������������

        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 10f), model.transform.rotation, LayerMask.GetMask(isAI?"Player":"Enemy")); //��״��ײ
        if (cols.Length == 0)
        {
            lockProcessA(null, false, false, isAI);
        }
        else
        {
            foreach (Collider col in cols)
            {
                if (lockTarget != null && lockTarget.obj == col.gameObject)
                {
                    lockProcessA(null, false, false, isAI);
                    break;
                }
                lockProcessA(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }

    }
}

public class LockTarget
{
    public GameObject obj;
    public float halfHeight;
    public ActorManager am;
    public LockTarget(GameObject _obj, float _haflHeight)
    {
        this.obj = _obj;
        halfHeight = _haflHeight;
        am = _obj.GetComponent<ActorManager>();
    }
}
