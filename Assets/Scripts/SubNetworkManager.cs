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
        if (numPlayers == 2)
        {
            NetworkServer.Spawn(Instantiate(subReader));
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

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        SceneManager.LoadScene(0);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        SceneManager.LoadScene(0);
    }
}