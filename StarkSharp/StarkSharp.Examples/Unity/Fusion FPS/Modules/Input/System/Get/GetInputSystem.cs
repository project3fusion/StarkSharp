using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ECS
{
    //Systembase is not burst compilable, use ISystem interface for burst compile
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class GetInputSystem : SystemBase
    {
        private InputActions playerInputActions;
        private Entity playerOwnerEntity;
        private Player player;

        //Base methods

        protected override void OnCreate()
        {
            //Wait for Player Owner Tag & Player Movement Input Data before update
            RequireForUpdate<PlayerOwnerTag>();
            RequireForUpdate<PlayerMovementInputData>();

            //Create input actions
            playerInputActions = new InputActions();
        }

        protected override void OnStartRunning()
        {
            //Enable Player Actions to start getting input
            playerInputActions.Enable();

            //Add performed actions to the Player Input Actions
            playerInputActions.Player.Attack.performed += OnPlayerAttackInput;
            playerInputActions.Player.Rotation.performed += OnPlayerRotationInput;
            playerInputActions.Player.Movement.performed += OnPlayerMovementInput;

            //Get player owner entity
            playerOwnerEntity = SystemAPI.GetSingletonEntity<PlayerOwnerTag>();

            //Get player aspect from player owner entity
            player = SystemAPI.GetAspect<Player>(playerOwnerEntity);
        }

        protected override void OnUpdate()
        {
            //Nothing to be implemented in here
        }

        protected override void OnStopRunning()
        {
            //Remove performed actions to the Player Input Actions
            playerInputActions.Player.Attack.performed -= OnPlayerAttackInput;
            playerInputActions.Player.Rotation.performed -= OnPlayerRotationInput;
            playerInputActions.Player.Movement.performed -= OnPlayerMovementInput;

            //Set Player Owner Entity to null
            playerOwnerEntity = Entity.Null;

            //Disable Player Actions to stop getting input
            playerInputActions.Disable();
        }

        //Actions

        private void OnPlayerAttackInput(InputAction.CallbackContext obj)
        {
            //If player owner doesn't exist return
            if (!SystemAPI.Exists(playerOwnerEntity)) return;

            //Set targeter tag to true
            SystemAPI.SetComponentEnabled<TargeterTag>(playerOwnerEntity, true);
        }

        private void OnPlayerRotationInput(InputAction.CallbackContext obj)
        {
            //If player owner doesn't exist return
            if (!SystemAPI.Exists(playerOwnerEntity)) return;

            //Get rotation input from Player Input Actions
            var rotationInput = playerInputActions.Player.Rotation.ReadValue<Vector2>();

            //Set Player Rotation Input Data's value to rotation input
            SystemAPI.SetSingleton(new PlayerRotationInputData { Value = rotationInput });
        }

        private void OnPlayerMovementInput(InputAction.CallbackContext obj)
        {
            //If player owner doesn't exist return
            if (!SystemAPI.Exists(playerOwnerEntity)) return;

            //Get movement input from Player Input Actions
            var movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();

            //Set Player Movement Input Data's value to movement input
            SystemAPI.SetSingleton(new PlayerMovementInputData { Value = movementInput });
        }
    }
}