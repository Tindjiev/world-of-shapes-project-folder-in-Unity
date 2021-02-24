using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : ControlBase
{

    [SerializeField]
    private int _numberOfTeams = 4, _eachTeamNumber = 7;
    [SerializeField]
    private Vector3[] _centreSpawns;
    private Transform[][] _characters;

    protected new void Start()
    {
        _characters = new Transform[_numberOfTeams][];
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i] = new Transform[_eachTeamNumber];
        }
        _centreSpawns = new Vector3[_characters.Length];
        for (int i = 0; i < _centreSpawns.Length; i++)
        {
            _centreSpawns[i] = MyMathlib.PolarVectorDeg(50f * MyMathlib.SQRT_OF_2, 50f * MyMathlib.SQRT_OF_2, 135f - i * 360 / _centreSpawns.Length);
        }

    }

    protected new void Update()
    {
        CheckToSpawn();
    }


    private void CheckToSpawn()
    {
        for (int i = 0; i < _characters.Length; i++)
        {
            for (int j = 0; j < _characters[i].Length; j++)
            {
                if (_characters[i][j] == null)
                {
                    GameObject temp = SpawnNpc(_characters[i]);
                    _characters[i][j] = temp.SearchComponentTransform<MoveComponent>();
                }
            }
        }
    }
    



    private GameObject SpawnNpc(Transform[] fightersgroup)
    {
        GameObject temp = null;
        int teamnum = 0;
        for (int i = 0; i < _characters.Length; i++)
        {
            if (_characters[i] == fightersgroup)
            {
                temp = BasicLib.InstantiatePrefabGmbjct("mobs/npc", MyColorLib.teams[i].transform);
                teamnum = i;
                break;
            }
        }
        LifeComponent life = temp.SearchComponent<LifeComponent>();
        AI aivars = temp.SearchComponent<AI>();
        BaseCharacterControl vars = temp.GetCharacter();
        MoveComponent MoveComponent = temp.SearchComponent<MoveComponent>();
        //MoveComponent.speed = 10f;

        MoveComponent.SetPosition(_centreSpawns[teamnum] + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
        aivars.SearchComponent<MoveAround_ChillMode>().CentrePosition = Vector3.zero;
        /*
        foreach (Transform[] otherteam in fighters)
        {
            if (otherteam != fightersgroup)
            {
                rules.addToCandoAndToOthers(otherteam, MoveComponent.transform, true, true);
            }
        }
        */

        int rand = Random.Range(0, 5);
        if (rand == 0)
        {
            aivars.AddAttacks(new int[] { WeaponsStaticClass.fire });
            temp.AddComponent<AttackFromDistance_AttackMode>();
        }
        else if (rand == 1)
        {
            aivars.AddAttacks(new int[] { WeaponsStaticClass.ball });
            temp.AddComponent<BallCharge_AttackMode>();
        }
        else if (rand == 2)
        {
            aivars.AddAttacks(new int[] { WeaponsStaticClass.spray });
            temp.AddComponent<AttackFromDistance_AttackMode>();
        }
        else if (rand == 3)
        {
            aivars.AddAttacks(new int[] { WeaponsStaticClass.ball });
            BallChargeAttack ball = temp.SearchComponent<BallChargeAttack>();
            ball.Reach = 30f;
            ball.MaxSize = 1f;
            ball.Speed = 60f;
            temp.AddComponent<BallCharge_AttackMode>();
        }
        else if (rand == 4)
        {
            aivars.AddAttacks(WeaponsStaticClass.shield).SearchComponent<Shield>().SetUpAI();
        }
        return temp;
    }

/*
    protected void OnDestroy()
    {
        foreach (Transform[] fighter in fighters)
        {
            foreach (Transform tr in fighter)
            {
                if (tr != null)
                {
                    Destroy(tr.getvars<BaseCharacterControl>().gameObject);
                }
            }
        }
    }
    */

}
