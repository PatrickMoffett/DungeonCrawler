using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerCharacter : MonoBehaviour
    {
        PlayerMovement _playerMovement;
        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public void SetMoveInputDirection(Vector2 inputDirection)
        {
            Debug.Log("Move: " + inputDirection.x);
            _playerMovement.SetMoveInput(inputDirection);
        }
    }
}