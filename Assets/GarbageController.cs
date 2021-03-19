using UnityEngine;
using UnityEngine.UI;

public class GarbageController : MonoBehaviour
{

    public Sprite[] leafs = new Sprite[7];
    public Sprite[] garbage = new Sprite[6];

    public Sprite teleporter;
    public Sprite totem;
    public Sprite diamond;

    private static bool leafOnly = false;

    private static bool leafSet = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!leafSet)
        {
            leafOnly = Random.Range(0, 2) == 0;
            leafSet = true;
        }
        GetComponent<SpriteRenderer>().sprite = GetSprite();
        var e = gameObject.transform.eulerAngles;
        e.z = Random.Range(0, 360);
        gameObject.transform.eulerAngles = e;
        gameObject.AddComponent<PolygonCollider2D>();
    }

    private Sprite GetSprite()
    {
        var v = Random.Range(0, 100);
        if (v < (leafOnly?86:52)) // leaf
        {
            var r = Random.Range(0,leafs.Length);
            return leafs[r];
        }

        if (v < 86)//trash
        {
            
            var r = Random.Range(0,garbage.Length);
            return garbage[r];
        }
        
        if (v < 91)
        {
            return teleporter;
        }
        
        return v < 96 ? totem : diamond;
    }


    public static void CallOnAll(float f=1f)
    {
        var objs = FindObjectsOfType<GarbageController>();
        foreach (var v in objs)
        {
             v.UpForce(f);   
        }
    }

    public void UpForce(float f=1f)
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up*f,ForceMode2D.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        if (Camera.main.WorldToScreenPoint(transform.position).y < 0)
        {
            Destroy(gameObject);
        }
    }
}
