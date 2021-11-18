using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager
{
    ManagerStatus status { get; } // перечисление, которое нужно обрабатывать
    void Startup();
}
