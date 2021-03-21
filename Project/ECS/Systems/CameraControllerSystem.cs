using Framework.ECS;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems;
using Project.ECS.Components;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Framework.ECS.Components.Scene;
using OpenTK.Mathematics;
using Framework;

namespace Project.ECS.Systems
{
    public class CameraControllerSystem : ISystem
    {
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var cameraEntity = entities.First(f => f.HasComponent<CameraControllerComponent>());
            var transform = cameraEntity.Components.Get<TransformComponent>();
            var controller = cameraEntity.Components.Get<CameraControllerComponent>();
            var input = sceneComponents.First(f => f is InputComponent) as InputComponent;
            var time = sceneComponents.First(f => f is TimeComponent) as TimeComponent;


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
