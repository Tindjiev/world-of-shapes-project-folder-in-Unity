using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FortressEnd : MonoBehaviour
{

    [SerializeField]
    private Image _blackImage;

    [SerializeField]
    private BaseCharacterControl _endCharacter;

    [SerializeField]
    private BaseCharacterControl[] _charactersToMove;

    [SerializeField]
    private Room _startingRoom, _nextRoom;

    private PlayerBaseControl _player;

    private void Start()
    {

    }


    public InputStruct InputForCheat;
    private bool _inputGiven = false;

    private void Update()
    {
        if (_inputGiven) return;
        if (InputForCheat.CheckInput())
        {
            _inputGiven = true;
            var player = FindObjectOfType<PlayerControlBattle>();
            player.SearchComponent<MachineGun>().DamagePerSecond = 5000f;
            player.MoveComponent.BaseSpeed = 20f;
            player.MoveComponent.SetBaseSpeed();
            Debug.Log("Cheat activated");
        }
    }

    public void End()
    {
        _player = FindObjectOfType<PlayerBaseControl>();
        MyInputs.InputsOff();
        _player.MoveComponent.StartFromScratchNewEndpos(_startingRoom.Position);

        if (_blackImage.color.a != 0f)
        {
            var temp = _blackImage.color;
            temp.a = 0f;
            _blackImage.color = temp;
        }
        _blackImage.gameObject.SetActive(true);
        _blackImage.DOFade(1.1f, 1.1f).onComplete = MoveToStart;
    }

    private void MoveToStart()
    {
        _blackImage.DOFade(0f, 1f).onComplete = _blackImage.gameObject.SetActiveFalse;

        foreach (var character in _charactersToMove)
        {
            character.MoveComponent.ClearPath();
            character.MoveComponent.SetPosition(_startingRoom.Position + new Vector3(Random.value, Random.value));
        }


        _player.MoveComponent.ClearPath();
        _player.MoveComponent.SetPosition(_nextRoom.Position);
        var tempPosition = 0.25f * _startingRoom.Position + 0.75f * _nextRoom.Position;
        _player.MoveComponent.StartFromScratchNewEndpos(tempPosition);

        _endCharacter.gameObject.SetActive(true);
        _endCharacter.MoveComponent.SetPosition(tempPosition - new Vector3(0f, 7f));

    }

}
