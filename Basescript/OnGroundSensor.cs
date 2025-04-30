using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//为实现fall,每帧检测碰撞，检测与之碰撞的图层是不是ground层
public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public float offset = 0.1f;

    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    void Awake()
    {
        radius=capcol.radius - 0.05f;
    }

    
    void FixedUpdate()
    {
        #region 胶囊体的上下两个端点
        point1 = transform.position + transform.up * (radius - offset); //下端点
        point2 = transform.position + transform.up * (capcol.height - offset) - transform.up * radius; //上端点
        #endregion

        Collider[] outputCols = Physics.OverlapCapsule( point1, point2, radius, LayerMask.GetMask("Ground") ); //检测与给定胶囊形状重叠的碰撞体
        if (outputCols.Length != 0)
        {
            SendMessageUpwards("IsGround");
        }
        else SendMessageUpwards("IsNotGround");
    }
}
