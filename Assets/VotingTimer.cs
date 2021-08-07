using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class VotingTimer : MonoBehaviour
{
    public Text text;

    private SoundController sc;
    // Start is called before the first frame update
    void Start()
    {
        sc = FindObjectOfType<SoundController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int time;
    public void StartTimer()
    {
        time = AmongUsGoSettings.singleton.votingTime;
        if(sc==null)
            sc = FindObjectOfType<SoundController>();
        
        StartCoroutine(Timer());
    }

    public IEnumerator Timer()
    {
        time = 10;
        
        text.text = "Time Remaining:\n"+FormatTime(time);
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time -= 1;
            text.text = "Time Remaining:\n"+FormatTime(time);
        }
        yield return new WaitForSeconds(1);
        text.text = "Time Remaining:\n"+FormatTime(time);
        sc._bite_source.PlayOneShot(sc.activeAlarm);
    }

    private string FormatTime(int i)
    {
        int mins = (time / 60);
        i -= mins * 60;
        var sMins = mins+"";
        var sSecs = i+"";

        if (sMins.Length == 1) sMins = "0" + sMins;
        if (sSecs.Length == 1) sSecs = "0" + sSecs;
        return sMins+"m:"+sSecs+"s";

    }
}
