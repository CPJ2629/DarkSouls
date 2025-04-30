using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PackageCell : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    private Transform UIIcon;
    private Transform UIHead;
    private Transform UINew;
    private Transform UISelect;
    private Transform UIDeleteSelect;

    private PackageLocalItem packageLocalData;
    private PackageTableItem packageTableItem;
    private PackagePanel uiParent;

    private void Awake()
    {
        InitUIName();
    }

    private void Update()
    {
        if (this.uiParent.chooseUID != this.packageLocalData.uid)
        {
            UISelect.gameObject.SetActive(false);
            uiParent.RefreshDetail();
        }
    }

    private void InitUIName()
    {
        UIIcon = transform.Find("Top/Icon");
        UIHead = transform.Find("Top/Head");
        UINew = transform.Find("Top/New");
        UISelect = transform.Find("Select");
        UIDeleteSelect = transform.Find("DeleteSelect");
    }

    public void Refresh(PackageLocalItem packageLocalData,PackagePanel uiParent) //更新滚动容器内单个格子的属性
    {
        this.packageLocalData = packageLocalData;
        this.packageTableItem = GameManager.Instance.GetPackageItemById(packageLocalData.id);
        this.uiParent = uiParent;

        UINew.gameObject.SetActive(this.packageLocalData.isNew);
        Texture2D t = (Texture2D)Resources.Load(this.packageTableItem.imagePath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        UIIcon.GetComponent<Image>().sprite = temp;
    }

    public void RefreshDeleteState()
    {
        if (this.uiParent.deleteChooseUid.Contains(this.packageLocalData.uid)) this.UIDeleteSelect.gameObject.SetActive(true);
        else this.UIDeleteSelect.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.uiParent.curMode == PackageMode.Delete) this.uiParent.AddChooseDeleteUid(this.packageLocalData.uid);
        this.uiParent.chooseUID = this.packageLocalData.uid;
        UISelect.gameObject.SetActive(!UISelect.gameObject.activeSelf);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
