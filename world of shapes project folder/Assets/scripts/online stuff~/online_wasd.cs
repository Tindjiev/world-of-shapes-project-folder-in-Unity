using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class online_wasd : NetworkBehaviour
{
    public Inputs.inputstruct inputup, inputdown, inputleft, inputright, inputsprint;
    public move movars;
    bool gotInputLastFrame = false;
    bool sprintedLastFrame = false;
    public float sprintMultiplier = 1.8f;


    void Start()
    {
        if (movars == null)
        {
            movars = this.getvars<move>();
        }
        inputset();
    }

    Vector3 inputPosition;
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        Cmdgetinputwasd();
        if (inputPosition != movars.position)
        {
            movars.endpos.Clear();
            movars.endpos.Enqueue(inputPosition);
            gotInputLastFrame = true;
        }
        else if (gotInputLastFrame)
        {
            movars.endpos.Clear();
            movars.path.Clear();
            gotInputLastFrame = false;
        }
    }




    [Command]
    void Cmdgetinputwasd()
    {
        if (!movars.freemove)
        {
            inputPosition = movars.position;
            return;
        }
        Vector3 endpos = movars.position;
        //ifsprint();
        if (inputup.checkinput())
        {
            endpos.y += 1f;
        }
        if (inputdown.checkinput())
        {
            endpos.y -= 1f;
        }
        if (inputright.checkinput())
        {
            endpos.x += 1f;
        }
        if (inputleft.checkinput())
        {
            endpos.x -= 1f;
        }
        inputPosition = endpos;
    }



    void ifsprint()
    {
        if (sprintMultiplier != 1f && inputsprint.checkinput())
        {
            movars.setRatioBaseSpeed(sprintMultiplier);
            sprintedLastFrame = true;
        }
        else
        {
            if (sprintedLastFrame)
            {
                movars.setBaseSpeed();
            }
            sprintedLastFrame = false;
        }
    }


    void inputset()
    {
        inputup = new Inputs.inputstruct(Input.GetKey, KeyCode.W);
        inputdown = new Inputs.inputstruct(Input.GetKey, KeyCode.S);
        inputleft = new Inputs.inputstruct(Input.GetKey, KeyCode.A);
        inputright = new Inputs.inputstruct(Input.GetKey, KeyCode.D);
        inputsprint = new Inputs.inputstruct(Input.GetKey, KeyCode.LeftShift);

        inputup.function = inputdown.function = inputleft.function = inputright.function = inputsprint.function = Input.GetKey;
    }







}
