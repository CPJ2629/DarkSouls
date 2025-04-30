using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//手柄--键鼠互换
/*
RB--mouse 0
LB--mouse 1
RT--e
LT--q
*/

public class PlayerInput : MonoBehaviour
{
    [Header("------ Key settings -----")]
    public string keyUp="w";
    public string keyDown="s";
    public string keyLeft="a";
    public string keyRight="d";

    [Header("-----  -----")]
    public MyButton KeyA = new MyButton(); //left shift
    public MyButton KeyB = new MyButton(); //space
    public MyButton KeyMouse0 = new MyButton(); //mouse 0
    public MyButton KeyMouse1 = new MyButton(); //mouse 1 
    public MyButton KeyF = new MyButton(); //f
    public MyButton KeyG = new MyButton(); //g-后撤
    public MyButton KeyLT = new MyButton(); //LT
    public MyButton KeyRT = new MyButton(); //RT
    public MyButton KeyC = new MyButton(); //c--翻滚
    public MyButton Key1 =new MyButton();
    public MyButton Key2 = new MyButton();
    public MyButton Key3 = new MyButton();


    [Header("----- Output Signals -----")]
    public float y; //上下
    public float x; //左右
    public float moveAmount; //动画变换幅度
    public Vector3 moveDirection;   //角色方向

    [Header("----- Mouse Setting -----")]
    public bool mouseEnable = true; //鼠标是否可用
    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;
    public float view_x;
    public float view_y;

    #region 右下箭头输入及其按键
    public string KeySmallUp = "up";
    public string KeySmallDown = "down";
    public string KeySmallLeft = "left";
    public string KeySmallRight = "right";
    #endregion

    #region signal
    public bool run; //pressing signal
    public bool jump; //trigger once signal
    //public bool attack; //trigger once signal
    public bool defense;
    public bool roll;
    public bool lockon;
    public bool stepback;
    public bool RT;
    public bool LT;
    public bool RB;
    public bool LB;
    public bool action;
    public bool weapon1;
    public bool weapon2;
    public bool weapon3;
    #endregion

    [Header("----- Others -----")]
    public bool inputEnable = true; //控件是否开启，flag

    private float targetY;
    private float targetX;
    private float velocityDup;
    private float velocityDright;
    void Update()
    {
        //视角移动
        if (mouseEnable)
        {
            view_x = Input.GetAxis("Mouse X") * mouseSensitivityX;
            view_y = Input.GetAxis("Mouse Y") * mouseSensitivityY;
        }
        else
        {
            view_y = (Input.GetKey(KeySmallUp) ? 1.0f : 0) - (Input.GetKey(KeySmallDown) ? 1.0f : 0);
            view_x = (Input.GetKey(KeySmallRight) ? 1.0f : 0) - (Input.GetKey(KeySmallLeft) ? 1.0f : 0);
        }

        //人物移动
        targetY = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetX = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        if (!inputEnable)
        {
            targetY = 0;
            targetX = 0;
        }

        y = Mathf.SmoothDamp(y, targetY, ref velocityDup, 0.1f); //平滑地变换数值,用于混合树中平滑切换动画状态
        x = Mathf.SmoothDamp(x, targetX, ref velocityDright, 0.1f);

        Vector2 tempRange = SquareToCircle(new Vector2(x, y));
        float x2 = tempRange.x;
        float y2 = tempRange.y;

        moveAmount = Mathf.Sqrt((y2 * y2) + (x2 * x2));
        moveDirection = y2 * transform.forward + x2 * transform.right;

        KeyC.tick(Input.GetKey("c"));
        roll = KeyC.onPressed && (Input.GetKey(keyUp) || Input.GetKey(keyLeft) || Input.GetKey(keyRight));

        KeyA.tick(Input.GetKey("left shift"));
        run = ((KeyA.isPressing && !KeyA.isDelaying) || KeyA.isExtending) && !roll;

        KeyB.tick(Input.GetKey("space"));
        jump = KeyB.onPressed;

        KeyMouse0.tick(Input.GetKey("mouse 0"));
        RB = KeyMouse0.onPressed;

        KeyMouse1.tick(Input.GetKey("mouse 1"));
        LB = KeyMouse1.onPressed;
        defense = KeyMouse1.isPressing;

        KeyF.tick(Input.GetKey("f"));
        lockon = KeyF.onPressed;

        KeyG.tick(Input.GetKey("g"));
        stepback = KeyG.onPressed;

        KeyLT.tick(Input.GetKey("q"));
        LT=KeyLT.onPressed;

        KeyRT.tick(Input.GetKey("e"));
        RT = KeyRT.onPressed;

        Key1.tick(Input.GetKey("1"));
        weapon1 = Key1.onPressed;

        Key2.tick(Input.GetKey("2"));
        weapon2 = Key2.onPressed;

        Key3.tick(Input.GetKey("3"));
        weapon3 = Key3.onPressed;
    }

    private Vector2 SquareToCircle(Vector2 input) //坐标系椭圆映射法，控制斜向向量长度为1,防止斜向移动时速度偏快
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }

    protected void UpdateMove(float x2,float y2)
    {
        moveAmount = Mathf.Sqrt((y2 * y2) + (x2 * x2));
        moveDirection = y2 * transform.forward + x2 * transform.right;
    }
}
