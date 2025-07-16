using System;
using UnityEngine;

namespace Games
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager Instance { get; private set; }
        public bool EscOpenMenuEnabled = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (EscOpenMenuEnabled && Input.GetKeyDown(KeyCode.Escape))
            {
                if (UniversalUiMenu.Instance)
                    UniversalUiMenu.CloseMenu();
                else
                    UniversalUiMenu.OpenMenu();
            }
        }
    }
}