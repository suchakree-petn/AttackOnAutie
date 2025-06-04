using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System.Runtime.InteropServices;

public class JsonConverter
{
  [DllImport("__Internal")]
  public static extern void SyncFS();
  public const string PROJECTFOLDER = "ProGaming";

  #region SaveJSON
  /// <summary>
  /// For save class data to json file. Can select to save it in StreamingAssets folder or not.
  /// </summary>
  /// <param name="fileName"></param>
  /// <param name="saveObject"></param>
  /// <param name="streaming"></param>
  //TODO async
  public static void SaveJSONAsObject(string fileName, object saveObject, bool streaming = false, JsonSerializerSettings jsonSerializerSettings = null)
  {
    string data = JsonConvert.SerializeObject(saveObject, jsonSerializerSettings);

    if (streaming)
      CreateStreamingJSON(fileName, data); // if save it to StreamingAssets folder. it should be game data
    else
      CreateUserJSON(fileName, data);  // if not. it should be a user data. ex. save game data.
  }

  /// <summary>
  /// Create a json file in StreamingAssets folder.
  /// </summary>
  /// <param name="fileName"></param>
  /// <param name="data"></param>
  static void CreateStreamingJSON(string fileName, string data)
  {
    if (Application.isEditor)
    {
      StreamWriter writer;
      FileInfo t = new FileInfo($"{Application.streamingAssetsPath}/{fileName}.json");
      if (!t.Exists)
        t.Directory.Create();
      else
        t.Delete();

      writer = t.CreateText();
      writer.Write(data);
      writer.Close();
    }
  }

  /// <summary>
  /// Create a json file which is a save game data in user computer. but not in StreamingAssets folder.
  /// </summary>
  /// <param name="fileName"></param>
  /// <param name="data"></param>
  public static void CreateUserJSON(string fileName, string data)
  {
    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
    {
      StreamWriter writer;
      FileInfo t = new FileInfo(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json");

      if (!t.Exists)
        t.Directory.Create();
      else
        t.Delete();

      writer = t.CreateText();
      writer.Write(data);
      writer.Close();
    }
    else if (Application.isEditor)
    {
      StreamWriter writer;
      FileInfo t = new FileInfo(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json");

      if (!t.Exists)
        t.Directory.Create();
      else
        t.Delete();

      writer = t.CreateText();
      writer.Write(data);
      writer.Close();
    }
  }


  public static void SaveByte(string fileName, byte[] data)
  {
    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
    {
      File.WriteAllBytes(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".dat", data);
    }
    else if (Application.isEditor)
    {
      File.WriteAllBytes(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".dat", data);
    }
  }

  public static byte[] LoadByte(string fileName)
  {
    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
    {
      return File.ReadAllBytes(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".dat");
    }
    else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
    {
      return File.ReadAllBytes(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".dat");

    }

    return new byte[0];
  }

  public static void SaveText(string fileName, string data)
  {
    if (Application.platform == RuntimePlatform.WebGLPlayer)
    {
      string directoryPath = Application.persistentDataPath + "/" + PROJECTFOLDER;
      string filePath = directoryPath + "/" + fileName + ".json";

      try
      {
        if (!Directory.Exists(directoryPath))
        {
          Directory.CreateDirectory(directoryPath);
        }
        File.WriteAllText(filePath, data);
      }
      catch (IOException ex)
      {
        Debug.LogError("Error writing file: " + ex.Message);
      }


      SyncFS();
    }
    else if (Application.isEditor)
    {
      File.WriteAllText(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json", data);
    }
  }

  public static string LoadText(string fileName)
  {
    if (Application.platform == RuntimePlatform.WebGLPlayer)
    {
      return File.ReadAllText(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json");
    }
    else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
    {
      return File.ReadAllText(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json");

    }

    return "";
  }


  /// <summary>
  /// For Delete user data in user computer. ex. Save game
  /// </summary>
  /// <param name="fileName"></param>
  static void DeleteUserJSON(string fileName)
  {
    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
    {
      FileInfo t = new FileInfo(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json");

      if (t.Exists)
        t.Delete();
    }
    else if (Application.isEditor)
    {
      FileInfo t = new FileInfo(Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json");

      if (t.Exists)
        t.Delete();
    }
  }
  #endregion

  #region LoadJSON
  /// <summary>
  /// Load json file as object and that file should in StreamingAssets folder.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="fileName"></param>
  /// <returns></returns>
  public static async UniTask<T> LoadJSONAsObject<T>(string fileName, JsonSerializerSettings serializerSettings = null)
  {
    var data = await LoadTextAppData(fileName);
    return DeserializeTextToObject<T>(data, serializerSettings);
  }

  /// <summary>
  /// For Load user data into Object.
  /// Ex. Save game data.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="fileName"></param>
  /// <returns></returns>
  public static async UniTask<T> LoadUserJSONAsObject<T>(string fileName, JsonSerializerSettings serializerSettings = null)
  {
    var data = await LoadTextUserData(fileName);

    return DeserializeTextToObject<T>(data, serializerSettings);
  }

  /// <summary>
  /// For Load user data into Text.
  /// Ex. Save game data.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="fileName"></param>
  /// <returns></returns>
  static async UniTask<string> LoadTextUserData(string fileName)
  {
    string filePath;

    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
    {
      filePath = Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json";

      if (File.Exists(filePath))
      {
        StreamReader reader = File.OpenText(filePath);

        if (reader != null)
        {
          string data = await reader.ReadToEndAsync();
          reader.Close();
          return data;
        }
      }
    }
    else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
    {
      filePath = Application.persistentDataPath + "/" + PROJECTFOLDER + "/" + fileName + ".json"; //Folder to save  use data when play in editor

      if (File.Exists(filePath))
      {
        StreamReader reader = File.OpenText(filePath);

        if (reader != null)
        {
          string data = await reader.ReadToEndAsync();
          reader.Close();
          return data;
        }
      }
    }

    return "";
  }

  /// <summary>
  /// For Load game data into Text.
  /// Ex. Item data in game.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="fileName"></param>
  /// <returns></returns>
  static async UniTask<string> LoadTextAppData(string fileName)
  {
    string filePath;

    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
    {
      filePath = $"{Application.streamingAssetsPath}/{fileName}.json";

      if (File.Exists(filePath))
      {
        StreamReader reader = File.OpenText(filePath);

        if (reader != null)
        {
          string data = await reader.ReadToEndAsync();
          reader.Close();
          return data;
        }
      }
    }
    else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
    {
      filePath = $"{Application.streamingAssetsPath}/{fileName}.json";

      if (File.Exists(filePath))
      {
        StreamReader reader = File.OpenText(filePath);

        if (reader != null)
        {
          string data = await reader.ReadToEndAsync();
          reader.Close();
          return data;
        }
      }
    }
    else if (Application.platform == RuntimePlatform.WebGLPlayer)
    {
      var dataRes = await Resources.LoadAsync<TextAsset>($"Data/{fileName}");

      if (dataRes is TextAsset textAsset)
      {
        return textAsset.text;
      }

    }

    return "";
  }

  public static T DeserializeTextToObject<T>(string data, JsonSerializerSettings serializerSettings = null)
  {
    try
    {
      return JsonConvert.DeserializeObject<T>(data, serializerSettings);
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
      throw new Exception("Error Deserialize Text to Object");
    }
  }
  #endregion
}
