using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))] // убеждаемся, что различные диспетчеры существуют
[RequireComponent(typeof(MissionManager))]
[RequireComponent(typeof(DataManager))]
public class Managers : MonoBehaviour
{
    public static PlayerManager Player { get; private set; } // статичные свойства, которыми остальной код пользуется для доступа к диспетчерам
    public static InventoryManager Inventory { get; private set; }
    public static MissionManager Mission { get; private set; }
    public static DataManager Data { get; private set; }
    private List<IGameManager> _startSequence; // список диспетчеров, который просматривается в цикле во время стартовой последовательности

    private void Awake() {
        DontDestroyOnLoad(gameObject); // команда Unity для сохранения объекта между сценами

        Data = GetComponent<DataManager>();
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        Mission = GetComponent<MissionManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(Mission);
        _startSequence.Add(Data);

        StartCoroutine(StartupManagers()); // асинхронно загружаем стартовую последовательность
    }

    private IEnumerator StartupManagers() {
        NetworkService network = new NetworkService();

        foreach(IGameManager manager in _startSequence) {
            manager.Startup(network);
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while(numReady < numModules) { // продолжаем цикл, пока не начнут работать все диспетчеры
            int lastReady = numReady;
            numReady = 0;

            foreach(IGameManager manager in _startSequence) {
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }

            if (numReady > lastReady) {
                Debug.Log("Progress: " + numReady + "/" + numModules);
                Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules); // событие загрузки рассылается вместе с параметрами
            }

            yield return null; // остановка на один кадр перед следующей проверкой
        }

        Debug.Log("All managers started up");
        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED); // событие загрузки рассылается без параметров
    }
}
