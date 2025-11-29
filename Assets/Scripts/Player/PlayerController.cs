using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerControls _playerControls;
        private PlayerCharacter _playerCharacter;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerCharacter = GetComponent<PlayerCharacter>();
            
            _playerControls.Player.Move.performed += OnMovePerformed;
            _playerControls.Player.Move.canceled += OnMoveCanceled;
        }
        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }
        private void OnMovePerformed(InputAction.CallbackContext ctx) => Move(ctx.ReadValue<Vector2>());
        private void OnMoveCanceled(InputAction.CallbackContext ctx) => Move(Vector2.zero);

        private void Move(Vector2 vector2)
        {
            _playerCharacter.SetMoveInputDirection(vector2);
        }
    }
}