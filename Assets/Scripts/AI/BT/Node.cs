using System.Collections.Generic;

namespace AI.BT
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }

        public void AddChild(Node child)
        {
            _Attach(child);
        }

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        protected void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        protected void SetDataInParent(string key, object value)
        {
            parent?.SetData(key, value);
        }
        
        public void SetDataInFirstParent(string key, object value)
        {
            if (parent == null)
            {
                SetData(key, value);
                return;
            }
            
            Node node = parent;
            while (node != null)
            {
                node = node.parent;
            }

            node?.SetData(key, value);
        }

        private object GetData(string key)
        {
            return _dataContext[key];
        }
        
        public bool TryGetData(string key, out object value)
        {
            if (_dataContext.TryGetValue(key, out value))
                return value != null;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value != null;
                node = node.parent;
            }
            return false;
        }
        
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }

}
