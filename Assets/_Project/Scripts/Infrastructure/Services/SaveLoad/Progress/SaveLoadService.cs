using System;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Services.SaveLoad.PlayerProgress;
using UnityEngine;

public class SaveLoadService : ISaveLoadService
{
    private const string Progress = "Progress";
    private const string Settings = "Settings";
    private const string FileName = "data";

    private readonly IPersistentProgress _persistentProgress;
    private readonly ISaveLoadStorage _saveLoadStorage;
    private readonly EmptyEncryptor _encryptor;

    public SaveLoadService(IPersistentProgress persistentProgress, ISaveLoadStorage saveLoadStorage)
    {
        _persistentProgress = persistentProgress;
        _saveLoadStorage = saveLoadStorage;
        _encryptor = new EmptyEncryptor();
    }

    public void SaveProgress()
    {
        Debug.Log($"Savers count: {_saveLoadStorage.Savers.Count()}");
        foreach (ISavedProgress progressWriter in _saveLoadStorage.Savers)
        {
            progressWriter.SaveProgress(_persistentProgress.PlayerProgress);
        }

        string jsonData = _persistentProgress.PlayerProgress.ToJSON();
        Save(Progress, FileName, jsonData);
    }

    public PlayerProgress LoadProgress()
    {
        string jsonData = Load(Progress, FileName);
        Debug.Log($"Json string {jsonData}");
        return jsonData?.Deserialize<PlayerProgress>();
    }

    public void SaveSettings()
    {
        string jsonData = _persistentProgress.Settings.ToJSON();
        Save(Settings, FileName, jsonData);
    }

    public Settings LoadSettings()
    {
        string jsonData = Load(Settings, FileName);
        return jsonData?.Deserialize<Settings>();
    }

    public void Delete()
    {
        string progress = GetFilePath(Progress, FileName);
        string soundSettings = GetFilePath(Settings, FileName);

        if (File.Exists(progress))
            File.Delete(progress);
        
        if (File.Exists(soundSettings))
            File.Delete(soundSettings);
    }

    private string Load(string folderName, string fileName)
    {
        string dataPath = GetFilePath(folderName, fileName);
        Debug.Log($"Load player progress path {dataPath}");

        if (File.Exists(dataPath))
        {
            using FileStream reader = new(dataPath, FileMode.OpenOrCreate, FileAccess.Read,
                FileShare.ReadWrite);
            byte[] result = new byte[reader.Length];
            int read = reader.Read(result, 0, (int)reader.Length);
            reader.Close();
            return Encoding.UTF8.GetString(result, 0, read);
        }

        return string.Empty;
    }

    private void Save(string folderName, string filename, string data)
    {
        string dataPath = GetFilePath(folderName, filename);

        Debug.Log($"Save player progress path {dataPath}");

        if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath) ?? string.Empty);

        if (_encryptor != null)
        {
            data = _encryptor.Encrypt(data);
        }

        using FileStream fs = new(dataPath, FileMode.Create, FileAccess.Write);
        fs.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
        fs.Close();
    }

    private string GetFilePath(string folderName, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"data/{folderName}");

        return Path.Combine(filePath, $"{fileName}.dat");
    }
}