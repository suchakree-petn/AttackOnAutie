using UnityEngine;
using Sirenix.OdinInspector;
using PhEngine.GoogleSheets;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "GameConfig", fileName = "GameConfig", order = 0)]
public class GameConfig : SerializedScriptableObject
{
    [FoldoutGroup("Setting", Expanded = false)]
    [SerializeField] SheetDownloader downloader;

    [FoldoutGroup("Setting", Expanded = false)]
    [SerializeField] string spreadsheetId;
    
    [FoldoutGroup("Setting", Expanded = false)]
    [SerializeField] string sheetNameAndRange;

    public Dictionary<string, Config> Config;


    [Button("Fetch Data", Icon = SdfIconType.ArrowCounterclockwise, ButtonHeight = 30)]
    [PropertyOrder(-10)]
    [GUIColor(0.5f, 0.8f, 0.5f)]
    public void FetchDataFromGoogleSheet()
    {
        downloader
            .CreateRequest<Config>(spreadsheetId, sheetNameAndRange)
            .OnSuccess(list => Config = list.ToDictionary(config => config.Name, config => config))
            .Download();
    }



}
[Serializable]
public class Config
{
    public string Name;
    public int Amount;
    public int Damage;
    public int HP;
    public float MissedChance;
    public float Sec;

}
