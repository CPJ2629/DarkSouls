using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput playerInput;
    public CameraHandle cam;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f; //奔跑速度加成
    public float jumpVelocity = 3.0f;
    public float rollVelocity = 2.0f;
    public float stepbackMultiplier = 10.0f;

    //通过切换模型的物理材质,来改变物体间摩擦力，如人物下落时与斜坡,大楼进行摩擦
    [Header("---Friction Settings---")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial Zero_friction;
    [SerializeField]
    public Animator animator;
    private Rigidbody rigid;
    private Vector3 planeVec;
    private Vector3 thrustVec; //跳跃冲量
    private bool canAttack;
    private bool trackDirection = false;
    private CapsuleCollider col;
    private float lerpDefenseLayerTarget;
    private Vector3 deltaPos;
    private bool lockPlaneMove = false; //锁死平面移动速度更新,保持末速度不变

    public bool leftIsShield;
    public delegate void OnActionDelegate();
    public event OnActionDelegate onAction;

    void Awake()
    {
        animator = model.GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>(); //物理引擎50帧，故Rigidbody不能在Update里使用
        col = GetComponent<CapsuleCollider>();
        leftIsShield = true;
    }

    public void OnAction()
    {
        onAction();
    }

    void Update()
    {
        float targetRunMulti = ((playerInput.run) ? runMultiplier : 1.0f);

        if (playerInput.lockon) cam.Lock();

        if (!cam.lockState)
        {
            animator.SetFloat("forward", playerInput.moveAmount * targetRunMulti * Mathf.Lerp(animator.GetFloat("forward"), targetRunMulti, 0.5f));
            animator.SetFloat("right", 0);
        }
        else
        {
            animator.SetFloat("forward", playerInput.moveDirection.z * targetRunMulti);
            animator.SetFloat("right",playerInput.moveDirection.x * targetRunMulti);
        }

        if (playerInput.roll)
        {
            animator.SetTrigger("roll");
            canAttack = false;
        }

        if (playerInput.jump)
        {
            animator.SetTrigger("Jump");
            canAttack = false;
        }

        if ((playerInput.RB || playerInput.LB) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)
        {
            if (playerInput.RB)
            {
                animator.SetBool("R0L1", false);
                animator.SetTrigger("attack");
            }
            else if (playerInput.LB && !leftIsShield)
            {
                animator.SetBool("R0L1", true);
                animator.SetTrigger("attack");
            }
        }

        //盾反功能
        if ((playerInput.LT || playerInput.RT) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)
        {
            if (playerInput.RT)
            {
                
            }
            else
            {
                if (!leftIsShield)
                {

                }
                else
                {
                    animator.SetTrigger("counterback");
                }
            }
        }

        if (playerInput.action)
        {
            onAction.Invoke(); 
        }

        if (leftIsShield)
        {
            if (CheckState("ground") || CheckState("block"))
            {
                animator.SetBool("defense", playerInput.defense);
                animator.SetLayerWeight(animator.GetLayerIndex("defense"), 1);
            }
            else
            {
                animator.SetBool("defense", false);
                animator.SetLayerWeight(animator.GetLayerIndex("defense"), 0);
            }
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("defense"), 0);
        }

        //视角锁定
        if (!cam.lockState)
        {
            if (playerInput.moveAmount > 0.1f)
            {
                Vector3 targetForward = Vector3.Slerp(model.transform.forward, playerInput.moveDirection, 0.3f); //使向量平缓变化,Lerp()线状,Slerp()球状
                model.transform.forward = targetForward;
            }

            if (!lockPlaneMove) planeVec = playerInput.moveAmount * model.transform.forward * walkSpeed * targetRunMulti;  //range时刻更新
        }

        else
        {
            if (!trackDirection)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planeVec.normalized;
            }

            if (!lockPlaneMove) planeVec = playerInput.moveDirection * walkSpeed * targetRunMulti;
        }


        if (playerInput.stepback) animator.SetTrigger("stepback");



    }

    private void FixedUpdate()
    {
        rigid.position += deltaPos;
        //rigid.position += planeVec * Time.fixedDeltaTime;
        rigid.velocity = new Vector3(planeVec.x, rigid.velocity.y, planeVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    //确认模型当前处于的动画状态
    public bool CheckState(string state,string layername = "Base Layer")
    {
        int layerIndex=animator.GetLayerIndex(layername);
        bool is_state = animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(state);
        return is_state;
    }

    public bool CheckStateTag(string tag, string layername = "Base Layer")
    {
        int layerIndex = animator.GetLayerIndex(layername);
        bool is_state = animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tag);
        return is_state;
    }


    #region Message Processing Block
    public void OnJumpEnter()
    {
        playerInput.inputEnable = false;
        lockPlaneMove = true;
        thrustVec = new Vector3(0, jumpVelocity , 0);
        trackDirection = true;
    }

    public void IsGround()
    {
        animator.SetBool("isGround", true);

    }

    public void IsNotGround()
    {
        animator.SetBool("isGround", false);

    }

    public void OnGroundEnter()
    {
        playerInput.inputEnable = true;
        lockPlaneMove= false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
    }

    public void OnGroundExit()
    {
        col.material = Zero_friction;
    }

    public void OnFallEnter()
    {
        playerInput.inputEnable = false;
        lockPlaneMove = true;
    }

    public void OnRollEnter()
    {
        playerInput.inputEnable = false;
        lockPlaneMove = true;
        thrustVec = new Vector3(0, rollVelocity, 0);
        trackDirection = true;
    }

    public void OnStepbackEnter()
    {
        playerInput.inputEnable = false;
        lockPlaneMove = true;
    }

    public void OnStepbackUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("stepback") * stepbackMultiplier;
    }

    public void OnAttackOnehand()
    {
        playerInput.inputEnable = false;
    }

    public void OnAttackOneUpdate()
    {
        //thrustVec = model.transform.forward * animator.GetFloat("attack_onehand1");
    }

    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnIdle()
    {
        playerInput.inputEnable = true;
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack_onehand3")) deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos) / 1.0f;
    }

    public void onDefense()
    {
        lerpDefenseLayerTarget = 1.0f;
    }

    public void onDefenseIdle()
    {
        lerpDefenseLayerTarget = 0f;
    }

    public void onDefenseIdleUpdate()
    {
        int index = animator.GetLayerIndex("defense");
        float currentWeight = animator.GetLayerWeight(index);
        currentWeight = Mathf.Lerp(currentWeight, lerpDefenseLayerTarget, 0.01f);
        animator.SetLayerWeight(index, currentWeight);
    }

    public void onDefenseUpdate()
    {
        int index = animator.GetLayerIndex("defense");
        float currentWeight = animator.GetLayerWeight(index);
        currentWeight = Mathf.Lerp(currentWeight, lerpDefenseLayerTarget, 0.01f);
        animator.SetLayerWeight(index, currentWeight);
    }

    public void OnHitEnter()
    {
        playerInput.inputEnable = false;
        planeVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void IssueTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetBool(string boolName,bool value)
    {
        animator.SetBool(boolName, value);
    }

    public void OnBlockEnter()
    {
        playerInput.inputEnable = false;
    }

    public void OnDieEnter()
    {
        playerInput.inputEnable = false;
        planeVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnStunnerEnter()
    {
        playerInput.inputEnable = false;
        planeVec = Vector3.zero;
    }

    public void OnCounterBackEnter()
    {
        playerInput.inputEnable = false;
        planeVec = Vector3.zero;
    }

    public void OnLockEnter()
    {
        playerInput.inputEnable = false;
        planeVec = Vector3.zero;
    }

    #endregion
}




