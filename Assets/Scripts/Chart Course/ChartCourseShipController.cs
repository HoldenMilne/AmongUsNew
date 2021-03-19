using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartCourseShipController : MonoBehaviour
{
    public Transform ship;
    public Transform[] points = new Transform[4];

    private int checkpoint = 0;

    public bool testing = false;

    private bool complete = false;
    private Vector2 nul = new Vector2(-1,-1);

    private GameController gameController;
    
    // Start is called before the first frame update
    void Start()
    {
        ship = transform;
        gameController = FindObjectOfType<GameController>();
        if(gameController == null)
            gameController = GameController.CreateGC();
        

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 touch;
        if (!complete)
        {
            touch = gameController.GetTouch();
            Debug.Log(touch);
            if (touch != nul)
            {

                FaceShipToNext();
                UpdatePosition(touch);
                complete=CheckCheckpoint();
                if (complete)
                    StartCoroutine(Success());
            }
        }


    }

    IEnumerator Success()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("MADE IT");
        var gc = FindObjectOfType<GameController>();

        gc.reportCanvas.enabled = true;
    }

    private bool CheckCheckpoint()
    {
        if (Vector2.Distance(points[checkpoint + 1].position, ship.position) < .2f)
        {
            ship.position = points[checkpoint + 1].position;
            checkpoint += 1;
        }

        if (checkpoint == 3)
            return true;
        return false;

    }

    private void UpdatePosition(Vector2 touch)
    {
        
        var t = (Vector3) touch;
        
        Vector3 line = (points[checkpoint + 1].position - points[checkpoint].position).normalized;
        
        line.z = t.z;
        var dot = Vector3.Dot(line,touch-(Vector2)points[checkpoint].position);
        
        // get the new point by taking the magnitude (dot) in the line direction.
        var newPoint = line * dot + points[checkpoint].position;
        if (newPoint.y< points[checkpoint].position.y)
        {
            ship.position = points[checkpoint].position;
            return;
        }
        Debug.DrawLine(points[checkpoint].position,newPoint,Color.blue);
        if (Vector2.Distance(touch, newPoint) < .2f && Vector2.Distance(ship.position,newPoint) < .5f)
        {
            ship.position = newPoint;
        }
    }

    private void FaceShipToNext()
    {
        var headingChange = Quaternion.FromToRotation(Vector3.up, (points[checkpoint+1].position-points[checkpoint].position).normalized)
            .eulerAngles.z;

        var rot = ship.eulerAngles;
        rot.z = headingChange;
        ship.eulerAngles = rot;
    }

    private Vector2 GetTouchPosition()
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            var p = touch.position;
            p.x -= Screen.width/2f;
            p.y -= Screen.height/2f;
            Debug.Log(p);
            p = Camera.main.ScreenToWorldPoint(p);
            Debug.Log(p);
            return p;

        }

        return nul;
    }
    
    private Vector2 GetMousePosition()
    {
        
        if (Input.GetMouseButton(0))
        {
            Vector2 p = Input.mousePosition;
            //p.x -= Screen.width/2f;
            //p.y -= Screen.height/2f;
            p = Camera.main.ScreenToWorldPoint(p);
            Debug.Log(p);
            return p;

        }

        return nul;
    }
}
