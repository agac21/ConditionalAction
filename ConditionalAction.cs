using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.ConditionalActionSystem
{
    public class ConditionalAction
    {
        private Action<object> _onTrue;
        private Action<Condition> _onFalse;

        private List<Condition> _conditions;

        public static ConditionalAction Create()
        {
            var conditionalAction = new ConditionalAction();
            conditionalAction._conditions = new List<Condition>();
            return conditionalAction;
        }


        public ConditionalAction If(Condition condition)
        {
            _conditions.Add(condition);
            return this;
        }


        public ConditionalAction Listen()
        {
            for (var i = 0; i < _conditions.Count; i++)
            {
                var condition = _conditions[i];
                condition.OnFalseEvent += () => TryInvoke();
                if (i == _conditions.Count - 1)
                {
                    condition.Triggered += TryInvokeOnLastCond;
                }
            }

            return this;
        }

        private void TryInvokeOnLastCond(object data)
        {
            TryInvoke();
        }

        public ConditionalAction TryInvoke()
        {
            foreach (var c in _conditions)
            {
                if (!c.Flag)
                {
                    _onFalse?.Invoke(c);
                    return this;
                }
            }

            var lastCondition = _conditions.Last();
            _onTrue?.Invoke(lastCondition.GetTrueOutput());

            return this;
        }

        public ConditionalAction OnFalse(Action<Condition> action)
        {
            _onFalse = action;
            return this;
        }

        public ConditionalAction OnTrue(Action<object> action)
        {
            _onTrue = action;
            return this;
        }

        public IReadOnlyList<Condition> GetConditions()
        {
            return _conditions;
        }

        public void RemoveCondition(Condition target)
        {
            var index = _conditions.FindIndex(c => c == target);
            if (index < 0) return;
            for (var i = index + 1; i < _conditions.Count; i++)
            {
                if (_conditions[i].Triggers.Contains(target))
                {
                    _conditions[i].Unbind(target);
                    if (i > 0) _conditions[i].Bind(_conditions[i - 1]);
                }
            }

            if (index == _conditions.Count - 1)
            {
                _conditions[index].Triggered -= TryInvokeOnLastCond;
                _conditions[index - 1].Triggered += TryInvokeOnLastCond;
            }

            target.UnbindAll();
            _conditions.Remove(target);
        }

        public void InsertToBegin(Condition condition)
        {
            _conditions.Insert(0, condition);
        }
    }
}