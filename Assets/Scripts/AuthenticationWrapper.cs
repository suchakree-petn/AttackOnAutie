using UnityEngine;
using Google;
using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Random = UnityEngine.Random;

public class AuthenticationWrapper
{

    private const string webClientId = "1082955021109-d47vtet8dts6iabqhogopf3gfo955rpf.apps.googleusercontent.com";

    private static AuthenticationWrapper instance;

    public static AuthenticationWrapper Instance
    {
        get
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                instance = new();
                instance.InitGoogleSignIn();
                instance.InitUnityServices();
                OnLogin += (_) => instance.IsLogin = true;
                OnLogout += () => instance.IsLogin = false;
            }
            return instance;
        }
    }

    public bool IsLogin { get; private set; }

    public UserInfo User { get; private set; }

    public static event Action<UserInfo> OnLogin;
    public static event Action OnLogout;

    private void InitGoogleSignIn()
    {
        GoogleSignIn.Configuration ??= new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = webClientId
        };
    }

    private void InitUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
            UnityServices.InitializeAsync();
    }

    public async UniTask LoginGoogle()
    {

#if PLATFORM_ANDROID && !UNITY_EDITOR
        UniTask<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn().AsUniTask();
        Debug.Log("Logging in");
        await signIn.ContinueWith(task =>
        {
            User = new(task);
            OnLogin?.Invoke(User);
        });
#else
        await UniTask.CompletedTask;
        User = new("Tutor (test in editor)", "https://lh3.googleusercontent.com/a/ACg8ocI_CN5PPYN9i2RXhUid5VicZ4C8zisRP18QR2DBgTAlba6wDg=s96-c");
        OnLogin?.Invoke(User);
#endif


    }

    public async UniTask LoginGuest()
    {
        await UniTask.WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized);

        try
        {
            Debug.Log("Start sign in anonymous");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("signed in anonymous!!");
            User = new("Guest_" + Random.Range(0, 999), "https://img.freepik.com/premium-vector/avatar-guest-vector-icon-illustration_1304166-97.jpg");
            OnLogin?.Invoke(User);
        }
        catch (AuthenticationException authException)
        {
            Debug.LogException(authException);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }


    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        OnLogout?.Invoke();
    }

}


public struct UserInfo
{
    public string DisplayName { get; private set; }
    public string ImageUrl { get; private set; }

    public UserInfo(string displayName, string imageUrl)
    {
        DisplayName = displayName;
        ImageUrl = imageUrl;
    }

    public UserInfo(GoogleSignInUser googleSignInUser)
    {
        DisplayName = googleSignInUser.DisplayName;
        ImageUrl = googleSignInUser.ImageUrl.ToString();
    }
}
