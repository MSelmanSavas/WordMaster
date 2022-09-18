using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetConnectionManager : MonoBehaviour
{
    public static NetConnectionManager Instance;

    [SerializeField]
    public NetworkReachability CurrentNetworkStatus;

    [SerializeField]
    public string lastRecordedDeviceDateTimeString;
    [SerializeField]
    public DateTime lastRecordedDeviceDateTime;

    [SerializeField]
    public string deviceDateTimeString;
    public DateTime deviceDateTime;

    [SerializeField]
    public string realLocalDateTimeString;
    public DateTime realLocalDateTime;

    CancellationTokenSource InternetCheckTaskToken;

    public Action OnInternetConnectionEstabilished;
    public Action OnInternetConnectionLost;

    public bool isCorrectDateLoaded = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        deviceDateTime = System.DateTime.Now;
        deviceDateTimeString = deviceDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture);


        lastRecordedDeviceDateTimeString = PlayerPrefs.GetString(nameof(lastRecordedDeviceDateTime));
        realLocalDateTimeString = PlayerPrefs.GetString(nameof(realLocalDateTime));

        DateTime.TryParse(realLocalDateTimeString, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out realLocalDateTime);

        if (!DateTime.TryParse(lastRecordedDeviceDateTimeString, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out lastRecordedDeviceDateTime))
        {
            lastRecordedDeviceDateTime = DateTime.Now;
            lastRecordedDeviceDateTimeString = lastRecordedDeviceDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(nameof(lastRecordedDeviceDateTime), lastRecordedDeviceDateTimeString);
        }

        StartCoroutine(InternetStatusChecker());

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneChanged;
    }

    void OnSceneChanged(UnityEngine.SceneManagement.Scene loadedScene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (loadedScene.name.Equals("MainMenu"))
        {
            if (deviceDateTime.Subtract(lastRecordedDeviceDateTime).Days > 0)
            {
                isCorrectDateLoaded = false;
                OnInternetConnectionLost?.Invoke();
                StartCoroutine(InternetStatusChecker());
            }
        }
    }

    IEnumerator DataCheck()
    {
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator InternetStatusChecker()
    {
        for (int i = 0; i < 20; i++)
        {
            try
            {
                TcpClient client = new TcpClient("time.nist.gov", 13);

                using (var streamReader = new StreamReader(client.GetStream()))
                {
                    string response = streamReader.ReadToEnd();
                    string utcDateTimeString = response.Substring(7, 17);
                    realLocalDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    realLocalDateTimeString = realLocalDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    Debug.LogError("Got real date time : " + realLocalDateTimeString);
                    PlayerPrefs.SetString(nameof(realLocalDateTime), realLocalDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    isCorrectDateLoaded = true;
                    OnInternetConnectionEstabilished?.Invoke();
                    break;
                }
            }
            catch (System.Exception e)
            {
                isCorrectDateLoaded = false;
                OnInternetConnectionLost?.Invoke();
                Debug.LogError("Error while getting real date time! Error: " + e);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(nameof(lastRecordedDeviceDateTime), DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            PlayerPrefs.SetString(nameof(lastRecordedDeviceDateTime), DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}