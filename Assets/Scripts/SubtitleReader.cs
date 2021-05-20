using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;

public class SubtitleReader : NetworkBehaviour
{
    public List<SubtitleObject> subtitles = new List<SubtitleObject>();

    [SyncVar(hook = nameof(OnSubChanged))]
    public string currentSub;
    [SyncVar(hook = nameof(OnNextTextChanged))]
    public string nextSub;
    public char divChar;

    string subFile;
    GameObject scrollContent;
    Button applyButton;
    Button nextButton;
    Button clearButton;
    Text currentText;
    Text nextText;
    GameObject greenPanel;
    public GameObject uiPrefab;

    public SubtitleObject currentSubtitleObject;
    public SubtitleObject selectedSubtitle;
    public SubtitleObject nextSubtitle;

    [SerializeField]
    List<string> talkers = new List<string>();
    [SerializeField]
    List<string> engSubs = new List<string>();
    [SerializeField]
    List<string> cnSubs = new List<string>();


    private void Start()
    {
        scrollContent = ((SubNetworkManager)NetworkManager.singleton).subScrollContent;
        nextButton = ((SubNetworkManager)NetworkManager.singleton).nextButton;
        applyButton = ((SubNetworkManager)NetworkManager.singleton).applyButton;
        clearButton = ((SubNetworkManager)NetworkManager.singleton).clearButton;
        currentText = ((SubNetworkManager)NetworkManager.singleton).currentText;
        nextText = ((SubNetworkManager)NetworkManager.singleton).nextText;
        greenPanel = ((SubNetworkManager)NetworkManager.singleton).greenPanel;

        if (isServer)
        {
            subFile = FileManager.LoadTexts("/Sub/", "subtitle");

            List<List<string>> csvSubtitle = FileManager.LoadCSV("/Sub/", "csvSubtitle");
            foreach (List<string> list in csvSubtitle)
            {
                talkers.Add(list[0]);
                engSubs.Add(list[1]);
                cnSubs.Add(list[2]);
            }

            BuildUI();

            ChangeSub(subtitles[0]);
        }
        else
        {
            nextButton.interactable = false;
            applyButton.interactable = false;
            clearButton.interactable = false;
        }
    }

    public void OnSubChanged(string _, string newSub)
    {
        FileManager.SaveTexts(currentSub, "/Sub/", "current");
        currentText.text = currentSub;
        greenPanel.GetComponentInChildren<Text>().text = currentSub;
    }

    public void OnNextTextChanged(string _, string newNext)
    {
        nextText.text = nextSub;
    }

    public void ChangeSub(SubtitleObject so)
    {
        if (currentSubtitleObject != null)
            currentSubtitleObject.isPlaying = false;

        currentSubtitleObject = so;
        currentSub = currentSubtitleObject.sub;
        currentSubtitleObject.isPlaying = true;

        int temp = (subtitles.IndexOf(currentSubtitleObject) == subtitles.Count - 1 ?
                    0 : subtitles.IndexOf(currentSubtitleObject) + 1);

        ChangeNext(subtitles[temp]);
    }

    public void ChangeNext(SubtitleObject so)
    {
        if (nextSubtitle != null)
            nextSubtitle.isNext = false;
        nextSubtitle = so;
        nextSub = nextSubtitle.sub;
        nextSubtitle.isSelected = false;
        nextSubtitle.isNext = true;
    }

    public void ClearSub()
    {
        if (currentSubtitleObject != null)
            currentSubtitleObject.isPlaying = false;

        currentSubtitleObject = null;
        currentSub = "";
    }

    public void BuildUI()
    {
        // Bind Button
        nextButton.onClick.AddListener(() => { ChangeSub(nextSubtitle); });
        applyButton.onClick.AddListener(() => { ChangeNext(selectedSubtitle); });
        clearButton.onClick.AddListener(() => { ClearSub(); });

        string[] subs = subFile.Split(divChar);
        RectTransform rect = scrollContent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(
            rect.rect.width,
            uiPrefab.GetComponent<RectTransform>().rect.height * subs.Length);

        foreach (var s in subs)
        {
            SubtitleObject so = Instantiate(uiPrefab, rect).GetComponent<SubtitleObject>();
            so.Init(s, this);
            subtitles.Add(so);
        }
    }
}