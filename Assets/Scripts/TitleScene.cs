using System;
using System.Collections;
using System.Collections.Generic;
using Games;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    private Menu _currentMenu = Menu.AnyKeyOrButton; // Current menu state
    private Menu _targetMenu = Menu.AnyKeyOrButton;
    public static bool shouldJumpToMainMenu = false; // Flag to indicate if we should jump to the main menu directly
    
    [Header("Enter the Game")]
    public TextMeshProUGUI pressAnyKeyOrButtonText; // Text to prompt user to press any key or button
    public float pressAnyKeyOrButtonTextFadeDuration = 1f; // Duration for which the prompt text is displayed
    private bool _isAnyKeyOrButtonPressed = false; // Flag to check if any key or button is pressed
    private bool _isPressAnyKeyOrButtonTextFading = false;
    private bool _isPressAnyKeyOrButtonTextVisible = true; // Flag to check if the prompt text is visible
    public Vector3 _pressAnyKeyOrButtonTextFadeScale = Vector3.one * 2; // Timer for fading the prompt text
    
    [Header("Main Menu")]
    private bool _isMainMenuVisible = false; // Flag to check if the main menu is visible
    private bool _isMainMenuFadingIn = false; // Flag to check if the main menu is fading in
    private bool _isMainMenuFadingOut = false; // Flag to check if the main menu is fading out
    public float mainMenuFadeDuration = 1f; // Duration for the main menu fade in/out
    public Button chooseGameButton; // Button to choose a game
    private Image _chooseGameButtonImage;
    public TextMeshProUGUI chooseGameButtonText;
    public Button exitGameButton; // Button to exit the game
    private Image _exitGameButtonImage;
    public TextMeshProUGUI exitGameButtonText;
    public Button trainModelButton; // Button to start the game
    private Image _trainModelButtonImage;
    public TextMeshProUGUI trainModelButtonText;
    
    [Header("Choose Game")]
    public GamePreview[] gamePreviewImages; // Array to hold game preview images
    public Transform gamePreviewPanel; // Panel to display game previews
    public float ArcRadius = 100f; // Radius of the sector for arranging preview images
    public float ArcAngle = 60f; // Angle of the sector for arranging preview images
    public Vector2 ArcCenterOffset = new Vector2(0f, 0f); // Center of the sector for arranging preview images
    private RectTransform[] _previewImageRectTransforms; // RectTransform for the preview images
    public Button backToMainMenuButton; // Button to go back to the main menu
    private Image _backToMainMenuButtonImage; // Image for the back to main menu button
    public TextMeshProUGUI backToMainMenuButtonText; // Text for the back to main menu button
    public float HidingArcRadius = 50f;
    public float buttonFadeDuration = 0.5f; // Duration for fading buttons in and out
    public float previewImageTransitionDuration = 1f;
    public float singlePreviewImageTransitionDuration = 0.5f;
    private bool _isChooseGameMenuVisible = false; // Flag to check if the choose game menu is visible
    private bool _isChooseGameMenuFadingIn = false; // Flag to check if the choose game menu is fading in
    private bool _isChooseGameMenuFadingOut = false; // 新增：标记是否在离开ChooseGame菜单
    private float _previewImageTransitionProgress = 0f; // Progress of the preview image transition
    private bool _isPreviewImageTransitioning = false; // Flag to check if the preview images are transitioning
    private float[] _previewImageStartTimes; // 每张预览图的动画开始时间
    private bool[] _previewImageIsMoving; 
    private bool _isPreviewImageShowing = true; // true: 展示动画，false: 隐藏动画
    private float _previewImageFromRadius;
    private float _previewImageToRadius;
    
    [Header("Choose Game")]
    public TextMeshProUGUI chooseGameMenuTipText; // 选择游戏菜单提示文字
    private bool _isChooseGameMenuTipFadingIn = false;
    private bool _isChooseGameMenuTipFadingOut = false;
    private bool _isChooseGameMenuTipVisible = false;

    public void StartGame(string sceneName)
    {
        // Load the specified scene to start the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        GlobalGameManager.Instance.EscOpenMenuEnabled = true;
    }
    
    private void Start()
    {
        GlobalGameManager.Instance.EscOpenMenuEnabled = false;
        #region MainMenu
        
        _chooseGameButtonImage = chooseGameButton.GetComponent<Image>();
        _exitGameButtonImage = exitGameButton.GetComponent<Image>();
        _trainModelButtonImage = trainModelButton.GetComponent<Image>();

        // Fade the buttons
        _chooseGameButtonImage.color = ChangeColorAlpha(_chooseGameButtonImage.color);
        _exitGameButtonImage.color = ChangeColorAlpha(_exitGameButtonImage.color);
        _trainModelButtonImage.color = ChangeColorAlpha(_trainModelButtonImage.color);
        chooseGameButtonText.color = ChangeColorAlpha(chooseGameButtonText.color);
        exitGameButtonText.color = ChangeColorAlpha(exitGameButtonText.color);
        trainModelButtonText.color = ChangeColorAlpha(trainModelButtonText.color);
        
        //disable buttons
        chooseGameButton.enabled = false;
        exitGameButton.enabled = false;
        trainModelButton.enabled = false;
        
        // Deactivate GameObjects
        chooseGameButton.gameObject.SetActive(false);
        exitGameButton.gameObject.SetActive(false);
        trainModelButton.gameObject.SetActive(false);
        
        chooseGameButton.onClick.AddListener(GotoChooseGameMenu);
        exitGameButton.onClick.AddListener(
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode
#else
            Application.Quit
#endif
        );

        #endregion

        #region ChooseGame

        _backToMainMenuButtonImage = backToMainMenuButton.GetComponent<Image>();
        _backToMainMenuButtonImage.color = ChangeColorAlpha(_backToMainMenuButtonImage.color);
        backToMainMenuButtonText.color = ChangeColorAlpha(backToMainMenuButtonText.color);
        chooseGameMenuTipText.color = ChangeColorAlpha(chooseGameMenuTipText.color);
        
        
        backToMainMenuButton.enabled = false;
        backToMainMenuButton.gameObject.SetActive(false);
        
        _previewImageRectTransforms = new RectTransform[gamePreviewImages.Length];
        for (int i = 0; i < gamePreviewImages.Length; i++)
        {
            var go = new GameObject(gamePreviewImages[i].gameName, typeof(RectTransform));
            go.transform.SetParent(gamePreviewPanel, false);
            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.sprite = gamePreviewImages[i].previewImage;
            _previewImageRectTransforms[i] = go.GetComponent<RectTransform>();
            var gameSceneName = gamePreviewImages[i].gameName;
            go.AddComponent<Button>().onClick.AddListener(() => StartGame(gameSceneName));
        }
        // 初始化时直接用 HidingArcRadius
        int count = _previewImageRectTransforms.Length;
        for (int i = 0; i < count; i++)
        {
            float angle = -ArcAngle / 2f + (ArcAngle / (count - 1)) * i;
            float rad = (angle + 90f) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(
                ArcCenterOffset.x + Mathf.Cos(rad) * HidingArcRadius,
                ArcCenterOffset.y + Mathf.Sin(rad) * HidingArcRadius
            );
            _previewImageRectTransforms[i].anchoredPosition = pos;
            _previewImageRectTransforms[i].localRotation = Quaternion.Euler(0, 0, angle);
            _previewImageRectTransforms[i].localScale = gamePreviewImages[i].scale;
        }
        
        _previewImageStartTimes = new float[_previewImageRectTransforms.Length];
        _previewImageIsMoving = new bool[_previewImageRectTransforms.Length];
        float interval = (previewImageTransitionDuration - singlePreviewImageTransitionDuration) / (_previewImageRectTransforms.Length - 1);
        for (int i = 0; i < _previewImageRectTransforms.Length; i++)
        {
            _previewImageStartTimes[i] = interval * i;
            _previewImageIsMoving[i] = false;
        }
        
        #endregion

        // 只在 Start 里绑定一次返回主菜单按钮事件
        backToMainMenuButton.onClick.RemoveAllListeners();
        backToMainMenuButton.onClick.AddListener(() => {
            // 开始离开 ChooseGame 菜单的动画
            EndTransitFromChooseGame();
            backToMainMenuButton.enabled = false;
            // 不要立即切换菜单，等待动画完成
        });

        if (shouldJumpToMainMenu)
        {
            JumpToMainMenuDirectly();
            shouldJumpToMainMenu = false;
        }
    }
    
    private void EndTransitFromChooseGame()
    {
        // 设置离开标志
        _isChooseGameMenuFadingOut = true;
        // 淡出 chooseGameMenuTipText
        _isChooseGameMenuTipFadingOut = true;
        // 预览图隐藏动画
        _isPreviewImageTransitioning = true;
        _previewImageTransitionProgress = 0f;
        _isPreviewImageShowing = false;
        _previewImageFromRadius = ArcRadius;
        _previewImageToRadius = HidingArcRadius;
    }

    private void Update()
    {
        Color color;
        float a;
        switch (_currentMenu)
        {
            case Menu.AnyKeyOrButton: goto AnyKeyOrButton; break;
            case Menu.MainMenu: goto MainMenu; break;
            case Menu.ChooseGame: goto ChooseGame; break;
            default: throw new ArgumentOutOfRangeException();
        }
        goto End;
        
        AnyKeyOrButton:
        if (!_isAnyKeyOrButtonPressed && Input.anyKeyDown)
        {
            _isAnyKeyOrButtonPressed = true;
            _isPressAnyKeyOrButtonTextFading = true;
        }
        if (_isPressAnyKeyOrButtonTextFading)
        {
            color = pressAnyKeyOrButtonText.color;
            color.a -= Time.deltaTime / pressAnyKeyOrButtonTextFadeDuration;
            pressAnyKeyOrButtonText.color = color;
            pressAnyKeyOrButtonText.rectTransform.localScale = Vector3.Lerp(Vector3.one, _pressAnyKeyOrButtonTextFadeScale, 1f - color.a);
            if (color.a <= 0f)
            {
                color.a = 0f;
                _isPressAnyKeyOrButtonTextFading = false;
                _isPressAnyKeyOrButtonTextVisible = false;
                _currentMenu = Menu.MainMenu;
                BeginTransitToMainMenu();
                goto MainMenu;
            }
        }
        goto End;

        MainMenu:

        if (_isMainMenuFadingIn && !_isMainMenuVisible)
        {
            a = exitGameButtonText.color.a + Time.deltaTime / mainMenuFadeDuration;
            exitGameButtonText.color = ChangeColorAlpha(exitGameButtonText.color, a);
            _exitGameButtonImage.color = ChangeColorAlpha(_exitGameButtonImage.color, a);
            chooseGameButtonText.color = ChangeColorAlpha(chooseGameButtonText.color, a);
            _chooseGameButtonImage.color = ChangeColorAlpha(_chooseGameButtonImage.color, a);
            trainModelButtonText.color = ChangeColorAlpha(trainModelButtonText.color, a);
            _trainModelButtonImage.color = ChangeColorAlpha(_trainModelButtonImage.color, a);
            
            if (a >= 1f)
            {
                a = 1f;
                EnterMainMenu();
            }
        }
        
        if (_isMainMenuFadingOut && _isMainMenuVisible)
        {
            a = exitGameButtonText.color.a - Time.deltaTime / mainMenuFadeDuration;
            exitGameButtonText.color = ChangeColorAlpha(exitGameButtonText.color, a);
            _exitGameButtonImage.color = ChangeColorAlpha(_exitGameButtonImage.color, a);
            chooseGameButtonText.color = ChangeColorAlpha(chooseGameButtonText.color, a);
            _chooseGameButtonImage.color = ChangeColorAlpha(_chooseGameButtonImage.color, a);
            trainModelButtonText.color = ChangeColorAlpha(trainModelButtonText.color, a);
            _trainModelButtonImage.color = ChangeColorAlpha(_trainModelButtonImage.color, a);
            if (a <= 0f)
            {
                a = 0f;
                _isMainMenuFadingOut = false;
                _isMainMenuVisible = false;
                chooseGameButton.enabled = false;
                exitGameButton.enabled = false;
                trainModelButton.enabled = false;
                _currentMenu = _targetMenu;
                switch (_currentMenu)
                {
                    case Menu.ChooseGame:
                        BeginTransitToChooseGame();
                        goto ChooseGame;
                        break;
                }
            }
        }
        

        goto End;
        
        ChooseGame:
        // 淡入 chooseGameMenuTipText
        if (_isChooseGameMenuTipFadingIn && !_isChooseGameMenuTipVisible)
        {
            a = chooseGameMenuTipText.color.a + Time.deltaTime / buttonFadeDuration;
            chooseGameMenuTipText.color = ChangeColorAlpha(chooseGameMenuTipText.color, a);
            if (a >= 1f)
            {
                a = 1f;
                _isChooseGameMenuTipFadingIn = false;
                _isChooseGameMenuTipVisible = true;
            }
        }
        // 淡出 chooseGameMenuTipText
        if (_isChooseGameMenuTipFadingOut && _isChooseGameMenuTipVisible)
        {
            a = chooseGameMenuTipText.color.a - Time.deltaTime / buttonFadeDuration;
            chooseGameMenuTipText.color = ChangeColorAlpha(chooseGameMenuTipText.color, a);
            if (a <= 0f)
            {
                a = 0f;
                _isChooseGameMenuTipFadingOut = false;
                _isChooseGameMenuTipVisible = false;
            }
        }

        // 预览图动画进度递增
        if (_isPreviewImageTransitioning)
        {
            _previewImageTransitionProgress += Time.deltaTime;
            if (_previewImageTransitionProgress > previewImageTransitionDuration)
            {
                _previewImageTransitionProgress = previewImageTransitionDuration;
            }
        }
        
        // backToMainMenuButton 淡入逻辑
        if (_isChooseGameMenuFadingIn && !_isChooseGameMenuVisible)
        {
            a = backToMainMenuButtonText.color.a + Time.deltaTime / buttonFadeDuration;
            backToMainMenuButtonText.color = ChangeColorAlpha(backToMainMenuButtonText.color, a);
            _backToMainMenuButtonImage.color = ChangeColorAlpha(_backToMainMenuButtonImage.color, a);
            if (a >= 1f)
            {
                a = 1f;
                backToMainMenuButton.enabled = true;
                _isChooseGameMenuFadingIn = false;
                _isChooseGameMenuVisible = true;
            }
        }
        
        // 预览图动画处理
        if ((_isChooseGameMenuFadingIn && !_isChooseGameMenuVisible) || (!_isChooseGameMenuFadingIn && _isPreviewImageTransitioning))
        {
            // 动画播放期间（无论是进入还是离开）都执行插值
            int count = _previewImageRectTransforms.Length;
            for (int i = 0; i < count; i++)
            {
                float startTime = _previewImageStartTimes[i];
                float endTime = startTime + singlePreviewImageTransitionDuration;
                float fromRadius = _previewImageFromRadius;
                float toRadius = _previewImageToRadius;
                if (_previewImageTransitionProgress >= startTime && _previewImageTransitionProgress <= endTime)
                {
                    float t = (_previewImageTransitionProgress - startTime) / singlePreviewImageTransitionDuration;
                    t = Mathf.Clamp01(t);
                    float angle = -ArcAngle / 2f + (ArcAngle / (count - 1)) * i;
                    float rad = (angle + 90f) * Mathf.Deg2Rad;
                    Vector2 from = new Vector2(
                        ArcCenterOffset.x + Mathf.Cos(rad) * fromRadius,
                        ArcCenterOffset.y + Mathf.Sin(rad) * fromRadius
                    );
                    Vector2 to = new Vector2(
                        ArcCenterOffset.x + Mathf.Cos(rad) * toRadius,
                        ArcCenterOffset.y + Mathf.Sin(rad) * toRadius
                    );
                    _previewImageRectTransforms[i].anchoredPosition = Vector2.Lerp(from, to, t);
                }
                else if (_previewImageTransitionProgress > endTime)
                {
                    float angle = -ArcAngle / 2f + (ArcAngle / (count - 1)) * i;
                    float rad = (angle + 90f) * Mathf.Deg2Rad;
                    Vector2 to = new Vector2(
                        ArcCenterOffset.x + Mathf.Cos(rad) * toRadius,
                        ArcCenterOffset.y + Mathf.Sin(rad) * toRadius
                    );
                    _previewImageRectTransforms[i].anchoredPosition = to;
                }
            }
            // 动画结束
            if (_previewImageTransitionProgress >= previewImageTransitionDuration)
            {
                _isPreviewImageTransitioning = false;
                if (_isChooseGameMenuFadingOut)
                {
                    // 只有在明确的离开动画时才切换到主菜单
                    _isChooseGameMenuVisible = false;
                    _isChooseGameMenuFadingOut = false;
                    backToMainMenuButton.gameObject.SetActive(false);
                    _targetMenu = Menu.MainMenu;
                    _isMainMenuFadingIn = true;
                    _currentMenu = Menu.MainMenu;
                }
            }
        }
        
        // BackToMainMenu按钮点击事件绑定（只绑定一次）
        if (backToMainMenuButton != null && backToMainMenuButton.onClick.GetPersistentEventCount() == 0)
        {
            backToMainMenuButton.onClick.AddListener(() => {
                // 开始离开 ChooseGame 菜单的动画
                EndTransitFromChooseGame();
                backToMainMenuButton.enabled = false;
                // 不要立即切换菜单，等待动画完成
            });
        }
        
        if (_isChooseGameMenuVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            // 返回主菜单，开始淡出 chooseGame 菜单
            _isChooseGameMenuVisible = false;
            _isChooseGameMenuFadingIn = false;
            _isPreviewImageTransitioning = false;
            EndTransitFromChooseGame(); // 淡出 tip
            backToMainMenuButton.enabled = false;
            backToMainMenuButton.gameObject.SetActive(false);
            // 预览图和其它元素也可在此处处理淡出逻辑
            _targetMenu = Menu.MainMenu;
            _isMainMenuFadingIn = true;
            _currentMenu = Menu.MainMenu;
        }
        
        goto End;
        
        End:
        return;

        void BeginTransitToMainMenu()
        {
            exitGameButton.gameObject.SetActive(true);
            chooseGameButton.gameObject.SetActive(true);
            trainModelButton.gameObject.SetActive(true);
            _isMainMenuFadingIn = true;
        }
        void BeginTransitToChooseGame()
        {
            backToMainMenuButton.gameObject.SetActive(true);
            _isChooseGameMenuFadingIn = true;
            _isPreviewImageTransitioning = true;
            _previewImageTransitionProgress = 0f;
            _isPreviewImageShowing = true;
            _previewImageFromRadius = HidingArcRadius;
            _previewImageToRadius = ArcRadius;
            // 淡入 chooseGameMenuTipText
            chooseGameMenuTipText.gameObject.SetActive(true);
            chooseGameMenuTipText.color = ChangeColorAlpha(chooseGameMenuTipText.color, 0f);
            _isChooseGameMenuTipFadingIn = true;
            _isChooseGameMenuTipVisible = false;
        }
        void EndTransitFromChooseGame()
        {
            // 设置离开标志
            _isChooseGameMenuFadingOut = true;
            // 淡出 chooseGameMenuTipText
            _isChooseGameMenuTipFadingOut = true;
            // 预览图隐藏动画
            _isPreviewImageTransitioning = true;
            _previewImageTransitionProgress = 0f;
            _isPreviewImageShowing = false;
            _previewImageFromRadius = ArcRadius;
            _previewImageToRadius = HidingArcRadius;
        }
    }

    public void GotoChooseGameMenu()
    {
        _targetMenu = Menu.ChooseGame;
        _isMainMenuFadingOut = true;
    }

    private void StartLeaveMainMenu()
    {
        trainModelButton.enabled = false;
        chooseGameButton.enabled = false;
        exitGameButton.enabled = false;
    }

    private void EnterMainMenu()
    {
        _isMainMenuFadingIn = false;
        _isMainMenuVisible = true;
        trainModelButton.enabled = true;
        chooseGameButton.enabled = true;
        exitGameButton.enabled = true;
    }
    
    private Color ChangeColorAlpha(Color color, float alpha = 0f)
    {
        color.a = alpha;
        return color;
    }

    [System.Serializable]
    public class GamePreview
    {
        public string gameName; // Name of the game
        public Sprite previewImage; // Preview image for the game
        public Vector3 scale = Vector3.one; // Scale of the preview image
    }

    private enum Menu
    {
        AnyKeyOrButton,
        MainMenu,
        ChooseGame
    }

    public void JumpToMainMenuDirectly()
    {
        // 重置所有状态标志
        _isAnyKeyOrButtonPressed = true;
        _isPressAnyKeyOrButtonTextFading = false;
        _isPressAnyKeyOrButtonTextVisible = false;
        _isMainMenuVisible = true;
        _isMainMenuFadingIn = false;
        _isMainMenuFadingOut = false;
        _isChooseGameMenuVisible = false;
        _isChooseGameMenuFadingIn = false;
        _isChooseGameMenuFadingOut = false;
        _isPreviewImageTransitioning = false;
        _isChooseGameMenuTipFadingIn = false;
        _isChooseGameMenuTipFadingOut = false;
        _isChooseGameMenuTipVisible = false;
        
        // 设置当前菜单状态
        _currentMenu = Menu.MainMenu;
        _targetMenu = Menu.MainMenu;
        
        // 隐藏所有非主菜单元素
        pressAnyKeyOrButtonText.gameObject.SetActive(false);
        backToMainMenuButton.gameObject.SetActive(false);
        chooseGameMenuTipText.gameObject.SetActive(false);
        
        // 显示并启用主菜单按钮
        chooseGameButton.gameObject.SetActive(true);
        exitGameButton.gameObject.SetActive(true);
        trainModelButton.gameObject.SetActive(true);
        
        chooseGameButton.enabled = true;
        exitGameButton.enabled = true;
        trainModelButton.enabled = true;
        
        // 设置主菜单按钮为完全可见
        _chooseGameButtonImage.color = ChangeColorAlpha(_chooseGameButtonImage.color, 1f);
        _exitGameButtonImage.color = ChangeColorAlpha(_exitGameButtonImage.color, 1f);
        _trainModelButtonImage.color = ChangeColorAlpha(_trainModelButtonImage.color, 1f);
        chooseGameButtonText.color = ChangeColorAlpha(chooseGameButtonText.color, 1f);
        exitGameButtonText.color = ChangeColorAlpha(exitGameButtonText.color, 1f);
        trainModelButtonText.color = ChangeColorAlpha(trainModelButtonText.color, 1f);
        
        // 重置预览图到隐藏位置
        if (_previewImageRectTransforms != null)
        {
            int count = _previewImageRectTransforms.Length;
            for (int i = 0; i < count; i++)
            {
                if (_previewImageRectTransforms[i] != null)
                {
                    float angle = -ArcAngle / 2f + (ArcAngle / (count - 1)) * i;
                    float rad = (angle + 90f) * Mathf.Deg2Rad;
                    Vector2 pos = new Vector2(
                        ArcCenterOffset.x + Mathf.Cos(rad) * HidingArcRadius,
                        ArcCenterOffset.y + Mathf.Sin(rad) * HidingArcRadius
                    );
                    _previewImageRectTransforms[i].anchoredPosition = pos;
                }
            }
        }
    }
}
