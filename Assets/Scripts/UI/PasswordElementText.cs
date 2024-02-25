using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordElementText : MonoBehaviour {

    [Header("References")]
    private TMP_Text infoText;

    [Header("Color")]
    [SerializeField] private Color correctColor; // correct element & position
    [SerializeField] private Color partialCorrectColor; // correct element, wrong position
    [SerializeField][Range(0, 1)] private float hoverDarkenAmount;
    [SerializeField] private float hoverFadeDuration;
    private Color startColor;

    public void Initialize(string text) {

        this.infoText = GetComponent<TMP_Text>();
        this.infoText.text = text;

        EventTrigger eventTriggers = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((data) => OnPointerEnter((PointerEventData) data));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((data) => OnPointerExit((PointerEventData) data));

        eventTriggers.triggers.Add(entry1);
        eventTriggers.triggers.Add(entry2);

        startColor = this.infoText.color;

    }

    public void SetCorrectColor() {

        infoText.DOColor(correctColor, hoverFadeDuration);
        infoText.fontStyle = FontStyles.Bold;

    }

    public void SetPartialCorrectColor() {

        infoText.DOColor(partialCorrectColor, hoverFadeDuration);
        infoText.fontStyle = FontStyles.Bold;

    }

    public void ResetColor() {

        infoText.DOColor(startColor, hoverFadeDuration);
        infoText.fontStyle = FontStyles.Normal;

    }

    private void OnPointerEnter(PointerEventData eventData) {

        infoText.DOColor(new Color(infoText.color.r - hoverDarkenAmount, infoText.color.g - hoverDarkenAmount, infoText.color.b - hoverDarkenAmount), hoverFadeDuration);

    }

    private void OnPointerExit(PointerEventData eventData) {

        infoText.DOColor(startColor, hoverFadeDuration);

    }
}
