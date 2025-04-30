using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esc : MonoBehaviour
{
    public GameObject menuList;
    private bool menuActive = false;
    void Start()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            menuList.SetActive(!menuActive);
            Time.timeScale = !menuActive ? 0f : 1f;
            menuActive = !menuActive;
        }
    }

    public void BackGame()
    {
        menuList.SetActive(false);
        Time.timeScale = 1f;
        menuActive = false;
    }
}
