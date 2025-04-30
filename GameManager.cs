using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance {  get { return instance; } }

    private DataBase weaponDB;
    private WeaponFactory weaponFactory;
    public WeaponManager wm;

    public PackageTable packageTable;
    public GameObject bag;
    public GameObject interation;
    public GameObject getGoods;
    public PackagePanel packagePanel;


    void Awake()
    {
        CheckGameObject();
        CheckSingle();
    }

    void Start()
    {
        InitWeaponDB();
        InitWeaponFactory();
        //packagePanel = bag.GetComponentInChildren<PackagePanel>();
    }

    public void DeletePackageItems(List<string> uids)
    {
        foreach(string uid in uids)
        {
            DeletePackageItem(uid, false);
        }
        PackageLocalData.Instance.SavePackage();
    }

    public void DeletePackageItem(string uid,bool needSave)
    {
        PackageLocalItem packageLocalItem = GetPackageLocalItemByUId(uid);
        if (packageLocalItem == null) return;
        PackageLocalData.Instance.items.Remove(packageLocalItem);
        if (needSave) PackageLocalData.Instance.SavePackage();
    }

    void Update()
    {
        SwitchWeapon();
        UpdateBag();
    }

    private void CheckSingle()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); //防止场景切换后gm被销毁
            return;
        }
        Destroy(this);
    }

    private void CheckGameObject()
    {
        if (gameObject.tag == "GM") return;
        else Destroy(this);
    }

    private void UpdateBag()
    {
        if (Input.GetKeyDown(KeyCode.B)) packagePanel.gameObject.SetActive(!packagePanel.gameObject.activeSelf);
        Time.timeScale = packagePanel.gameObject.activeSelf ? 0f : 1.0f;
    }

    private void InitWeaponDB()
    {
        weaponDB = new DataBase();
    }

    private void SwitchWeapon()
    {
        if (wm.am.ac.playerInput.weapon1)
        {
            wm.UnloadWeapon("R");
            wm.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Sword", "R", wm));
        }
        else if (wm.am.ac.playerInput.weapon2)
        {
            wm.UnloadWeapon("R");
            wm.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Mace", "R", wm));
        }
        else if (wm.am.ac.playerInput.weapon3)
        {
            wm.UnloadWeapon("R");
            wm.UpdateWeaponCollider("R", weaponFactory.CreateWeapon("Falchion", "R", wm));
        }
    }

    public void UpdateInteration(bool active)
    {
        interation.SetActive(active);
    }

    public IEnumerator UpdateGetGoods()
    {
        GetWeapon((int)Random.Range(0, 3));
        //packagePanel.RefreshUI();
        getGoods.SetActive(true);
        yield return new WaitForSeconds(2f);
        getGoods.SetActive(false);
    }

    private void InitWeaponFactory()
    {
        weaponFactory = new WeaponFactory(weaponDB);
    }

    public PackageTable GetPackageTable()
    {
        if (packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("PackageTable");
        }
        return packageTable;
    }

    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }

    public PackageTableItem GetPackageItemById(int id)
    {
        List<PackageTableItem> packageDataList = GetPackageTable().DataList;
        foreach(PackageTableItem item in packageDataList)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public PackageLocalItem GetPackageLocalItemByUId(string uid)
    {
        List<PackageLocalItem> packageDataList = GetPackageLocalData();
        foreach(PackageLocalItem item in packageDataList)
        {
            if(item.uid == uid)
            {
                return item;
            }
        }
        return null;
    }

    public PackageLocalItem GetWeapon(int index)
    {
        PackageTableItem item = packageTable.DataList[index];

        PackageLocalItem packageLocalItem = new()
        {
            uid = System.Guid.NewGuid().ToString(),
            id = item.id,
            num = 1,
            isNew = true,
            isEquip = false
        };
        if (PackageLocalData.Instance.items == null) PackageLocalData.Instance.items = new List<PackageLocalItem>();
        PackageLocalData.Instance.items.Add(packageLocalItem);
        PackageLocalData.Instance.SavePackage();
        return packageLocalItem;
    }

    public class PackageItemComparer:IComparer<PackageLocalItem>
    {
        public int Compare(PackageLocalItem x, PackageLocalItem y)
        {
            PackageTableItem a = GameManager.instance.GetPackageItemById(x.id);
            PackageTableItem b = GameManager.instance.GetPackageItemById(y.id);

            return b.ATK.CompareTo(a.ATK);
        }
    }


    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.LoadPackage();
        localItems.Sort(new PackageItemComparer());
        return localItems;
    }
}
