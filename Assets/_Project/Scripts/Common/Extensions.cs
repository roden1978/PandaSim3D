using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using PlayerScripts;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions
{
    public static string ToJSON(this object obj) =>
        JsonUtility.ToJson(obj);

    public static T Deserialize<T>(this string json) =>
        JsonUtility.FromJson<T>(json);

    public static Texture2D TextureFromSprite(this Sprite sprite)
    {
        Texture2D texture = new(Mathf.FloorToInt(sprite.rect.width), Mathf.FloorToInt(sprite.rect.height));
        Color[] pixels = sprite.texture.GetPixels(
            Mathf.FloorToInt(sprite.rect.xMin),
            Mathf.FloorToInt(sprite.rect.yMin),
            Mathf.FloorToInt(sprite.rect.width),
            Mathf.FloorToInt(sprite.rect.height));
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    public static Vector3 Vector3DataToVector3(this Vector3Data vector3Data)
    {
        return new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);
    }

    public static Vector3Data Vector3ToVector3Data(this Vector3 vector3)
    {
        return new Vector3Data(vector3.x, vector3.y, vector3.z);
    }

    public static int Random100()
    {
        return Random.Range(0, 101);
    }

    public static float DivideBy100ToFloat(int value)
    {
        return Convert.ToSingle(value) / 100;
    }

    public static int OneItem => 1;

    public static void GenerateEnum(this IEnumerable<string> fields, string enumName,
        string path = "Assets/Scripts/Enums/")
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
        string filePathAndName =
            path + enumName + ".cs";
        using (StreamWriter streamWriter = new(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            IEnumerable<string> enumerable = fields as string[] ?? fields.ToArray();
            for (int i = 0; i < enumerable.Count(); i++)
            {
                streamWriter.WriteLine("	" + enumerable.ElementAt(i) + $" = {i},");
            }

            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }

    public static string ParseAssetPath(this string path)
    {
        ReadOnlySpan<char> filename = path.Split("/")[^1].AsSpan();
        int pos = filename.IndexOf(".");
        return pos > -1 ? filename[..pos].ToString() : filename.ToString();
    }

    public static void SetPositionAdapterValue(this GameObject player, Vector3 position, Quaternion rotation)
    {
        IPositionAdapter positionAdapter = player.GetComponent<IPositionAdapter>();
        positionAdapter.Position = position;
        IRotationAdapter rotationAdapter = player.GetComponent<IRotationAdapter>();
        rotationAdapter.Rotation = rotation;
    }
}