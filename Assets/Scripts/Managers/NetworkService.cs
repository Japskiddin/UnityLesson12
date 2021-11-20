using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class NetworkService
{
    // url ����� ��� �������� �������
    private const string xmlApi = "http://api.openweathermap.org/data/2.5/weather?q=Ulan-Ude,ru&mode=xml&APPID=d703f9ab2e2ae738eea812a5dda8b647";
    private const string jsonApi = "http://api.openweathermap.org/data/2.5/weather?q=Ulan-Ude,ru&APPID=d703f9ab2e2ae738eea812a5dda8b647";
    private const string webImage = "http://upload.wikimedia.org/wikipedia/commons/c/c5/Moraine_Lake_17092005.jpg";
    private const string localApi = "http://localhost:8080/uia/api.php";

    private IEnumerator CallAPI(string url, WWWForm form, Action<string> callback) {
        // ������ POST � �������������� ������� WWWForm ��� ����� GET ��� ����� �������
        using (UnityWebRequest request = (form == null) ? UnityWebRequest.Get(url) : UnityWebRequest.Post(url, form)) { // ������ UnityWebRequest � ������ GET
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError) { // ��������� ����� �� ������� ������
                Debug.LogError("Network problem: " + request.error);
            } else if (request.responseCode != (long) System.Net.HttpStatusCode.OK) {
                Debug.LogError("Response error: " + request.responseCode);
            } else {
                callback(request.downloadHandler.text); // ������� ����� ������� ��� ��, ��� � �������� �������
            }
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback) { // ������ �������� ���� yield � ���������� ���� ����� ������� �����������
        return CallAPI(xmlApi, null, callback);
    }

    public IEnumerator GetWeatherJSON(Action<string> callback) {
        return CallAPI(jsonApi, null, callback);
    }

    public IEnumerator LogWeather(string name, float cloudValue, Action<string> callback) {
        WWWForm form = new WWWForm(); // ���������� ����� � ����������� ��� ��������
        form.AddField("message", name);
        form.AddField("cloud_value", cloudValue.ToString());
        form.AddField("timestamp", DateTime.UtcNow.Ticks.ToString()); // ���������� ����� ������� ������ � ����������� �� ����������

        return CallAPI(localApi, form, callback);
    }

    public IEnumerator DownloadImage(Action<Texture2D> callback) { // ������ ������ ���� �������� ����� ��������� ������� Texture2D
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(webImage);
        yield return request.SendWebRequest();
        callback(DownloadHandlerTexture.GetContent(request)); // �������� ����������� ����������� � ������� ��������� ��������� DownloadHandler
    }
}
