using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifesaver : MonoBehaviour {


    bool saved = false, forsave = true;
    int control;
    LifeComponent lifevars;
    const float whensave = 5f;


    public AudioClip deathmark;
    public AudioClip wololo;

    void Awake()
    {
        lifevars = this.SearchComponent<LifeComponent>();
        if (lifevars == null)
        {
            Destroy(this);
        }
    }



	

	void FixedUpdate ()
    {
        if (saved)
        {
            control--;
            if (control < 0)
            {
                AudioSource.PlayClipAtPoint(deathmark, transform.position);
                saved = false;
                if ((lifevars.Health -= 11.5f) < 1f)
                {
                    lifevars.Health = 1f;
                }
                //vars.damageratio = 1f;
                if (lifevars.Health >= whensave)
                {
                    forsave = true;
                }
            }
        }
        if (!(forsave || saved || lifevars.Health < whensave))
        {
            forsave = true;
        }
        if (transform.parent.gameObject == ControlBase.PlayerGameObject && lifevars.Health < whensave && forsave)
        {
            AudioSource.PlayClipAtPoint(wololo, transform.position, 5f);
            lifevars.Health += 11.5f;
            //vars.damageratio = 1.5f;
            lifevars.Rend.color = new Color(lifevars.Rend.color.r, lifevars.Rend.color.g, 1f);
            saved = true;
            control = 1000;
            forsave = false;
        }
    }


    void LateUpdate()
    {
        if (saved)
        {
            lifevars.Rend.color = new Color(lifevars.Rend.color.r, lifevars.Rend.color.g, control / 1000f + 0.2f);
            if (control < 0)
            {
                lifevars.Rend.color = new Color(lifevars.Rend.color.r, lifevars.Rend.color.g, 0f);
            }
        }
    }
}
