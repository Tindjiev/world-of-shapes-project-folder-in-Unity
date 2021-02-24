using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionInterface : MonoBehaviour
{

    private BaseCharacterControl _character;
    private SpriteRenderer _rend;

    protected void Start()
    {
        _character = this.GetCharacter();
        _rend = GetComponent<SpriteRenderer>();
    }


    private void LateUpdate()
    {
        if (CameraScript.showPlayerInterface)
        {
            transform.rotation = Quaternion.Euler(0f, 0f,
                                (transform.parent != null ? _character.TargetPosition - transform.parent.position : _character.DirectionVector).AnlgeDegrees());
            _rend.enabled = true;
        }
        else
        {
            _rend.enabled = false;
        }
    }

    public void SetCentreTransform(Transform centre)
    {
        transform.parent = centre;
    }

}
