using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class NetworkService
{
    // url адрес для отправки запроса
    private const string xmlApi = "http://api.openweathermap.org/data/2.5/weather?q=Ulan-Ude,ru&mode=xml&APPID=d703f9ab2e2ae738eea812a5dda8b647";
    private const string jsonApi = "http://api.openweathermap.org/data/2.5/weather?q=Ulan-Ude,ru&APPID=d703f9ab2e2ae738eea812a5dda8b647";
    private const string webImage = "http://upload.wikimedia.org/wikipedia/commons/c/c5/Moraine_Lake_17092005.jpg";
    private const string localApi = "http://localhost:8080/uia/api.php";

    private IEnumerator CallAPI(string url, WWWForm form, Action<string> callback) {
        // запрос POST с использованием объекта WWWForm или запос GET без этого объекта
        using (UnityWebRequest request = (form == null) ? UnityWebRequest.Get(url) : UnityWebRequest.Post(url, form)) { // создаём UnityWebRequest в режиме GET
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError) { // проверяем ответ на наличие ошибок
                Debug.LogError("Network problem: " + request.error);
            } else if (request.responseCode != (long) System.Net.HttpStatusCode.OK) {
                Debug.LogError("Response error: " + request.responseCode);
            } else {
                callback(request.downloadHandler.text); // делегат можно вызвать так же, как и исходную функцию
            }
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback) { // каскад ключевых слов yield в вызывающих друг друга методах сопрограммы
        return CallAPI(xmlApi, null, callback);
    }

    public IEnumerator GetWeatherJSON(Action<string> callback) {
        return CallAPI(jsonApi, null, callback);
    }

    public IEnumerator LogWeather(string name, float cloudValue, Action<string> callback) {
        WWWForm form = new WWWForm(); // определяем форму с аргументами для отправки
        form.AddField("message", name);
        form.AddField("cloud_value", cloudValue.ToString());
        form.AddField("timestamp", DateTime.UtcNow.Ticks.ToString()); // отправляем метку времени вместе с информацией об облачности

        return CallAPI(localApi, form, callback);
    }

    public IEnumerator DownloadImage(Action<Texture2D> callback) { // вместо строки этот обратный вызов принимает объекты Texture2D
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(webImage);
        yield return request.SendWebRequest();
        callback(DownloadHandlerTexture.GetContent(request)); // получаем загруженное изображение с помощью служебной программы DownloadHandler
    }
}
