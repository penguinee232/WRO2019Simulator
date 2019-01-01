using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public class SetVariable : Command
    {
        SetVariablePhrase SetVariablePhrase { get; set; }
        public SetVariable()
        {
            SetVariablePhrase = new SetVariablePhrase();
            SetVisulizeItems();
        }
        private SetVariable(SetVariable original)
        {
            Form = original.Form;
            SetVariablePhrase = new SetVariablePhrase();
            original.SetVariablePhrase.CopyTo(SetVariablePhrase);
            SetVisulizeItems();
        }
        void SetVisulizeItems()
        {
            VisulizeItems = new List<IGetSetFunc>()
            {
                new GetSetFunc<SetVariablePhrase>((i)=>SetVariablePhrase, (v,i)=>SetVariablePhrase=v, "SetVariablePhrase")
            };
            Init(false);
        }

        public override Command CompleteCopy()
        {
            return new SetVariable(this);
        }

        public override Queue<Action> GetActions(Robot robot)
        {
            Queue<Action> queue = new Queue<Action>();
            queue.Enqueue(new Action(new List<Request>() { new SetVariableRequest(SetVariablePhrase) }));
            return queue;
        }
    }

    public class SetVariableRequest : Request
    {
        SetVariablePhrase setVariablePhrase;
        public SetVariableRequest(SetVariablePhrase setVariablePhrase)
        {
            this.setVariablePhrase = setVariablePhrase;
        }
        public override void InitRequest(Robot robot)
        {
        }
        public override bool UpdateRequest(Robot robot)
        {
            setVariablePhrase.SetVariableValue();
            return false;
        }
    }
}
