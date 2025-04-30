using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponManager wm;
    public WeaponData wd;
    void Awake()
    {
        wd = GetComponentInChildren<WeaponData>();
    }

    void Update()
    {
        
    }

    public float GetATK()
    {
        return wd.ATK + wm.am.sm.ATK;
    }
}
