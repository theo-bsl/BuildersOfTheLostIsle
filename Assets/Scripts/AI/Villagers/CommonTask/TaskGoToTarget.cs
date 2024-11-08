using AI.BT;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private readonly Transform _transform;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly float _speed;
    private readonly float _minDistance;

    public TaskGoToTarget(Transform transform, NavMeshAgent navMeshAgent, float speed, float minDistance)
    {
        _transform = transform;
        _navMeshAgent = navMeshAgent;
        _speed = speed;
        _minDistance = minDistance;
    }
    
    public override NodeState Evaluate()
    {
        if (!TryGetData("Target", out object data))
            return NodeState.FAILURE;
        
        Vector3 target = (Vector3)data;

        if (Vector3.Distance(_transform.position, target) > _minDistance)
        {
            if (_navMeshAgent.destination != target)
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = _speed;
                _navMeshAgent.SetDestination(target);
            }
            
            return NodeState.RUNNING;
        }
        
        _navMeshAgent.isStopped = true;
        
        return NodeState.SUCCESS;
    }
}
