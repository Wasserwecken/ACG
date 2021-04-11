﻿using Framework.Extensions;
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
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public CameraControllerSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var transform = ref entity.Get<TransformComponent>();
            var controller = entity.Get<CameraControllerComponent>();
            var input = _worldComponents.Get<InputComponent>();
            var time = _worldComponents.Get<TimeComponent>();

            var moveInput = Vector2.Zero;
            if (input.Keyboard.IsKeyDown(Keys.W)) moveInput.Y += 1;
            if (input.Keyboard.IsKeyDown(Keys.S)) moveInput.Y -= 1;
            if (input.Keyboard.IsKeyDown(Keys.A)) moveInput.X -= 1;
            if (input.Keyboard.IsKeyDown(Keys.D)) moveInput.X += 1;
            moveInput *= time.DeltaFrame * controller.MoveSpeed;

            var lookInput = Vector2.Zero;
            lookInput.X = input.Mouse.Delta.X;
            lookInput.Y = input.Mouse.Delta.Y;
            lookInput *= controller.LookSpeed * 0.005f;

            transform.Position += transform.Right * moveInput.X;
            transform.Position += transform.Forward * -moveInput.Y;
            transform.Forward = transform.Forward.Rotate(lookInput.X, Vector3.UnitY);
            transform.Forward = transform.Forward.Rotate(lookInput.Y, transform.Right);
        }
    }
}
