using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Search;
using UnityEngine;

//动态数据--本地存储方式
public class PackageLocalData
{
    private static PackageLocalData instance;
    public List<PackageLocalItem> items;

    public static PackageLocalData Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PackageLocalData();
            }
            return instance;
        }
    }


    public void SavePackage()
    {
        string inventoryJson = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PackageLocalData", inventoryJson);
        PlayerPrefs.Save();
    }

    public List <PackageLocalItem> LoadPackage()
    {
        //if (items != null) return items;

        if (PlayerPrefs.HasKey("PackageLocalData"))
        {
            string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
            PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(inventoryJson);
            items = packageLocalData.items;
            return items;
        }
        else
        {
            items = new List<PackageLocalItem>();
            return items;
        }
    }

}

[System.Serializable]
public class PackageLocalItem
{
    public string uid;

    public int id;
    public int num;

    public bool isNew;
    public bool isEquip;
}
