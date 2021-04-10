namespace Framework.ECS.Components.Scene
{
    public struct TimeComponent
    {
        public float Total { get; set; }
        public float TotalSin { get; set; }
        public float DeltaFrame { get; set; }
        public float DeltaFixed { get; set; }
    }
}
