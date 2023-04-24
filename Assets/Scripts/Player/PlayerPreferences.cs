using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreferences : MonoBehaviour
{
    public String playerName;
    public String playerColour;

    private void Start()
    {
        playerName = GetPlayerName();
        playerColour = GetPlayerColour();    
    }

    public virtual string GetPlayerName()
    {
        return PlayerPrefs.GetString("LocalName", "");
    }

    public virtual void SetPlayerName(String newName)
    {
        PlayerPrefs.SetString("LocalName", newName);
    }

    public virtual string GetPlayerColour()
    {
        return PlayerPrefs.GetString("LocalColour", "FF0000");
    }

    public virtual void SetPlayerColour(String colourHex)
    {
        PlayerPrefs.SetString("LocalColour", colourHex);
    }
}
