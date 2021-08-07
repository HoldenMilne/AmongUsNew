using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StationGameController : GameController
{
    protected override void ReturnToMenu()
    {
        SceneManager.LoadScene("PCIntroScene");
    }
}
