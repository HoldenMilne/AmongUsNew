using System;
using System.Collections;
using System.Collections.Generic;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AdminPanel : MonoBehaviour
{
    public int updateFrequencyMillis = 500;
    private CustomNetworkManager cnm;
    public Transform spawnPoint;
    public GameObject scrollView;
    public void Start()
    {
        cnm = GetComponent<CustomNetworkManager>();
        StartCoroutine(UpdatePanel());
    }

    IEnumerator UpdatePanel()
    {
        while (spawnPoint != null)
        {
            ReloadPanel();
            yield return new WaitForSecondsRealtime(updateFrequencyMillis / 1000f);
        }
    }

    private void ReloadPanel()
    {
        
        foreach (var g in spawnPoint.GetComponentsInChildren<Transform>())
        {
            if(g!=spawnPoint)
                Destroy(g.gameObject);
        }

        
        int i = 0;
        string[] _sorted = new string[cnm.playerNames.Count];
        cnm.playerNames.CopyTo(_sorted);
        List<string> sorted = new List<string>(_sorted); // Does this work???
        // If not, replace cnm is SortedList
        
        foreach (var p in sorted)
        {
            var go = GameObject.Instantiate(cnm.playerDisplayObject,spawnPoint.transform) as GameObject;
            var p_obj = cnm.players[p].gameObject.GetComponent<PlayerData>();
            go.GetComponentInChildren<Text>().text=p+ " : " + p_obj.GetCurrentLocation();
            var anc = go.GetComponent<RectTransform>().anchoredPosition;
            anc.y = - 60 * i;
            i += 1;
            go.GetComponent<RectTransform>().anchoredPosition = anc;
            
            var img = go.GetComponent<Image>().color;
            img.a = ((i & 1) + 1) * .1f;
            go.GetComponent<Image>().color = img;
        }

        var size = scrollView.GetComponent<RectTransform>().sizeDelta;
        size.y = i * 60;
        scrollView.GetComponent<RectTransform>().sizeDelta = size;
        
        
        var anc2 = scrollView.GetComponent<RectTransform>().anchoredPosition;
        anc2.y = 0;
        scrollView.GetComponent<RectTransform>().anchoredPosition = anc2;
    }
    
}
