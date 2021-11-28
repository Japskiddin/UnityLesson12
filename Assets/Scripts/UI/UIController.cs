using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text healthLabel; // ��������� �� UI ������ � �����
    [SerializeField] private InventoryPopup popup;
    [SerializeField] private Text levelEnding;
    [SerializeField] private SettingsPopup settingsPopup;

    private void Awake()
    { // ������ ���������� ��� ������� ���������� ��������
        Messenger.AddListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.AddListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.AddListener(GameEvent.GAME_COMPLETE, OnGameComplete);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.RemoveListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.RemoveListener(GameEvent.GAME_COMPLETE, OnGameComplete);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnHealthUpdated();
        levelEnding.gameObject.SetActive(false);
        popup.gameObject.SetActive(false); // �������������� ����������� ���� ��������
        settingsPopup.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        { // ���������� ���������� ���� �� ������� M
            settingsPopup.gameObject.SetActive(false);
            bool isShowing = popup.gameObject.activeSelf;
            popup.gameObject.SetActive(!isShowing);
            popup.Refresh();
        } else if (Input.GetKeyDown(KeyCode.N))
        {
            popup.gameObject.SetActive(false);
            bool isShowing = settingsPopup.gameObject.activeSelf;
            settingsPopup.gameObject.SetActive(!isShowing);
            settingsPopup.Refresh();
        }
    }

    private void OnGameComplete()
    {
        levelEnding.gameObject.SetActive(true);
        levelEnding.text = "You finished the Game!";
    }

    private void OnHealthUpdated()
    { // ��������� ������� �������� ������� ��� ���������� ����� health
        string message = "Health: " + Managers.Player.health + " / " + Managers.Player.maxHealth;
        healthLabel.text = message;
    }

    private void OnLevelComplete()
    {
        StartCoroutine(CompleteLevel());
    }

    private IEnumerator CompleteLevel()
    {
        levelEnding.gameObject.SetActive(true);
        levelEnding.text = "Level Complete!";

        yield return new WaitForSeconds(2); // � ������� ���� ������ ���������� ���������, � ����� ��������� �� ��������� �������

        Managers.Mission.GoToNext();
    }

    private void OnLevelFailed()
    {
        StartCoroutine(FailLevel());
    }

    private IEnumerator FailLevel()
    {
        levelEnding.gameObject.SetActive(true);
        levelEnding.text = "Level Failed"; // ���������� �� �� ����� �������� ������, �� � ������ ����������

        yield return new WaitForSeconds(2);

        Managers.Player.Respawn();
        Managers.Mission.RestartCurrent(); // ����� ������������� ����� �������� ������� ������� �������
    }

    public void SaveGame()
    {
        Managers.Data.SaveGameState();
    }

    public void LoadGame()
    {
        Managers.Data.LoadGameState();
    }
}
