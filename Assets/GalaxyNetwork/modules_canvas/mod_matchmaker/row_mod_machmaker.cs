using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class row_mod_machmaker : MonoBehaviour {

    public Text text_col1;
    public Text text_col2;
    public Image img_owner;

    public uint clientId;
    public bool isMy
    {
        get
        {
            return _isMy;
        }
        set
        {
            _isMy = value;
            //if (_isMy == true) img_host.enabled = true;
            //else img_host.enabled = false;
        }
    }
    bool _isMy;

    public bool isOwner
    {
        get
        {
            return _isOwner;
        }
        set
        {
            _isOwner = value;
            if (_isOwner == true) img_owner.enabled = true;
            else img_owner.enabled = false;
        }
    }
    bool _isOwner;

    public string playerName
    {
        get
        {
            return _playerName;
        }
        set
        {
            _playerName = value;
            text_col2.text = value;
        }
    }
    string _playerName;

    public ushort avaId;
    public Vector3 position;
    public Quaternion rotation;
}
