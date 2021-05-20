using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubNetworkManager : NetworkManager
{
    public GameObject subScrollContent;
    public GameObject subReader;
    public Button nextButton;
    public Button applyButton;
    public Button clearButton;

    [Header("UI們")]
    public Text currentText;
    public Text nextText;

    public GameObject greenPanel;

    GameObject subtitleReader;

    [SerializeField]
    bool autoHost = false;
    public bool AutoHost
    {
        set { autoHost = value; }
    }
    
    [SerializeField]
    bool autoReconnect = false;

    public bool AutoReconnect
    {
        set { autoReconnect = value; }
    }

    public override void Start()
    {
        base.Start();
        if (FileManager.LoadTexts("/Config/", "address") != null)
            networkAddress = FileManager.LoadTexts("/Config/", "address");
        else
        {
            FileManager.SaveTexts(networkAddress, "/Config/", "address");
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        // if (numPlayers == 2)
        // {
        //     NetworkServer.Spawn(Instantiate(subReader));
        // }
        if (subtitleReader != null)
        {
            NetworkServer.Spawn(subtitleReader);
        }
        else
        {
            subtitleReader = Instantiate(subReader);
            NetworkServer.Spawn(subtitleReader);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        FileManager.SaveTexts(networkAddress, "/Config/", "address");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        SceneManager.LoadScene(0);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!autoReconnect)
            SceneManager.LoadScene(0);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }

    private void Update()
    {
        if (autoReconnect && !isNetworkActive)
            StartClient();

#if UNITY_ANDROID
#else
        if (Input.GetKeyDown(KeyCode.F))
        {
            greenPanel.SetActive(!greenPanel.activeInHierarchy);
        }
#endif
    }
}