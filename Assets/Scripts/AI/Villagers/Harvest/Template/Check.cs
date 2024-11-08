using System;
using AI.BT;

public class Check<T> : Node where T : Enum
{
    private readonly T _dataToCheck;
    private readonly string _dataKey;
    private readonly Func<T, bool> _checkData;

    public Check(T dataToCheck, string dataKey, Func<T, bool> checkData)
    {
        _dataToCheck = dataToCheck;
        _dataKey = dataKey;
        _checkData = checkData;
    }

    public override NodeState Evaluate()
    {
        if (!_checkData(_dataToCheck))
            return NodeState.FAILURE;
        
        SetDataInParent(_dataKey, _dataToCheck);

        return NodeState.SUCCESS;
    }
}
