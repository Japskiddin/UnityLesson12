using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    private string _filename;
    private NetworkService _network;

    public void Startup(NetworkService service)
    {
        Debug.Log("Data manager starting...");

        _network = service;
        _filename = Path.Combine(Application.persistentDataPath, "game.dat"); // генерируем полный путь к файлу game.dat
        status = ManagerStatus.Started;
    }

    public void SaveGameState()
    {
        Dictionary<string, object> gamestate = new Dictionary<string, object>
        {
            { "inventory", Managers.Inventory.GetData() },
            { "health", Managers.Player.health },
            { "maxHealth", Managers.Player.maxHealth },
            { "curLevel", Managers.Mission.curLevel },
            { "maxLevel", Managers.Mission.maxLevel }
        }; // словарь, который будет подвергнут сериализации

        FileStream stream = File.Create(_filename); // создадим файл по указанному пути
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, gamestate); // сериализуем объект Dictionary как содержимое созданного файла
        stream.Close();
    }

    public void LoadGameState()
    {
        if (!File.Exists(_filename)) // продолжить загрузку только при наличии файла
        {
            Debug.Log("No saved game");
            return;
        }

        Dictionary<string, object> gamestate; // словарь для размещения загруженных данных
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = File.Open(_filename, FileMode.Open);
        gamestate = formatter.Deserialize(stream) as Dictionary<string, object>;
        stream.Close();

        Managers.Inventory.UpdateData((Dictionary<string, int>)gamestate["inventory"]);
        Managers.Player.UpdateData((int)gamestate["health"], (int)gamestate["maxHealth"]);
        Managers.Mission.UpdateData((int)gamestate["curLevel"], (int)gamestate["maxLevel"]);
        Managers.Mission.RestartCurrent();
    }
}