using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))] // убеждаемся, что различные диспетчеры существуют
public class Managers : MonoBehaviour
{
    public static PlayerManager Player { get; private set; } // статичные свойства, которыми остальной код пользуется для доступа к диспетчерам
    public static InventoryManager Inventory { get; private set; }
    private List<IGameManager> _startSequence; // список диспетчеров, который просматривается в цикле во время стартовой последовательности

    private void Awake() {
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);

        StartCoroutine(StartupManagers()); // асинхронно загружаем стартовую последовательность
    }

    private IEnumerator StartupManagers() {
        foreach(IGameManager manager in _startSequence) {
            manager.Startup();
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
            }

            yield return null; // остановка на один кадр перед следующей проверкой
        }

        Debug.Log("All managers started up");
    }
}
