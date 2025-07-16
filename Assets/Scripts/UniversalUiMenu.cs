using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniversalUiMenu : MonoBehaviour
{
    [SerializeField] private RawImage background; // 改为RawImage
    [SerializeField] private Button closeButton; // Esc Also works
    [SerializeField] private Button backToMainMenuButton;
    public static UniversalUiMenu Instance { get; private set; }

    private void Start()
    {
        Time.timeScale = 0;
        backToMainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Title");
            TitleScene.shouldJumpToMainMenu = true;
            Time.timeScale = 1;
        });
    }

    public static void OpenMenu()
    {
        if (Instance)
            throw new Exception("Menu is already open");
        Instance = Instantiate(Resources.Load<GameObject>("Prepabs/General/Universal UI")).GetComponent<UniversalUiMenu>();
    }

    public static void CloseMenu()
    {
        Destroy(Instance.gameObject);
        Instance = null;
        Time.timeScale = 1;
    }
}
