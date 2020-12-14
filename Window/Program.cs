using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Window
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameSettings = GameWindowSettings.Default;
            var nativeSettings = NativeWindowSettings.Default;
            nativeSettings.Size = new Vector2i(800, 600);

            using (var window = new Window(gameSettings, nativeSettings))
            {
                window.Run();
            }
        }
    }
}
