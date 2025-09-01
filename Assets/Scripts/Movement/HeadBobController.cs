using UnityEngine;


public class HeadBobController : MonoBehaviour
{
    [Header("Enable")]
    [SerializeField] private bool _enable = true;

    [Header("Motion Settings")]
    [SerializeField, Range(0f, 0.1f)] private float _amplitude = 0.05f;
    [SerializeField, Range(0f, 30f)] private float _frequency = 10.0f;

    [Header("References")]
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;

    [Header("Jump Settings")]
    [SerializeField, Range(0f, 0.3f)] private float _jumpBobAmplitude = 0.1f;
    [SerializeField, Range(0f, 5f)] private float _jumpBobDuration = 0.3f;

    private bool _wasGrounded;
    private float _jumpBobTimer;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPos = _camera.localPosition;
    }

    private void Update()
    {
        if (!_enable) return;
        CheckMotion();
        HandleJumpBob();
        ResetPosition();
        _camera.LookAt(FocusTarget());
        _wasGrounded = _controller.isGrounded;
    }
    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }
    private void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        if (speed < _toggleSpeed) return;
        if (!_controller.isGrounded) return; //umändern in meinen eigenen groundcheck

        PlayMotion(FootStepMotion());

    }
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
            pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
            pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }
    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }
    private Vector3 FocusTarget()
    {
       Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * 10;
        return pos;
    }
    private void HandleJumpBob()
    {
        // Start JumpBob wenn wir in die Luft gehen
        if (_wasGrounded && !_controller.isGrounded)
        {
            _jumpBobTimer = _jumpBobDuration;
        }

        // Landebob (extra Effekt beim Aufkommen)
        if (!_wasGrounded && _controller.isGrounded)
        {
            _jumpBobTimer = _jumpBobDuration;
        }

        // Timer runterzählen
        if (_jumpBobTimer > 0)
        {
            float bobOffset = Mathf.Sin((1 - (_jumpBobTimer / _jumpBobDuration)) * Mathf.PI) * _jumpBobAmplitude;
            PlayMotion(new Vector3(0, -Mathf.Abs(bobOffset), 0));
            _jumpBobTimer -= Time.deltaTime;
        }
    }
}
