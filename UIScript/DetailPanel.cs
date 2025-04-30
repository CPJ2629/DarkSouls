using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanel : MonoBehaviour
{
    private Transform UIDescription;
    private Transform UIIcon;
    private Transform UITitle;
    private Transform UIParameter;

    private PackageLocalItem packageLocalData;
    private PackageTableItem packageTableItem;
    private PackagePanel uiParent;

    private void Awake()
    {
        InitUIName();
    }

    private void InitUIName()
    {
        UIDescription = transform.Find("Center/Description");
        UIIcon = transform.Find("Center/Icon");
        UITitle = transform.Find("Top/Title");
        UIParameter = transform.Find("Bottom/Parameter");
    }

    public void Refresh(PackageLocalItem packageLocalData,PackagePanel uiParent)
    {
        this.packageLocalData = packageLocalData;
        this.uiParent = uiParent;
        this.packageTableItem = GameManager.Instance.GetPackageItemById(packageLocalData.id);

        UIDescription.GetComponent<Text>().text = this.packageTableItem.description;
        UITitle.GetComponent<Text>().text = this.packageTableItem.name;
        UIParameter.GetComponent<Text>().text = "¹¥»÷Á¦: " + this.packageTableItem.ATK;

        Texture2D t = (Texture2D)Resources.Load(this.packageTableItem.imagePath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        UIIcon.GetComponent<Image>().sprite = temp;
    }
}
