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

    public async UniTask<PlayerProgress> LoadProgress()
    {
        string jsonData = await Load(Progress, FileName);
        Debug.Log($"Json string {jsonData}");
        return jsonData?.Deserialize<PlayerProgress>();
    }

    public void SaveSettings()
    {
        string jsonData = _persistentProgress.Settings.ToJSON();
        Save(Settings, FileName, jsonData);
    }

    public async UniTask<Settings> LoadSettings()
    {
        string jsonData = await Load(Settings, FileName);
        return jsonData?.Deserialize<Settings>();
    }

    private async UniTask<string> Load(string folderName, string fileName)
    {
        string dataPath = GetFilePath(folderName, fileName);
        Debug.Log($"Load player progress path {dataPath}");
        
        if (File.Exists(dataPath))
        {
            await using FileStream reader = new (dataPath, FileMode.OpenOrCreate);
            byte[] result = new byte[reader.Length];
            await reader.ReadAsync(result, 0, (int)reader.Length);
            reader.Close();
            return Encoding.UTF8.GetString(result, 0, result.Length);
        }

        return string.Empty;
    }

    private async void Save(string folderName, string filename, string data)
    {
        string dataPath = GetFilePath(folderName, filename);
        
        Debug.Log($"Save player progress path {dataPath}");
        
        if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath) ?? string.Empty);

        if (_encryptor != null)
        {
            data = _encryptor.Encrypt(data);
        }
        await using FileStream fs = new(dataPath, FileMode.Create, FileAccess.Write);
        await fs.WriteAsync(Encoding.UTF8.GetBytes(data), 0, data.Length);
        fs.Close();
    }

    private string GetFilePath(string folderName, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"data/{folderName}");

        return Path.Combine(filePath, $"{fileName}.dat");
    }
}