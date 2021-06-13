using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerDataHandler : MonoBehaviour
{
    public string PlayerName { get; set; }

    public bool SetPlayerNameFromInput(InputField i)
    {
        var t = i.text;
        if (t != "")
        {
            // TRY assigning the name, BUT disconnect if name is taken
            PlayerName = t;
            return true;
        }

        return false;
    }
}
