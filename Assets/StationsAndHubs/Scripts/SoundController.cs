using System;
using System.Collections;
using System.Collections.Generic;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioSource _bite_source;
    public AudioSource _music_source;
    [Range(0f,1f)] public float biteVolume = 1f;
    [Range(0f,1f)] public float musicVolume = 1f;
    [Range(0f,1f)] public float alarmVolume = 1f;
    public AudioClip[] bites;
    public AudioClip[] music;
    public AudioClip[] alarms;

    public AudioClip activeAlarm;
    // Start is called before the first frame update
    void Start()
    {
        if(bites!=null && bites.Length==0)
            bites = Resources.LoadAll<AudioClip>("Sound/Effects");
        if(music!=null && music.Length==0)
            music = Resources.LoadAll<AudioClip>("Sound/Music");
        if(alarms!=null && alarms.Length==0)
            alarms = Resources.LoadAll<AudioClip>("Alarms");
        if (_bite_source == null)
            _bite_source = GetComponent<AudioSource>();
        SetAlarm("echo");
    }

    public void PlayBite(string _name)
    {
        foreach (AudioClip c in bites)
        {
            if(c.name.Equals(_name,StringComparison.InvariantCultureIgnoreCase))
                _bite_source.PlayOneShot(c,biteVolume);
        }
    }
    public void PlayBite(string _name,float volMod)
    {
        foreach (AudioClip c in bites)
        {
            if(c.name.Equals(_name,StringComparison.InvariantCultureIgnoreCase))
                _bite_source.PlayOneShot(c,biteVolume*volMod);
        }
    }
    
    public void PlayMusic(string _name)
    {
        foreach (AudioClip c in music)
        {
            if (c.name.Equals(_name, StringComparison.InvariantCultureIgnoreCase))
            {
                _music_source.clip = c;
                _music_source.volume = musicVolume;
                _music_source.Play();
            }
        }
    }

    public void SetAndPlayAlarm(Dropdown dropdown)
    {
        var v = dropdown.options[dropdown.value].text;
        Debug.Log(v);
        SetAlarm(v);
        PlayAlarm();
    }
    
    public void SetAndPlayAlarm(string s)
    {
        SetAlarm(s);
        PlayAlarm();
    }
    
    public void SetAlarm(string s)
    {
        foreach (var a in alarms)
        {
            if (a.name.Equals(s, StringComparison.InvariantCultureIgnoreCase))
            {
                activeAlarm = a;
                break;
                
            }
        }
    }
    public void PlayAlarm()
    {
        if (activeAlarm != null)
        {
            if(_bite_source.isPlaying)
                _bite_source.Stop();
            _bite_source.PlayOneShot(activeAlarm,alarmVolume);
        }
        
    }
    public void MusicStop()
    {
        _music_source.Stop();
    }
    public void SoundStop()
    {
        _bite_source.Stop();
    }
}
