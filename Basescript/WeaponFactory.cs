using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class WeaponFactory
{
    private DataBase weaponDB;
    
    public WeaponFactory(DataBase weaponDB)
    {
        this.weaponDB = weaponDB;
    }

    public GameObject CreateWeapon(string weaponName,Vector3 pos,Quaternion rot)
    {
        GameObject prefab = Resources.Load(weaponName) as GameObject;
        GameObject obj = GameObject.Instantiate(prefab,pos, rot);
        
        WeaponData weaponData = obj.AddComponent<WeaponData>();
        weaponData.ATK = weaponDB.weaponDataBase[weaponName]["ATK"].floatValue;

        return obj;
    }

    public Collider CreateWeapon(string weaponName, string side, WeaponManager wm)
    {
        WeaponController wc;
        if (side == "L")
        {
            wc = wm.WCL;
        }
        else if (side == "R")
        {
            wc = wm.WCR;
        }
        else return null;

        GameObject prefab = Resources.Load(weaponName) as GameObject;
        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.parent = wc.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = new Quaternion(-180, 0, 30, 0);

        if (obj.GetComponent<WeaponData>() != null) obj.GetComponent<WeaponData>().enabled = false;
        WeaponData weaponData = obj.AddComponent<WeaponData>();
        weaponData.ATK = weaponDB.weaponDataBase[weaponName]["ATK"].floatValue;
        wc.wd = weaponData;

        obj.AddComponent<CapsuleCollider>();
        obj.GetComponent<CapsuleCollider>().isTrigger = true;
        obj.tag = "Weapon";
        obj.layer = 9;

        return obj.GetComponent<Collider>();
    }


}
