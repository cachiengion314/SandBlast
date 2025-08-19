using UnityEngine;
using System.IO;
using TigerForge;
using System;

namespace HoangNam
{
  public class SaveSystem
  {
    public static string GetRawDataPathFrom(string dataName)
    {
      string dataPath;

#if UNITY_IOS
      dataPath = Application.persistentDataPath + "/" + dataName + ".json";
#endif
#if UNITY_ANDROID
      dataPath = Application.persistentDataPath + "/" + dataName + ".json";
#endif
#if UNITY_EDITOR
      dataPath = Application.dataPath + "/" + dataName + ".json";
#endif

      return dataPath;
    }

    public static string GetSaveDataPathFrom(string dataName)
    {
      string dataPath;

#if UNITY_IOS
      dataPath = Application.persistentDataPath + "/" + dataName + ".save.json";
#endif
#if UNITY_ANDROID
      dataPath = Application.persistentDataPath + "/" + dataName + ".save.json";
#endif
#if UNITY_EDITOR
      dataPath = Application.dataPath + "/" + dataName + ".save.json";
#endif

      return dataPath;
    }

    public static string GetSaveDataEncryptPathFrom(string dataName)
    {
      string dataPath;

#if UNITY_IOS
      dataPath = Application.persistentDataPath + "/" + dataName + ".dat";
#endif
#if UNITY_ANDROID
      dataPath = Application.persistentDataPath +  "/" + dataName + ".dat";
#endif
#if UNITY_EDITOR
      dataPath = Application.dataPath + "/" + dataName + ".dat";
#endif

      return dataPath;
    }

    public static void RemoveFile(string name)
    {
      var _name = GetSaveDataPathFrom(name);
      if (File.Exists(_name))
        File.Delete(_name);
    }

    public static void SaveWith<T>(T data, string dataName)
    {
      var dataPath = GetSaveDataPathFrom(dataName);
      string _data = JsonUtility.ToJson(data, true);
      File.WriteAllText(dataPath, _data);
    }

    public static T LoadWith<T>(string dataName)
    {
      T temp = default;
      var dataPath = GetSaveDataPathFrom(dataName);

      if (File.Exists(dataPath))
      {
        string data = File.ReadAllText(dataPath);
        var tData = JsonUtility.FromJson<T>(data);

        return tData;
      }
      return temp;
    }

    public static void Save<T>(T data, string dataName)
    {
      var dataPath = GetRawDataPathFrom(dataName);
      string _data = JsonUtility.ToJson(data, true);
      File.WriteAllText(dataPath, _data);
    }

    public static T Load<T>(string dataName)
    {
      T temp = default;
      var dataPath = GetRawDataPathFrom(dataName);

      if (File.Exists(dataPath))
      {
        string data = File.ReadAllText(dataPath);
        var tData = JsonUtility.FromJson<T>(data);

        return tData;
      }
      return temp;
    }

    public static void SaveAndEncrypt<T>(T data, string dataName, string password)
    {
      var myFile = new EasyFileSave(dataName);
      string _data = JsonUtility.ToJson(data);
      myFile.Add("_contentof_" + dataName, _data);
      myFile.Save(password);
    }

    public static void ChecFileExist(string dataName, Action notFoundAction = null, Action foundAction = null)
    {
      var myFile = new EasyFileSave(dataName);
      if (!File.Exists(myFile.GetFileName()))
      {
        notFoundAction?.Invoke();
      }
      else
      {
        foundAction?.Invoke();
      }
    }

    public static T LoadAndDecrypt<T>(string dataName, string password)
    {
      T temp = default;
      var myFile = new EasyFileSave(dataName);
      Debug.Log(">> Data loaded from: " + myFile.GetFileName() + "\n");
      if (myFile.Load(password))
      {
        var rawStr = myFile.GetString("_contentof_" + dataName);
        var data = JsonUtility.FromJson<T>(rawStr);
        myFile.Dispose();
        return data;
      }
      myFile.Dispose();
      return temp;
    }
  }
}