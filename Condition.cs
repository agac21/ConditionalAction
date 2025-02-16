using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.ConditionalActionSystem
{
    public abstract class Condition : ITrigger
    {
        protected internal bool Flag { get; protected set; }

        public event OnTriggered Triggered;
        public event Action OnFalseEvent;

        internal readonly List<ITrigger> Triggers = new();

        private Action _trueAction;
        private Action _falseAction;

        public void Bind(ITrigger trigger)
        {
            trigger.Triggered += Evaluate;
            Triggers.Add(trigger);
        }

        public void Unbind(ITrigger trigger)
        {
            trigger.Triggered -= Evaluate;
            Triggers.Remove(trigger);
        }

        public void UnbindAll()
        {
            for (var i = Triggers.Count - 1; i >= 0; i--)
            {
                var trigger = Triggers[i];
                trigger.Triggered -= Evaluate;
                Triggers.Remove(trigger);
            }
        }

        protected void InvokeTrigger(object data)
        {
            Triggered?.Invoke(data);
        }

        public void Evaluate(object data)
        {
            EvaluateProtected(data);
            Debug.Log($"Condition {GetType().Name} evaluated to {Flag}");
            if (Flag)
            {
                InvokeTrueCallback();
            }
            else
            {
                InvokeFalseCallback();
                OnFalseEvent?.Invoke();
            }
        }

        protected abstract void EvaluateProtected(object data);

        public Condition OnTrue(Action action)
        {
            _trueAction = action;
            return this;
        }

        public Condition OnFalse(Action action)
        {
            _falseAction = action;
            return this;
        }

        public virtual object GetFalseOutput()
        {
            return default;
        }

        public virtual object GetTrueOutput()
        {
            return default;
        }

        private void InvokeTrueCallback()
        {
            _trueAction?.Invoke();
        }

        private void InvokeFalseCallback()
        {
            _falseAction?.Invoke();
        }
    }
}