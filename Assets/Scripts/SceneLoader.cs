using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public static class SceneLoader
{
    private const string _loadingScene = "LoadingScene";
    private static AsyncOperation _loadingOperation;
    private static bool _isLoading = false;

    public static void LoadSceneAsync(string targetScene)
    {
        if (_isLoading)
            return;

        _isLoading = true;
        SceneManager.sceneLoaded += OnLoadingSceneLoaded;
        SceneManager.LoadScene(_loadingScene, LoadSceneMode.Additive);

        void OnLoadingSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != _loadingScene) return;

            SceneManager.sceneLoaded -= OnLoadingSceneLoaded;
            LoadMainSceneAsync(targetScene).Forget();
        }
    }

    private static async UniTaskVoid LoadMainSceneAsync(string targetScene)
    {
        _loadingOperation = SceneManager.LoadSceneAsync(targetScene);
        _loadingOperation.allowSceneActivation = false;

        while (_loadingOperation.progress < 0.9f)
            await UniTask.Yield();

        FinishLoading();
    }

    private static void FinishLoading()
    {

        if (_loadingOperation != null)
        {
            _loadingOperation.allowSceneActivation = true;
            _loadingOperation = null;
            _isLoading = false;
        }
    }
}
