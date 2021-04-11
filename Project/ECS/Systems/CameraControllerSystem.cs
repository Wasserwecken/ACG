using Framework.Extensions;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Project.ECS.Components;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using DefaultEcs;
using DefaultEcs.System;

namespace Project.ECS.Systems
{
    [With(typeof(CameraControllerComponent))]
    [With(typeof(TransformComponent))]
    public class CameraControllerSystem : AEntitySetSystem<bool>
    {
        private readonly InputComponent _input;
        private readonly TimeComponent _time;

        /// <summary>
        /// 
        /// </summary>
        public CameraControllerSystem(World world, Entity worldComponents) : base(world)
        {
            _input = worldComponents.Get<InputComponent>();
            _time = worldComponents.Get<TimeComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var controller = entity.Get<CameraControllerComponent>();
            var transform = entity.Get<TransformComponent>();


            var moveInput = Vector2.Zero;
            if (_input.Keyboard.IsKeyDown(Keys.W)) moveInput.Y += 1;
            if (_input.Keyboard.IsKeyDown(Keys.S)) moveInput.Y -= 1;
            if (_input.Keyboard.IsKeyDown(Keys.A)) moveInput.X -= 1;
            if (_input.Keyboard.IsKeyDown(Keys.D)) moveInput.X += 1;
            moveInput *= _time.DeltaFrame * controller.MoveSpeed;

            var lookInput = Vector2.Zero;
            lookInput.X = _input.Mouse.Delta.X;
            lookInput.Y = _input.Mouse.Delta.Y;
            lookInput *= controller.LookSpeed * 0.005f;


            transform.Position += transform.Right * moveInput.X;
            transform.Position += transform.Forward * -moveInput.Y;
            transform.Forward = transform.Forward.Rotate(lookInput.X, Vector3.UnitY);
            transform.Forward = transform.Forward.Rotate(lookInput.Y, transform.Right);
        }
    }
}
