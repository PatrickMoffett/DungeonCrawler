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

            v += _inputDirection * (acceleration * dt);

            var brake = dampening * dt;
            float newSpeed = v.magnitude - brake;
            if (newSpeed < 0f)
            {
                newSpeed = 0f;
            }
            v = v.normalized * newSpeed;
            
            _rigidbody.linearVelocity = v;
        }
        
        public void SetMoveInput(Vector2 moveDirection)
        {
            Debug.Log("SetMoveInput: " + moveDirection.x);
            _inputDirection = moveDirection;  
        }
    }
}