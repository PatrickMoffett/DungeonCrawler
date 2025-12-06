using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 _velocity;
    private Vector2 _inputDirection;
    private Rigidbody2D _rigidbody;
    
    [SerializeField]
    private float acceleration = 25f;
    [SerializeField]
    private float maxSpeed = 25f;
    [SerializeField]
    private float dampening = 25f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        var v = _rigidbody.linearVelocity;
        var dt = Time.deltaTime;

        if (Mathf.Approximately(_inputDirection.x,0f)
            || (v.x > 0f && _inputDirection.x < 0f)
            || (v.x < 0f && _inputDirection.x > 0f))
        {
            var brake = dampening * dt;
            float newSpeed = Mathf.MoveTowards(v.x, 0f, brake);
            v.x = newSpeed;
        }

        if (Mathf.Approximately(_inputDirection.y,0f)
            || (v.y > 0f && _inputDirection.y < 0f)
            || (v.y < 0f && _inputDirection.y > 0f))
        {
            var brake = dampening * dt;
            float newSpeed = Mathf.MoveTowards(v.y, 0f, brake);
            v.y = newSpeed;
        }
        v += _inputDirection * (acceleration * dt);

        if (v.sqrMagnitude > maxSpeed * maxSpeed)
        {
            v = v.normalized * maxSpeed;
        }
        _rigidbody.linearVelocity = v;
    }
    
    public void SetMoveInput(Vector2 moveDirection)
    {
        _inputDirection = moveDirection;  
    }
}
} // namespace Player