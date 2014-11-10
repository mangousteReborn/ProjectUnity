using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Final_Assets.Scripts.Class
{
    public class StackSchemClass
    {
        private ActionInterface _script;
        public ActionInterface Script
        {
            get { return this._script; }
        }

        private object[] _param;
        public object[] Param
        {
            get { return this._param; }
        }

        private float _time;
        public float Time
        {
            get { return this._time; }
        }

        public StackSchemClass(ActionInterface script, object[] param, float time)
        {
            this._script = script;
            this._param = param;
            this._time = time;
        }
    }
}
