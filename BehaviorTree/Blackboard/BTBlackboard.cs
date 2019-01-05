using System.Collections.Generic;

namespace Exanite.BehaviorTree
{
    public class BTBlackboard // Use this to save variables to the tree
    {
        protected Dictionary<string, object> _blackboard;

        public BTBlackboard(params KeyValuePair<string, object>[] vars)
        {
            _blackboard = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> var in vars)
            {
                _blackboard.Add(var.Key, var.Value);
            }
        }

        public object GetVariable(string key) 
        {

            if(Exists(key)) return _blackboard[key];
            // else
            throw new KeyNotFoundException(string.Format("The key {0} does not exist in {1}", key, _blackboard));
        }

        public void SetVariable(string key, object value) 
        {
            _blackboard[key] = value;
        }

        public bool Exists(string key) 
        {
            return _blackboard.ContainsKey(key);
        }
    }
}