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
    [SerializeField, Required] TextMeshProUGUI winnerText, gameTimeText;

    string filePath;

    GameContext gameContext;

    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        gameContext = GameManager.Instance.GameContext;

        HideResultUI();
        filePath = Path.Combine(Application.persistentDataPath, "resultImg.png");

        shareButton.onClick.AddListener(ShareScreenShot);
        replayButton.onClick.AddListener(() => SceneLoader.LoadSceneAsync("Gameplay"));
        homeButton.onClick.AddListener(() => SceneLoader.LoadSceneAsync("Title"));
    }



    public async void ShowResultUI(PlayerIndex winner, bool delay = true)
    {
        if (delay)
            await UniTask.WaitForSeconds(2);

        if (gameTimeText)
        {
            float timeInSeconds = gameContext.GameTime;
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            gameTimeText.SetText($"Time : {minutes:D2}:{seconds:D2} m");
        }

        if (winnerText)
        {
            string resultText = (winner == PlayerIndex.Player1 && gameContext.GameMode == GameMode.OnePlayer) ? "You Lose" : "You Win";
            winnerText.SetText(resultText);
        }

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
        if (resultUI)
            resultUI.gameObject.SetActive(false);
        await SaveScreenShot();
        if (resultUI)
            resultUI.gameObject.SetActive(true);
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
