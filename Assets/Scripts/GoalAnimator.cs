using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAnimator : MonoBehaviour
{
    public enum MiniGame
    {
        ChartCourse,
    }

    public MiniGame miniGame = MiniGame.ChartCourse;

    [HideInInspector] public bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        switch (miniGame)
        {
            case MiniGame.ChartCourse:
                StartCoroutine(ChartCourse());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    IEnumerator ChartCourse()
    {
        var rot = transform.eulerAngles;
        while (!done)
        {
            rot.z += 450 * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rot);
            yield return null;
        }
    }
}
