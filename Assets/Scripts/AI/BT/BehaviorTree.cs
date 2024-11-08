using UnityEngine;

namespace AI.BT
{
    public abstract class BehaviorTree : MonoBehaviour
    {
        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            _root?.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
