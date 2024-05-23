using RBush;

namespace Models.Navigation
{
    public class PointWithNode : ISpatialData
    {
        private readonly Envelope envelope;

        public PointWithNode(float x, float y)
        {
            X = x;
            Y = y;
            envelope = new Envelope(X, Y, X, Y);
        }

        public PointWithNode(NavigationNode node)
        {
            Node = node;
            X = node.Position.x;
            Y = node.Position.z;
            envelope = new Envelope(X, Y, X, Y);
        }

        public float X { get; set; }
        public float Y { get; set; }
        public NavigationNode Node { get; set; }

        public ref readonly Envelope Envelope => ref envelope;
    }
}