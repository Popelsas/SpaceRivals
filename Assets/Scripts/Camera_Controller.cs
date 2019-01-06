using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour {

    public float maxZoom;
    public float minZoom;
    public float speedZoom;

    private float _zoom;
    private bool activate = false;
    private GameObject _target;
    private float distance; 

    private void Awake()
    {
        Messenger<bool, GameObject>.AddListener(GameEvent.CONNECTED, Connected);
    }

    private void LateUpdate()
    {
        if (activate)
        {
            if (_target != null)
            {
                transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y, transform.position.z);
            }
#if UNITY_STANDALONE_WIN
            ZoomWin();
#endif
#if UNITY_EDITOR
            ZoomWin();
#endif            
#if UNITY_ANDROID
            if (Input.touchCount == 2) ZoomAndroid();
            else if (distance != 0) distance = 0;
#endif
        }
    }


    private void ZoomWin()
    {        
        _zoom = Input.GetAxis("Mouse ScrollWheel");
        //print(transform.position.z);
        if (_zoom != 0)
        {
            float posZ = transform.position.z;
            posZ += _zoom * speedZoom;
            posZ = Mathf.Clamp(posZ, minZoom, maxZoom);
            transform.position = new Vector3(transform.position.x, transform.position.y, posZ);
        }

    }
    private void ZoomAndroid()
    {
        Vector2 finger1 = Input.GetTouch(0).position;
        Vector2 finger2 = Input.GetTouch(2).position;

        if (distance == 0) distance = Vector2.Distance(finger1, finger2);

        float delta = Vector2.Distance(finger1, finger2) - distance;

        float posZ = transform.position.z;
        posZ += delta * speedZoom;
        posZ = Mathf.Clamp(posZ, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, transform.position.y, posZ);
        distance = Vector2.Distance(finger1, finger2);
    }
    private void Connected(bool connect, GameObject player)
    {
        if (connect)
        {
            activate = true;
            _target = player;
            Messenger<Camera>.Broadcast(GameEvent.PLAYER_CAMERA, GetComponent<Camera>());
        }
    }

    private void OnDestroy()
    {
        Messenger<bool, GameObject>.RemoveListener(GameEvent.CONNECTED, Connected);
    }
}
