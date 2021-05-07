using OpenTK.Mathematics;

namespace Framework.Assets.Textures
{
    public class TextureSpace
    {
        public readonly Vector3 Space;
        public int Resolution { get; private set; }
        public bool IsFull { get; private set; }
        public bool IsLeaf { get; private set; }
        public bool HasChildren { get; private set; }
        public TextureSpace[] Children { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TextureSpace(int resolution) : this(resolution, new Vector3(0f, 0f, 1f)) { }

        /// <summary>
        /// 
        /// </summary>
        public TextureSpace(int resolution, Vector3 space)
        {
            IsFull = false;
            IsLeaf = false;
            HasChildren = false;
            Children = null;
            Space = space;
            Resolution = resolution;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Add(int resolution, out Vector3 space)
        {
            space = Space;

            if (IsLeaf || IsFull || resolution < 8)
                return false;

            else if (resolution == Resolution && !HasChildren)
            {
                IsLeaf = true;
                return true;
            }

            else if (resolution < Resolution)
            {
                if (Children == null)
                {
                    var halfResolution = Resolution / 2;
                    var halfSize = Space.Z / 2f;

                    Children = new TextureSpace[]
                    {
                            new TextureSpace(halfResolution, new Vector3(Space.X, Space.Y, halfSize)),
                            new TextureSpace(halfResolution, new Vector3(Space.X + halfSize, Space.Y, halfSize)),
                            new TextureSpace(halfResolution, new Vector3(Space.X, Space.Y + halfSize, halfSize)),
                            new TextureSpace(halfResolution, new Vector3(Space.X + halfSize, Space.Y + halfSize, halfSize))
                    };
                }

                if (Children[0].Add(resolution, out space) ||
                    Children[1].Add(resolution, out space) ||
                    Children[2].Add(resolution, out space) ||
                    Children[3].Add(resolution, out space))
                {
                    HasChildren = true;
                    return true;
                }

                IsFull = true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            IsFull = false;
            IsLeaf = false;
            HasChildren = false;

            if (Children != null)
            {
                Children[0].Clear();
                Children[1].Clear();
                Children[2].Clear();
                Children[3].Clear();
            }
        }
    }
}
