using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyLib;

public class Player_Controller : MonoBehaviour {

    public float rotSpeed;
    public float moveSpeed;
    public Camera playerCamera;
    public GameObject target;

    [NetVar]
    public TextMesh playerText;
    [NetVar]
    public int platform;

    private Vector2 _targetPos;
    private Vector2 _secondPos;
    private GalaxyNetID _netID;
    private GameObject _targetSphere;

    private void Awake()
    {
        _netID = this.transform.GetComponent<GalaxyNetID>();
        Messenger<Camera>.AddListener(GameEvent.PLAYER_CAMERA, PlayerCamera);
    }

    void Update ()
    {
        if (_netID.isMy)
        {
            float distance = Vector3.Distance(this.transform.position, _targetPos);
            _secondPos = new Vector2(transform.position.x, transform.position.y);
            //print(distance);
            
            if (distance > 1.5)
            {
                this.transform.position = Vector2.Lerp(this.transform.position, _targetPos, Time.deltaTime* moveSpeed);                
            }
            else if (distance > 0)
            {
                if (_targetSphere != null)
                {
                    Destroy(_targetSphere);
                    _targetSphere = null;
                }
            }
            if (_targetPos != Vector2.zero)
            {
                transform.right = Vector3.Lerp(transform.right, (_targetPos - _secondPos), rotSpeed * Time.deltaTime);
            }
            if (Input.GetMouseButtonDown(0))
            {                
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "gameMap")
                    {
#if UNITY_ANDROID
                        platform = 1;
                        playerText.text = platform.ToString();
                        if (Input.touchCount == 1 || Input.touchCount == 2)
                        {
                            if (_targetSphere != null)
                            {
                                Destroy(_targetSphere);
                                _targetSphere = null;
                            }
                            _targetPos = hit.point;
                            _targetSphere = Instantiate(target, hit.point, target.transform.rotation);
                            _targetSphere.transform.position = new Vector3(_targetSphere.transform.position.x, _targetSphere.transform.position.y, 0);
                        }
#endif
#if UNITY_STANDALONE_WIN
                        platform = 2;
                        playerText.text = platform.ToString();
                        if (_targetSphere != null)
                        {
                            Destroy(_targetSphere);
                            _targetSphere = null;
                        }
                        _targetPos = hit.point;
                        _targetSphere = Instantiate(target, hit.point, target.transform.rotation);
                        _targetSphere.transform.position = new Vector3(_targetSphere.transform.position.x, _targetSphere.transform.position.y, 0);
#endif

                    }
                }
            }
        }        
    }

    private void PlayerCamera (Camera cam)
    {
        playerCamera = cam;
    }

    private void OnDestroy()
    {
        Messenger<Camera>.RemoveListener(GameEvent.PLAYER_CAMERA, PlayerCamera);
    }
}
