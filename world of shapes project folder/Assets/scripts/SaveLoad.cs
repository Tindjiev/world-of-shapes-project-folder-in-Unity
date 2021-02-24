using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



public static class io
{
    public static string saveExtension { get; } = ".shapesave";
    public static string savespath
    {
        get
        {
            {
                return savespathfolder + "/";
            }
        }
    }
    public static string savespathfolder
    {
        get
        {
            {
                return Application.dataPath;
            }
        }
    }

    public static string buildStringPath(string filename)
    {
        return savespath + filename + saveExtension;
    }

    public static System.IO.DirectoryInfo createSsaveFolder()
    {
        if (!System.IO.Directory.Exists(savespathfolder))
        {
            return System.IO.Directory.CreateDirectory(savespathfolder);
        }
        else
        {
            return System.IO.Directory.CreateDirectory(savespathfolder);
        }
    }

    public static void Save(string wavepath, object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(wavepath, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static DataType Load<DataType>(string path) where DataType : DataClassBase
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                return null;
            }
            DataType temp = (DataType)formatter.Deserialize(stream);
            stream.Close();
            return temp;
        }
        return null;
    }



}

public interface ISavable
{

    string GetStringPath();

    void Save();

    void Load();



}


[System.Serializable]
public abstract class DataClassBase
{

}