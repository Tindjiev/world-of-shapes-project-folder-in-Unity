using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class createTest : NetworkBehaviour
{

    protected void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Cmdspawnthing();
    }


    void Update()
    {
        
    }

    //GameObject myplayerunit;

    [Command]
    void Cmdspawnthing()
    {
        GameObject testsubject = somefunctions.InstantiatePrefabGmbjct("online/fornetid", null);
        move testmovars = testsubject.transform.GetChild(0).getvars<move>();
        gameObject.AddComponent<online_wasd>().movars = testmovars;
        testmovars.BaseSpeed = 10f;
        testmovars.setBaseSpeed();
        testmovars.SetPosition(10f * new Vector3(Random.value, Random.value, 0));


        NetworkServer.Spawn(testsubject);
    }
}
