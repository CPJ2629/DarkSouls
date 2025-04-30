using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum PackageMode { Normal,Sort,Delete}

public class PackagePanel : BasePanel
{
    private Transform UIMenu;
    private Transform UIMenuWeapon;
    private Transform UIMenuFood;
    private Transform UITabName;
    private Transform UICloseBtn;
    private Transform UICenter;
    private Transform UIScrollView;
    private Transform UIDetailPanel;

    private Transform UILeftBtn;
    private Transform UIRightBtn;
    private Transform UIDeletePanel;
    private Transform UIDeleteBackBtn;
    private Transform UIBottomMenus;
    private Transform UIDeleteBtn;
    private Transform UIDetailBtn;
    private Transform UIOnDelete;
    private Transform UICapacity;

    public GameObject PackageUIItemPrefab;
    public PackageMode curMode = PackageMode.Normal;
    private string _chooseUid;//当前选中物品的uid
    private int curType;
    private int count = 0;
    public string chooseUID
    {
        get
        {
            return _chooseUid;
        }
        set
        {
            _chooseUid = value;
        }
    }
    public List<string> deleteChooseUid;

    public void AddChooseDeleteUid(string uid)
    {
        this.deleteChooseUid ??= new List<string>();
        if(!this.deleteChooseUid.Contains(uid)) this.deleteChooseUid.Add(uid);
        else this.deleteChooseUid.Remove(uid);
        RefreshDeletePanel();
    }

    private void RefreshDeletePanel()
    {
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        foreach(Transform cell in scrollContent)
        {
            PackageCell packageCell = cell.GetComponent<PackageCell>();
            packageCell.RefreshDeleteState();
        }
    }

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        InitUIName();
        InitClick();
        curType = 0;
        UpdateTabName(curType);
        PackageUIItemPrefab = Resources.Load("Prefab/package") as GameObject;
    }

    private void Start()
    {
        //InitUI();
       /* curType = 0;
        UpdateTabName(curType);*/
    }

    public void InitUI()
    {
        InitUIName();
        InitClick();
        PackageUIItemPrefab = Resources.Load("Prefab/package") as GameObject;
    }

    private void RefreshUI()
    {
        if (!gameObject.activeSelf) return;
        StopCoroutine("DelayedRefresh");
        StartCoroutine(DelayedRefresh());
    }

    private IEnumerator DelayedRefresh()
    {
        // 等待一帧避免同一帧内多次刷新
        yield return null;
        RefreshScroll();
    }

    public void OnEnable()
    {
        curType = 0;
        UpdateTabName(curType);
        RefreshUI();
    }

    private void RefreshScroll()
    {
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++) Destroy(scrollContent.GetChild(i).gameObject); //删除滚动容器中的所有物品
        count = 0;

        foreach(PackageLocalItem localData in GameManager.Instance.GetPackageLocalData()) //载入新的静态、动态数据
        {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
            print("mid");
            count++;
        }
        print("out");
        UICapacity.GetComponent<Text>().text = count.ToString() + " / 1000";
        print("outRefreshScroll");
    }

    public void RefreshDetail()
    {
        PackageLocalItem localItem = GameManager.Instance.GetPackageLocalItemByUId(chooseUID);
        UIDetailPanel.GetComponent<DetailPanel>().Refresh(localItem, this);
    }

    private void InitUIName()
    {
        UIMenu = transform.Find("centerTop/Image/Menu");
        UIMenuWeapon = transform.Find("centerTop/Image/Menu/Weapon");
        UIMenuFood = transform.Find("centerTop/Image/Menu/Food");
        UITabName = transform.Find("leftTop/TabName");
        UICloseBtn = transform.Find("rightTop/Close");
        UICenter = transform.Find("center");
        UIScrollView = transform.Find("center/Scroll View");
        UIDetailPanel = transform.Find("center/DetailPanel");
        UILeftBtn = transform.Find("center/left");
        UIRightBtn = transform.Find("center/right");

        UIDeletePanel = transform.Find("Bottom/DeletePanel");
        UIDeleteBackBtn = transform.Find("Bottom/DeletePanel/Bg/back");

        UIBottomMenus = transform.Find("Bottom/BottomMenus");
        UIDeleteBtn = transform.Find("Bottom/DeletePanel/Bg/delete");
        UIDetailBtn = transform.Find("Bottom/BottomMenus/DetailBtn");
        UIOnDelete = transform.Find("Bottom/BottomMenus/DeleteBtn");
        UICapacity = transform.Find("rightTop/capacity");


        UIDetailPanel.gameObject.SetActive(false);
        UIDeletePanel.gameObject.SetActive(false);
        UIBottomMenus.gameObject.SetActive(true);
    }

    private void InitClick()
    {
        UIMenuWeapon.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClickWeapon);
        UIMenuFood.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClickFood);
        UICloseBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClickClose);
        UILeftBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClickLeft);
        UIRightBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClickRight);

        UIDeleteBackBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onDeleteBack);
        UIDeleteBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DeleteSelect);
        UIDetailBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onDetail);
        UIOnDelete.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onDelete);
    }

    private List<PackageLocalItem> RefreshType(int type)
    {
        List<PackageLocalItem> list = new List<PackageLocalItem>();
        foreach (PackageLocalItem localData in GameManager.Instance.GetPackageLocalData())
        {
            if (GameManager.Instance.GetPackageItemById(localData.id).type == type) list.Add(localData);
        }
        return list;
    }

    private void onClickWeapon()
    {
        curType = 0;
        List<PackageLocalItem> list = RefreshType(0);
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++) Destroy(scrollContent.GetChild(i).gameObject); //删除滚动容器中的所有物品

        foreach (PackageLocalItem localData in list)
        {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
        }
        UpdateTabName(curType);
    }

    private void onClickFood()
    {
        curType = 1;
        List<PackageLocalItem> list = RefreshType(1);
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++) Destroy(scrollContent.GetChild(i).gameObject); //删除滚动容器中的所有物品

        foreach (PackageLocalItem localData in list)
        {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
        }
        UpdateTabName(curType);
    }

    private void onClickClose()
    {
        ClosePanel();
    }

    private void onClickLeft()
    {
        curType = Mathf.Abs((curType - 1)) % 2;
        List<PackageLocalItem> list = RefreshType(curType);
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++) Destroy(scrollContent.GetChild(i).gameObject); //删除滚动容器中的所有物品

        foreach (PackageLocalItem localData in list)
        {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
        }
        UpdateTabName(curType);
    }

    private void onClickRight()
    {
        curType = (curType + 1) % 2;
        List<PackageLocalItem> list = RefreshType(curType);
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++) Destroy(scrollContent.GetChild(i).gameObject); //删除滚动容器中的所有物品

        foreach (PackageLocalItem localData in list)
        {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
        }
        UpdateTabName(curType);
    }

    private void onDeleteBack()
    {
        curMode = PackageMode.Normal;
        UIDeletePanel.gameObject.SetActive(false);
        deleteChooseUid = new List<string>();
        RefreshDeletePanel();
    }

    private void onDelete()
    {
        curMode = PackageMode.Delete;
        UIDeletePanel.gameObject.SetActive(true);
    }

    private void onDetail()
    {
        RefreshDetail();
        UIDetailPanel.gameObject.SetActive(!UIDetailPanel.gameObject.activeSelf);
    }

    private void UpdateTabName(int type)
    {
        string name = "";
        switch (type)
        {
            case 0:name = "武器";break;
            case 1:name = "食物";break;
        }
        UITabName.GetComponent<Text>().text = name;
    }

    private void DeleteSelect()
    {
        GameManager.Instance.DeletePackageItems(deleteChooseUid);
        RefreshScroll();
    }
}
