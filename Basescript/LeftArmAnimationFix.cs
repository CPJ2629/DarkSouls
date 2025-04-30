using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimationFix : MonoBehaviour
{
    private Animator anim;
    private ActorController ac;
    public Vector3 angle;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }

    void OnAnimatorIK()
    {
        if (ac.leftIsShield)
        {
            if (!anim.GetBool("defense"))
            {
                Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                leftLowerArm.localEulerAngles += 0.75f * angle;
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
            }
        }
    }
}
