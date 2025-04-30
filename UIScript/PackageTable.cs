using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//¾²Ì¬Êý¾Ý

[CreateAssetMenu(menuName ="PackageTable",fileName ="PackageTable")]
public class PackageTable : ScriptableObject
{
    public List<PackageTableItem> DataList = new List<PackageTableItem>();
}

[System.Serializable]
public class PackageTableItem
{
    public int id;
    public int type;
    public int ATK;
    public int num;

    public string name;
    public string description;
    public string imagePath;
}

