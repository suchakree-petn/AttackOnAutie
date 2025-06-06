using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Google;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Title_UIController : MonoBehaviour
{
    [SerializeField] private string imageURL;

    [FoldoutGroup("Login Panel")]
    [SerializeField, Required] private TMP_Text userNameTxt;
    [FoldoutGroup("Login Panel")]
    [SerializeField, Required] private Button googleLoginButton, guestLoginButton;
    [FoldoutGroup("Login Panel")]
    [SerializeField, Required] private GameObject loginPanelGroup;

    [FoldoutGroup("Profile")]
    [SerializeField, Required] private Image profilePic;
    [FoldoutGroup("Profile")]
    [SerializeField, Required] private GameObject profileGroup;

    [FoldoutGroup("Main Menu")]
    [SerializeField, Required] private Button howToPlayButton, playGameButton;

    [FoldoutGroup("Main Menu")]
    [SerializeField, Required] private GameObject mainMenuPanelGroup;

    [FoldoutGroup("How To Play")]
    [SerializeField, Required] private GameObject howToPlayPanelGroup;

    [FoldoutGroup("How To Play")]
    [SerializeField, Required] private Button closeHowToPlayButton;

    [FoldoutGroup("Game Mode")]
    [SerializeField, Required] private Button onePlayerButton, twoPlayerButton, closeGameModeButton;

    [FoldoutGroup("Game Mode")]
    [SerializeField, Required] private GameObject gameModePanelGroup;


    private Texture2D profilePicTexture;

    void Awake()
    {
        howToPlayPanelGroup.SetActive(false);
        gameModePanelGroup.SetActive(false);

        howToPlayButton.onClick.AddListener(() =>
        {
            howToPlayPanelGroup.SetActive(true);
        });
        closeHowToPlayButton.onClick.AddListener(() =>
        {
            howToPlayPanelGroup.SetActive(false);
        });
        closeGameModeButton.onClick.AddListener(HideGameModeSelectPanel);

        playGameButton.onClick.AddListener(ShowGameModeSelectPanel);

        onePlayerButton.onClick.AddListener(() => OnSelectGameMode(GameMode.OnePlayer));
        twoPlayerButton.onClick.AddListener(() => OnSelectGameMode(GameMode.TwoPlayer));

    }


    private void Start()
    {
        if (AuthenticationWrapper.Instance.IsLogin)
        {
            UpdateProfileUI(AuthenticationWrapper.Instance.User).Forget();
            mainMenuPanelGroup.SetActive(true);
            loginPanelGroup.SetActive(false);
            profileGroup.SetActive(true);
        }
        else
        {
            mainMenuPanelGroup.SetActive(false);
            loginPanelGroup.SetActive(true);
            profileGroup.SetActive(false);
        }



        AuthenticationWrapper.OnLogin += OnLoginHandler;
        AuthenticationWrapper.OnLogout += OnLogoutHandler;

        googleLoginButton.onClick.AddListener(async () =>
        {
            Debug.Log("click login google");
            await AuthenticationWrapper.Instance.LoginGoogle();
        });
        guestLoginButton.onClick.AddListener(async () =>
        {
            Debug.Log("click login guest");
            await AuthenticationWrapper.Instance.LoginGuest();
        });
    }


    void OnDestroy()
    {
        AuthenticationWrapper.OnLogin -= OnLoginHandler;
        AuthenticationWrapper.OnLogout -= OnLogoutHandler;

    }


    private void ShowGameModeSelectPanel()
    {
        gameModePanelGroup.SetActive(true);
    }

    private void HideGameModeSelectPanel()
    {
        gameModePanelGroup.SetActive(false);
    }

    private void OnSelectGameMode(GameMode gameMode)
    {
        GameManager.Instance.GameContext.GameMode = gameMode;
        SceneLoader.LoadSceneAsync("Gameplay");
    }


    private void OnLoginHandler(UserInfo userInfo)
    {
        UpdateProfileUI(userInfo).Forget();
        mainMenuPanelGroup.SetActive(true);
        loginPanelGroup.SetActive(false);
        profileGroup.SetActive(true);
    }


    private void OnLogoutHandler()
    {
        mainMenuPanelGroup.SetActive(false);
        loginPanelGroup.SetActive(true);
        profileGroup.SetActive(false);

        if (userNameTxt)
            userNameTxt.text = "";

        imageURL = "";
    }

    async UniTask UpdateProfileUI(UserInfo user)
    {
        Debug.Log("Welcome: " + user.DisplayName + "!");
        if (userNameTxt)
            userNameTxt.text = user.DisplayName;

        await UpdateProfiePicture(user.ImageUrl);

        if (profileGroup)
            profileGroup.SetActive(true);
        if (loginPanelGroup)
            loginPanelGroup.SetActive(false);
    }

    private async UniTask UpdateProfiePicture(string imageURL)
    {
        if (this.imageURL != imageURL)
        {
            this.imageURL = imageURL;
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);

            await request.SendWebRequest();

            profilePicTexture = DownloadHandlerTexture.GetContent(request);
        }
        Rect rect = new(0, 0, profilePicTexture.width, profilePicTexture.height);
        Vector2 pivot = new(0.5f, 0.5f);
        if (profilePic)
            profilePic.sprite = Sprite.Create(profilePicTexture, rect, pivot);
    }
}
