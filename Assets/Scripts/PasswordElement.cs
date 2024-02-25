using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordElement {

    [Header("Data")]
    private string information;
    private PasswordElementText text;

    public PasswordElement(string information, PasswordElementText text) {

        this.information = information;
        this.text = text;

    }

    public string GetInformation() { return information; }

    public PasswordElementText GetText() { return text; }

}
