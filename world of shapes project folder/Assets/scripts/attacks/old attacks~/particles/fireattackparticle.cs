using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireattackparticle : MonoBehaviour
{

    fireattack firevars;

    public Vector3 v;
    float TimeSinceEnable;

    Vector3 lastparentpos;
    Vector3 angletemp;


   // Transform tempParent;

    protected void Awake()
    {
        firevars = GetComponentInParent<fireattack>();
        GetComponent<SpriteRenderer>().sprite = colorlib.GetSpriteColored(this.getvars<BaseCharacterControl>().teamcolor, GetComponent<SpriteRenderer>().sprite, colorlib.colortochange);
        gameObject.SetActive(false);
  //      tempParent = transform.parent;
    }

    protected void OnEnable()
    {
      //  transform.parent = null;
        v = new Vector3(Random.Range(fireattack.originalheight / 0.3f, fireattack.originalheight / 0.6f), Random.Range(-fireattack.originalhalfwidth / 0.6f, fireattack.originalhalfwidth / 0.6f));
        v = mathlib.rotatevector(v, angletemp = mathlib.polarvectdeg(transform.parent.rotation.eulerAngles.z)) + firevars.movars.velocity;
        transform.position = mathlib.rotatevector(Vector3.right, angletemp) + transform.parent.position;
        //transform.position = mathlib.rotatevector(Vector3.right, angletemp);
        //transform.position += lastparentpos = transform.parent.position;
        TimeSinceEnable = Time.time;
        //v = new Vector3(Random.Range(firevars.height / 0.3f, firevars.height / 0.6f), Random.Range(firevars.halfwidth / 0.3f, firevars.halfwidth / 0.6f));
    }
    /*
    void Update()
    {
        transform.parent = null;
        transform.localScale = new Vector3(1f, 1f);
    }

    void FixedUpdate()
    {
        transform.parent = null;
        transform.localScale = new Vector3(1f, 1f);
    }
    */
    void LateUpdate()
    {
        transform.position += v * Time.deltaTime;
        /*if (transform.lossyScale.x != 1f || transform.lossyScale.y != 1f)
        {
            Transform tempParent = transform.parent;
            transform.parent = null;
            transform.localScale = new Vector3(1f, 1f);
            transform.parent = tempParent;
        }*/
        //fixposition(transform.parent.position, ref lastparentpos);
        if (Time.time - TimeSinceEnable > 2f || firevars.checkborders((mathlib.rotatevector(transform.position - transform.parent.position, mathlib.Conjugatevect(angletemp))) / 1.35f))
        {
            gameObject.SetActive(false);
        }
        transform.localScale = new Vector3(1f, 1f);
       // transform.parent = tempParent;
    }

    void fixposition(Vector3 currparpos,ref Vector3 lastparpos)
    {
        transform.position -= currparpos - lastparpos;
        lastparentpos = currparpos;
    }



}
