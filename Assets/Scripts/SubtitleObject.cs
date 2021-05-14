using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubtitleObject : MonoBehaviour
{
    public string sub;
    Text text;
    public Image cover;
    public Color hoverColor;
    public Color playingColor;
    public Color nextColor;
    public Color selectColor;

    public bool isHovering = false;
    public bool isPlaying = false;
    public bool isSelected = false;
    public bool isNext = false;

    public void Init(string s, SubtitleReader sr)
    {
        sub = s;
        this.text = GetComponentInChildren<Text>();
        text.text = sub;

        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener(
            delegate
            {
                isHovering = true;
            });
        trigger.triggers.Add(hoverEntry);

        EventTrigger.Entry leaveEntry = new EventTrigger.Entry();
        leaveEntry.eventID = EventTriggerType.PointerExit;
        leaveEntry.callback.AddListener(
            delegate
            {
                isHovering = false;
            });
        trigger.triggers.Add(leaveEntry);

        EventTrigger.Entry selectEntry = new EventTrigger.Entry();
        selectEntry.eventID = EventTriggerType.PointerDown;
        selectEntry.callback.AddListener(
            delegate
            {
                if (sr.selectedSubtitle != null)
                    sr.selectedSubtitle.isSelected = false;
                sr.selectedSubtitle = this;
                isSelected = true;
            });
        trigger.triggers.Add(selectEntry);

        ScrollRect scroll = gameObject.GetComponentInParent<ScrollRect>();

        EventTrigger.Entry dragEnd = new EventTrigger.Entry();
        dragEnd.eventID = EventTriggerType.EndDrag;
        dragEnd.callback.AddListener((data) => { scroll.OnEndDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEnd);

        EventTrigger.Entry dragBegin = new EventTrigger.Entry();
        dragBegin.eventID = EventTriggerType.BeginDrag;
        dragBegin.callback.AddListener((data) => { scroll.OnBeginDrag((PointerEventData)data); });
        trigger.triggers.Add(dragBegin);

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { scroll.OnDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEntry);
    }

    private void Update()
    {
        if (isPlaying)
            cover.color = playingColor;
        else if (isNext)
            cover.color = nextColor;
        else if (isSelected)
            cover.color = selectColor;
        else if (isHovering)
            cover.color = hoverColor;

        if (isPlaying || isNext || isSelected || isHovering)
            cover.enabled = true;
        else
            cover.enabled = false;
    }
}
