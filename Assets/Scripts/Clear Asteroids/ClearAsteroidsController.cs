using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClearAsteroidsController : MonoBehaviour
{

    public GameObject[] asteroids;
    public RectTransform cursor;

    public RectTransform _base;
    public Text text;

    public GameObject b_left; // have line renderers
    public GameObject b_right;

    private GameController gc;

    private LineRenderer lrl, lrr;

    public int taskCompleteAt = 30;
    private int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>();
        if (gc == null)
        {
            gc = GameController.CreateGC();
        }
        
        lrl = b_left.GetComponent<LineRenderer>();
        lrr = b_right.GetComponent<LineRenderer>();

        var p1 = b_left.GetComponent<RectTransform>().position;
        var p2 = b_right.GetComponent<RectTransform>().position;
        p1.z = p2.z = 0;
        lrl.SetPosition(0,p1);
        lrr.SetPosition(0,p2);
        lrl.SetPosition(1,cursor.position);
        lrr.SetPosition(1,cursor.position);

        d = _base.sizeDelta.x * (1 - 450f / (506f))/2f;
        Debug.Log(d+"<<");
    }

    private float timer = 1;

    void Update()
    {
        if (done) return;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            CreateAsteroid(RandomAsteroid());
            timer = (float)(new System.Random().NextDouble()*.85f+.15f);
        }
    }

    private float d;

    private bool done = false;
    public void OnClick()
    {
        if(!done)
            OnClick(null);
    }

    public void OnClick(GameObject b)
    {
        Debug.Log("CLEEK "+b);
        
        var touch = gc.GetTouch();
        if (Vector2.Distance(touch, _base.position) < d)
        {
            cursor.position = touch;
            lrl.SetPosition(1, touch);
            lrr.SetPosition(1, touch);
        }   
        if(b!=null)
        {
            StartCoroutine(DestroyAsteroid(b.GetComponent<AsteroidController>()));
            counter += 1;
            text.text = "Destroyed: " + counter;
            if (counter >= taskCompleteAt)
            {
                StartCoroutine(Success());
            }
            
            
        }
    }

    IEnumerator Success()
    {
        done = true;
        yield return new WaitForSeconds(1f);

        gc.reportCanvas.enabled = true;
    }
    IEnumerator DestroyAsteroid(AsteroidController ast)
    {
        ast.explosion.enabled=true;
        yield return new WaitForSecondsRealtime(.2f);
        
        ast.brokenSprite.enabled = true;
        ast.GetComponent<Image>().enabled = false;
        ast.explosion.enabled=false;
        yield return new WaitForSecondsRealtime(.2f);
        Destroy(ast.gameObject,0);
    }
    
    public RectTransform[] creationPoints = new RectTransform[3];

    public RectTransform p1;
    public RectTransform p2;
    public void CreateAsteroid(GameObject asteroid)
    {
        var r = new System.Random().Next(0, creationPoints.Length);

        var ac = Instantiate(asteroid,creationPoints[r]).GetComponent<AsteroidController>();

        var x = p1.position.x;
        var y = (float)(new System.Random().NextDouble()*(p1.position.y-p2.position.y))+p2.position.y;
        
        ac.target = new Vector2(x,y);

    }
    
    
    private GameObject RandomAsteroid()
    {
        var r = new System.Random().Next(0,asteroids.Length);
        return asteroids[r];
    }

}
