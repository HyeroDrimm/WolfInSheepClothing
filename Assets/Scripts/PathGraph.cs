using System.Collections.Generic;

public class PathGraph<RealObject>
{
    public List<PathGraphNode> nodes;
    public class PathGraphNode
    {
        public RealObject realObject;
        public List<PathGraphNodeConnection> connections;
    }

    public class PathGraphNodeConnection
    {
        public PathGraphNode neighbourNode;
        public float weight;
    }
}
