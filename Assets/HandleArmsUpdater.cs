using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleArmsUpdater : MonoBehaviour
{
    private RectTransform arms;
    public RectTransform handleBase;

    public RectTransform handle;

    private float initDif;

    private float initSize;
    // Start is called before the first frame update
    void Start()
    {
        arms = GetComponent<RectTransform>();
        var by = handleBase.anchoredPosition.y;
        var hy = handle.anchoredPosition.y;
        var anc = handle.anchoredPosition;
        anc.y = by+arms.sizeDelta.y/2f;
        arms.anchoredPosition = anc;
        initDif = hy - by;
        initSize = arms.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        var by = handleBase.anchoredPosition.y;
        var hy = handle.anchoredPosition.y;
        var dif = hy - by;
        
        // set rotation correctly
        var pos = arms.anchoredPosition;
        pos.y = dif / 2f;
        arms.anchoredPosition = pos;
        var e = arms.eulerAngles;
        if (dif < 0)
        {
            e.z = 180;
            dif = -1 * dif; // abs

        }
        else
        {
            e.z = 0;
        }

        arms.eulerAngles = e;
        var s = initSize * (dif / initDif);
        var size = arms.sizeDelta;
        size.y = s;
        arms.sizeDelta = size;
    }
}
