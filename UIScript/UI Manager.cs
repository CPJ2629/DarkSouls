using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    private Dictionary<string, string> pathDict; //����->·��
    private Dictionary<string, GameObject> prefabDict;//��ǰ�Ѿ����ڵ�Ԥ�Ƽ�(����)
    public Dictionary<string, BasePanel> panelDict; //��ǰ�Ѿ��򿪵Ľ���
    private Transform uiRoot;//Ҫ�������Ǹ��������

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
                    uiRoot = new GameObject("Canvas").transform;//Ϊ����ÿһ��panel�������ڵ�����Canvas
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
            return null; //�����Դ�
        }

        string path = "";
        if(!pathDict.TryGetValue(name,out path))
        {
            return null; //·��������
        }

        GameObject panelPrefab = null;
        if(!prefabDict.TryGetValue(name,out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }

        //�򿪽���
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

public class UIConst //Ϊ�������Ԥ������ַ������ƣ���������
{
    public const string PackagePanel = "PackagePanel";

}
