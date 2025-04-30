using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    public Collider weaponColL;
    public Collider weaponColR;

    public GameObject WHL; //◊Û ÷Œ‰∆˜
    public GameObject WHR;

    public WeaponController WCL;
    public WeaponController WCR;


    void Start()
    {
        WHL = transform.DeepFind("WeaponHandleL").gameObject;
        WHR = transform.DeepFind("WeaponHandleR").gameObject;
        weaponColL = WHL.GetComponentInChildren<Collider>();
        weaponColR = WHR.GetComponentInChildren<CapsuleCollider>();
        am = GetComponentInParent<ActorManager>();

        WCL = BindWeaponController(WHL);
        WCR = BindWeaponController(WHR);
    }

    public WeaponController BindWeaponController(GameObject targetobj)
    {
        WeaponController tempWC;
        tempWC = targetobj.GetComponentInChildren<WeaponController>();
        if (tempWC == null) tempWC = targetobj.AddComponent<WeaponController>();
        tempWC.wm = this;
        return tempWC;
    }

    public void WeaponEnable()
    {
        if(am.ac.CheckStateTag("attackL")) weaponColL.enabled = true;
        else weaponColR.enabled = true;
    }

    public void WeaponDisable()
    {
        weaponColR.enabled = false;
        weaponColL.enabled = false;
    }

    public void CounterBackEnable()
    {
        am.SetIsCounterBack(true);
    }

    public void CounterBackDisable()
    {
        am.SetIsCounterBack(false);
    }

    public void UpdateWeaponCollider(string side,Collider col)
    {
        if (side == "L") weaponColL = col;
        else if(side == "R") weaponColR = col;
    }

    public void UnloadWeapon(string side)
    {
        if(side == "L")
        {
            foreach(Transform trans in WHL.transform){
                weaponColL = null;
                WCL.wd = null;
                Destroy(trans.gameObject);
            }
        }
        else if(side == "R")
        {
            foreach (Transform trans in WHR.transform)
            {
                weaponColR = null;
                WCR.wd = null;
                Destroy(trans.gameObject);
            }
        }
    }
}
