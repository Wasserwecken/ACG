using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Framework.ECS.Components.Scene
{
    public class InputComponent : IComponent
    {
        public KeyboardState Keyboard { get; set; }
        public MouseState Mouse { get; set; }
    }
}
