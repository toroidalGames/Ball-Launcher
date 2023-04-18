using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] Camera _mainCamera;

    [SerializeField] Rigidbody2D _currentBallRb;
    [SerializeField] Rigidbody2D _pivotRb;

    [SerializeField] SpringJoint2D _springJoint;

    [SerializeField] float _launchDelay = 0.4f;
    [SerializeField] float _respawnDelay = 1f;

    bool _isLaunchable = true;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //_currentBallRb.isKinematic = true;
        SpawnNewBall();
    }
    void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(_ballPrefab, _pivotRb.position, Quaternion.identity);

        _currentBallRb = ballInstance.GetComponent<Rigidbody2D>();
        _springJoint = _currentBallRb.GetComponent<SpringJoint2D>();

        _springJoint.connectedBody = _pivotRb;

        _isLaunchable = true;

    }
    // Update is called once per frame
    void Update()
    {
        if (_isLaunchable == false)
        {
            return;
        }

        bool isPressed = Touchscreen.current.primaryTouch.press.isPressed;
        if (isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(touchPosition);

            _currentBallRb.position = worldPosition;


        }
        bool screenReleased = Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;

        if (screenReleased)
        {
            LaunchBall();
        }
    }

    void LaunchBall()
    {
        Debug.Log("Launchball called");
        _currentBallRb.isKinematic = false;
        _isLaunchable = false;
        StartCoroutine(DetachBall());
    }

    IEnumerator DetachBall()
    {
        yield return new WaitForSeconds(_launchDelay);
        _springJoint.enabled = false;
        _springJoint = null;
        yield return new WaitForSeconds(_respawnDelay);
        SpawnNewBall();
    }
}
