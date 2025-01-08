using Tools;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Vector3 _startRotation;
    
    [Header("Camera Movement")]
    [SerializeField] private float _movementSpeed = 20f;
    [SerializeField] private float _sprintMovementSpeed = 50f;
        
    [Header("Camera Rotation")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private float _rotationSpeedX = 10f;
    [SerializeField] private float _rotationSpeedY = 2.5f;
    [SerializeField] private float _rotationMinX = 10;
    [SerializeField] private float _rotationMaxX = 80;
        
    [Header("Camera Zoom")]
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _sprintZoomSpeed = 50f;
    [SerializeField] private float _zoomMin = 10f;
    [SerializeField] private float _zoomMax = 120f;
        
    private Transform _transform;
    private Camera _camera;
    
    private Vector2 _movementDirection;
    private Vector2 _rotationDirection;
    private float _zoomDirection;
    private float _zoomDistance;
    
    private float _currentMovementSpeed;
    private float _currentZoomSpeed;

    private void Awake()
    {
        _transform = transform;
        _camera = GetComponentInChildren<Camera>();
        
        Assert.IsNotNull(_camera, "No camera found");
    }

    private void Start()
    {
        SetupCamera();
        
        _currentMovementSpeed = _movementSpeed;
        _currentZoomSpeed = _zoomSpeed;
    }

    private void Update()
    {
        Move();
        Rotate();
        Zoom();
    }

    private void Move()
    {
        _transform.position += _transform.right * (_movementDirection.x * _currentMovementSpeed * Time.deltaTime) +
                               _transform.forward * (_movementDirection.y * _currentMovementSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        float yRotation = _cameraPivot.rotation.eulerAngles.y + _rotationDirection.x * _rotationSpeedX * Time.deltaTime;
        float xRotation = _cameraPivot.rotation.eulerAngles.x - _rotationDirection.y * _rotationSpeedY * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, _rotationMinX, _rotationMaxX);
        
        _transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        _cameraPivot.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void Zoom()
    {
        _zoomDistance = Mathf.MoveTowards(_zoomDistance, _zoomDistance + _zoomDirection, _currentZoomSpeed * Time.deltaTime);
        _zoomDistance = Mathf.Clamp(_zoomDistance, _zoomMin, _zoomMax);
        _cameraTransform.position = _cameraPivot.position - _cameraTransform.forward * _zoomDistance;
    }

    private void SetupCamera()
    {
        _cameraPivot.rotation = Quaternion.Euler(_startRotation);
        
        _cameraTransform.localPosition = _cameraOffset;
        _cameraTransform.LookAt(_cameraPivot.position);
        
        _zoomDistance = Vector3.Distance(_cameraPivot.position, _cameraTransform.position);
    }

    public void SetMovementDirection(Vector2 movementDirection)
    {
        _movementDirection = movementDirection;
    }

    public void SetRotationDirection(Vector2 rotationDirection)
    {
        _rotationDirection = rotationDirection;
    }

    public void SetZoomDirection(float zoomDirection)
    {
        _zoomDirection = -zoomDirection.Sign();
    }

    public void SetSpeed(bool isSprinting)
    {
        _currentMovementSpeed = isSprinting ? _sprintMovementSpeed : _movementSpeed;
        _currentZoomSpeed = isSprinting ? _sprintZoomSpeed : _zoomSpeed;
    }
}