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
    [SerializeField] private TMP_Text userNameTxt;
    [FoldoutGroup("Login Panel")]
    [SerializeField] private Button googleLoginButton, guestLoginButton;
    [FoldoutGroup("Login Panel")]
    [SerializeField] private GameObject loginPanelGroup;

    [FoldoutGroup("Profile")]
    [SerializeField] private Image profilePic;
    [FoldoutGroup("Profile")]
    [SerializeField] private GameObject profileGroup;

    [FoldoutGroup("Main Menu")]
    [SerializeField] private Button howToPlayButton, playGameButton;

    [FoldoutGroup("Main Menu")]
    [SerializeField] private GameObject mainMenuPanelGroup;

    [FoldoutGroup("How To Play")]
    [SerializeField] private GameObject howToPlayPanelGroup;

    [FoldoutGroup("How To Play")]
    [SerializeField] private Button closeHowToPlayButton;


    private Texture2D profilePicTexture;

    void Awake()
    {
        howToPlayPanelGroup.SetActive(false);
        howToPlayButton.onClick.AddListener(() =>
        {
            howToPlayPanelGroup.SetActive(true);
        });
        closeHowToPlayButton.onClick.AddListener(() =>
        {
            howToPlayPanelGroup.SetActive(false);
        });

        playGameButton.onClick.AddListener(() => SceneLoader.LoadSceneAsync("Gameplay"));
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
            await AuthenticationWrapper.Instance.LoginGoogle();
        });
        guestLoginButton.onClick.AddListener(async () =>
        {
            await AuthenticationWrapper.Instance.LoginGuest();
        });
    }


    void OnDestroy()
    {
        AuthenticationWrapper.OnLogin -= OnLoginHandler;
        AuthenticationWrapper.OnLogout -= OnLogoutHandler;

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

        profileGroup.SetActive(true);
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
