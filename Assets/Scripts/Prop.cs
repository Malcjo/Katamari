using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum enumPropSize
{
    tiny, small, medium, large, enormous, humongous
}
public class Prop : MonoBehaviour
{
    [SerializeField] enumPropSize propSize = new enumPropSize();
    private float _PropAddMass;
    private float _propSize;
    private float _propAddRadius;
    private float _propAddSize;
    private float _camAddRadius;
    public float PropSize { get{ return _propSize; } }
    public float CamAddRadius { get { return _camAddRadius; } }
    public float PropAddradius { get { return _propAddRadius; } set { _propAddRadius = value; } }
    public float PropAddSize { get { return _propAddSize; } set { _propAddSize = value; } }
    public float PropAddMass {get{ return _PropAddMass; } set{ _PropAddMass = value; }}

    private void Start()
    {
        SetPropSize();
    }
    private void SetPropSize()
    {
        switch (propSize)
        {
            case enumPropSize.tiny:
                _PropAddMass = 0.001f;
                _propSize = 1.0f;
                _propAddRadius = 0.0005f;
                _propAddSize = 0.02f;
                _camAddRadius = 0.001f;
                break;
            case enumPropSize.small:
                _PropAddMass = 0.003f;
                _propSize = 2f;
                _propAddRadius = 0.0005f;
                _propAddSize = 0.03f;
                _camAddRadius = 0.01f;
                break;
            case enumPropSize.medium:
                _PropAddMass = 0.006f;
                _propSize = 3f;
                _propAddRadius = 0.001f;
                _propAddSize = 0.04f;
                _camAddRadius = 0.03f;
                break;
            case enumPropSize.large:
                _PropAddMass = 0.009f;
                _propSize = 5.0f;
                _propAddRadius = 0.01f;
                _propAddSize = 0.06f;
                _camAddRadius = 0.1f;
                break;
            case enumPropSize.enormous:
                _PropAddMass = 0.012f;
                _propSize = 7.0f;
                _propAddRadius = 0.1f;
                _propAddSize = 0.06f;
                _camAddRadius = 0.5f;
                break;
            case enumPropSize.humongous:
                _PropAddMass = 0.02f;
                _propSize = 10.0f;
                _propAddRadius = 0.25f;
                _propAddSize = 0.1f;
                _camAddRadius = 1f;
                break;
        }
    }


}
