using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Framework.ECS.Components.Scene
{
    public struct InputComponent
    {
        public KeyboardState Keyboard { get; set; }
        public MouseState Mouse { get; set; }
    }
}
