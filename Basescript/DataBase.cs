using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase
{
    private string weaponDatabaseFileName = "weaponData";
    public readonly JSONObject weaponDataBase;

    public DataBase()
    {
        TextAsset weaponContent = Resources.Load(weaponDatabaseFileName) as TextAsset;
        weaponDataBase = new JSONObject(weaponContent.text);
    }
}
