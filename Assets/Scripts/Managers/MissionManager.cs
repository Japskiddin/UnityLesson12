using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public int curLevel { get; private set; }
    public int maxLevel { get; private set; }
    private NetworkService _network;

    public void Startup(NetworkService service)
    {
        Debug.Log("Mission manager starting...");
        _network = service;
        UpdateData(0, 1);

        status = ManagerStatus.Started;
    }

    public void UpdateData(int curLeve, int maxLevel)
    {
        this.curLevel = curLevel;
        this.maxLevel = maxLevel;
    }

    public void GoToNext()
    {
        if (curLevel < maxLevel)
        { // проверяем, достигнут ли последний уровень
            curLevel++;
            string name = "Level" + curLevel;
            Debug.Log("Loading " + name);
            SceneManager.LoadScene(name); // команда загрузки сцены
        } else
        {
            Debug.Log("Max level");
        }
    }

    public void ReachObjective()
    {
        // здесь может быть код обработки нескольких целей
        Messenger.Broadcast(GameEvent.LEVEL_COMPLETE);
    }

    public void RestartCurrent()
    {
        string name = "Level" + curLevel;
        Debug.Log("Loading " + name);
        SceneManager.LoadScene(name);
    }
}
