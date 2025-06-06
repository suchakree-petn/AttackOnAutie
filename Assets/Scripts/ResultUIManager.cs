using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sych.ShareAssets.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : Singleton<ResultUIManager>
{
    [SerializeField, Required] Canvas resultUI;
    [SerializeField, Required] Button shareButton, replayButton, homeButton;
    [SerializeField, Required] TextMeshProUGUI gameTimeText;

    string filePath;


    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        HideResultUI();
        filePath = Path.Combine(Application.persistentDataPath, "resultImg.png");

        shareButton.onClick.AddListener(ShareScreenShot);
        replayButton.onClick.AddListener(() => SceneLoader.LoadSceneAsync("Gameplay"));
        homeButton.onClick.AddListener(() => SceneLoader.LoadSceneAsync("Title"));
    }



    public async void ShowResultUI(bool delay = true)
    {
        if (delay)
            await UniTask.WaitForSeconds(2);

        float timeInSeconds = GameManager.Instance.GameContext.GameTime;
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        if (gameTimeText)
            gameTimeText.SetText($"Time : {minutes:D2}:{seconds:D2} m");
        if (resultUI)
            resultUI.gameObject.SetActive(true);



    }

    public void HideResultUI()
    {
        resultUI.gameObject.SetActive(false);
    }


    private async void ShareScreenShot()
    {
        shareButton.interactable = false;
        HideResultUI();
        await SaveScreenShot();
        ShowResultUI(false);
        await Share.ItemAsync(filePath);
        shareButton.interactable = true;
    }

    [Button]
    public async UniTask SaveScreenShot()
    {
        await UniTask.WaitForEndOfFrame(this);

        try
        {
            int width = Screen.width;
            int height = Screen.height;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);

            File.WriteAllBytes(filePath, bytes);

            Debug.Log("Screenshot saved to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Screenshot capture failed: " + e.Message);
        }

    }

}
