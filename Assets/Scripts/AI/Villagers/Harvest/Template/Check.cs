using System;
using AI.BT;

public class Check<T> : Node where T : Enum
{
    private readonly T _typeToCheck;
    private readonly string _dataKey;
    private readonly Func<T, bool> _checkNeed;

    public Check(T typeToCheck, string dataKey, Func<T, bool> checkNeed)
    {
        _typeToCheck = typeToCheck;
        _dataKey = dataKey;
        _checkNeed = checkNeed;
    }

    public override NodeState Evaluate()
    {
        if (!_checkNeed(_typeToCheck))
            return NodeState.FAILURE;
        
        SetDataInParent(_dataKey, _typeToCheck);

        return NodeState.SUCCESS;
    }
}
