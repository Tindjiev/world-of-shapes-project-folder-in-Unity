using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Home : HubRoom, ISavable
{
    BaseCharacterControl squire;

    protected new void Awake()
    {
        base.Awake();
        squire = AddInEditor.MobCreate.SimpleMob(hub.ProportionalPosition(0.1f, -0.5f), playervars.Team.transform, 10f).GetCharacter();
        squire.MoveComponent.enabled = false;


        SetUpSquireGreeting();


        _textStyle = new GUIStyle();
        _textStyle.normal.textColor = Color.black;
        _textStyle.alignment = TextAnchor.MiddleCenter;




    }

    protected new void Start()
    {
        base.Start();
    }


    protected new void Update()
    {
        base.Update();
    }

    void OnGUI()
    {
        _textStyle.fontSize = Screen.width * 2 / 100;
        GUI.Label(new Rect(Screen.width >> 2, Screen.height * 3 / 4, 0f, 0f), string.Format("WASD to move"), _textStyle);
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
    }



    private void SetUpSquireGreeting()
    {
        EventObjectDialogueEnd trigdiag = hub.AddEventComponent<EventObjectDialogueEnd>();
        trigdiag.Dialouge = DialogueStaticClass.SetDialogue(squire, new DialogueStruct[] {
                                        new DialogueStruct("Welcome back sir",DialogueStruct.Mood.BriefResponse),
                                        },
                                        true, true, DialogueComponentBase.DialogueStart.ByEnteringRange, DialogueComponentBase.DialogueRepetition.DeleteAfterOnce, 8f);
        trigdiag.AddAction(() => squire.MoveComponent.enabled = true);
        trigdiag.AddAction(InputsOn);
        trigdiag.AddAction(SetUpOptions);
    }


    private void SetUpOptions()
    {

        DialogueStaticClass.setDialogue(squire,
            new DialogueStruct[][] {
            new DialogueStruct[] {
                                        new DialogueStruct("What would you like to do sir?",DialogueStruct.Mood.Question),
                                        new DialogueStruct("Uhh... what's there to do?",DialogueStruct.Mood.Question,player),
                                        //new DialogueStruct("Well... Firstly, you can enter the training room",DialogueStruct.Mood.NeutralMedium),
                                        new DialogueStruct("Well... only thing to do is go to the training room",DialogueStruct.Mood.NeutralMedium),
                                        new DialogueStruct("In which, you can test your abilities and weapons",DialogueStruct.Mood.NeutralLong),
                                        new DialogueStruct("and also fight off charger bots",DialogueStruct.Mood.NeutralShort),
                                        //new DialogueStruct("Secondly, you could go to the Arena and slay some blue automatons we've captured",DialogueStruct.Mood.NeutralLong),
                                        new DialogueStruct("That's all for now",DialogueStruct.Mood.NeutralMedium),
                                        },
            new DialogueStruct[] {
                                        new DialogueStruct("Let's go to the training room",DialogueStruct.Mood.BriefResponse,player),
                                        new DialogueStruct("As you wish...",DialogueStruct.Mood.Afformative,squire.GetCharacter(), SwtcihToTriangles)
                                        },
            new DialogueStruct[] {
                                        new DialogueStruct("Nevermind",DialogueStruct.Mood.BriefResponse,player),
                                        new DialogueStruct("Come back anytime sir!",DialogueStruct.Mood.NeutralShort,squire)
                                        }

            },
            new int[,] { { -1, -1, 0, 0 }, { 0, 0, -1, -1 }, { 0, 0, -1, -1 } }, false, true, DialogueComponentBase.DialogueStart.ByInteraction, DialogueComponentBase.DialogueRepetition.RepeatButNotAutomatically);
    }

    private void InputsOn() => MyInputs.InputsOn();
    private void SwtcihToTriangles()
    {
        SceneChanger.SetSceneToSwitchTo(SceneChanger.SwtichToHome);
        SceneChanger.SwitchToTrianglesPit();
    }

    public string GetStringPath()
    {
        return io.buildStringPath("Home");
    }

    public void Save()
    {
        io.Save(GetStringPath(), homedata);
    }

    public void Load()
    {
        homedata = io.Load<HomeData>(GetStringPath());
        if (homedata == null)
        {
            homedata = new HomeData(false);
        }
    }

    HomeData homedata;

    [System.Serializable]
    class HomeData : DataClassBase
    {
        public bool SquireIntroduced;

        public HomeData(bool SquireIntroduced)
        {
            this.SquireIntroduced = SquireIntroduced;
        }


    }
}

