using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    private Dictionary<string, string> pathDict; //名称->路径
    private Dictionary<string, GameObject> prefabDict;//当前已经存在的预制件(缓存)
    public Dictionary<string, BasePanel> panelDict; //当前已经打开的界面
    private Transform uiRoot;//要挂载在那个父物件上

    public static UIManager Instance
    {
        get
        {
            if(instance == null) instance = new UIManager();
            return instance;
        }
    }

    public Transform UIRoot
    {
        get
        {
            if (uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    uiRoot = new GameObject("Canvas").transform;//为了让每一个panel都挂载在单独的Canvas
                }
            }
            return uiRoot;
        }
    }

    private UIManager()
    {
        InitDicts();
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();

        pathDict = new Dictionary<string, string>()
        {
            {UIConst.PackagePanel, "Package/PackagePanel"},

        };

    }

    public BasePanel OpenPanel(string name)
    {
        BasePanel panel = null;
        if(panelDict.TryGetValue(name,out panel))
        {
            return null; //界面以打开
        }

        string path = "";
        if(!pathDict.TryGetValue(name,out path))
        {
            return null; //路径不存在
        }

        GameObject panelPrefab = null;
        if(!prefabDict.TryGetValue(name,out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }

        //打开界面
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        return panel;
    }

    public bool ClosePanel(string name)
    {
        BasePanel panel = null;
        if(!panelDict.TryGetValue(name, out panel))
        {
            return false;
        }
        panel.ClosePanel();
        return true;
    }
}

public class UIConst //为储存界面预制体的字符串名称，创建该类
{
    public const string PackagePanel = "PackagePanel";

}
