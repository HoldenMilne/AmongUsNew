using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
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
    public GameObject adminDisplayObject;
    public void Start()
    {
        cnm = ((CustomNetworkManager)NetworkManager.singleton);
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
        Debug.Log(cnm.playerNames);
        string[] _sorted = new string[cnm.playerNames.Count];
        cnm.playerNames.CopyTo(_sorted);
        List<string> sorted = new List<string>(_sorted); // Does this work???
        // If not, replace cnm is SortedList
        
        foreach (var p in sorted)
        {
            var p_obj = cnm.players[p].gameObject.GetComponent<PlayerData>();
            if(AmongUsGoSettings.singleton.adminPanelOnlyShowAtStations && p_obj.currentLocation.Equals("Unknown",StringComparison.InvariantCultureIgnoreCase))
                continue;
            var go = GameObject.Instantiate(adminDisplayObject,spawnPoint.transform) as GameObject;
            foreach (var text in go.GetComponentsInChildren<Text>())
            {
                switch (text.tag)
                {
                    case "adminUnitName":
                        text.text = p_obj.playerName;
                        break;
                    case "adminUnitTime":
                        text.text = p_obj.timeOfUpdateLocation+"";
                        break;
                    case "adminUnitLoc":
                        text.text = p_obj.GetCurrentLocation();
                        break;
                }
            }
            var anc = go.GetComponent<RectTransform>().anchoredPosition;
            anc.y = - 60 * i;
            i += 1;
            go.GetComponent<RectTransform>().anchoredPosition = anc;
            var imgComp = go.GetComponentInChildren<Image>();
            var img = imgComp.color;
            img.a = ((i & 1) + 1) * .1f;
            imgComp.color = img;
        }

        var size = scrollView.GetComponent<RectTransform>().sizeDelta;
        size.y = i * 60;
        scrollView.GetComponent<RectTransform>().sizeDelta = size;
        
        
        var anc2 = scrollView.GetComponent<RectTransform>().anchoredPosition;
        anc2.y = 0;
        scrollView.GetComponent<RectTransform>().anchoredPosition = anc2;
    }
    
}
