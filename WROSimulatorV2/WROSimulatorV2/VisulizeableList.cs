using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class VisulizeableList<T> : VisulizableItem, IVisulizeableList
    {
        public Func<T, bool> CanAdd { get; set; }
        public Func<T> GetNewItem { get; set; }
        public List<T> List { get; private set; }
        public T this[int index]
        {
            get
            {
                return List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        public VisulizeableList()
            : this(null)
        {

        }
        public VisulizeableList(IEnumerable<T> other)
        {
            CanAdd = null;
            if (other == null)
            {
                List = new List<T>();
            }
            else
            {
                List = new List<T>(other);
            }
            SetVisItems();
        }
        void SetVisItems()
        {
            VisulizeItems = new List<IGetSetFunc>();
            for (int i = 0; i < List.Count; i++)
            {
                VisulizeItems.Add(new GetSetFunc<T>((j) => List[j], (v, j) => List[j] = v, "Item " + i));
            }
            Init();
        }
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            SetVisItems();
        }
        public bool Insert(int index, T item)
        {
            if (CanAdd == null || CanAdd.Invoke(item))
            {
                List.Insert(index, item);
                SetVisItems();
                return true;
            }
            return false;
        }
        public bool Add(T item)
        {
            if (CanAdd == null || CanAdd.Invoke(item))
            {
                List.Add(item);
                SetVisItems();
                return true;
            }
            return false;
        }
        public override List<Control> GetManatoryControls()
        {
            Button addButton = new Button();
            addButton.Text = "Add";
            addButton.Click += AddButton_Click;
            Button insertButton = new Button();
            insertButton.Text = "Insert";
            insertButton.Click += InsertButton_Click;
            Button removeButton = new Button();
            removeButton.Text = "Remove";
            removeButton.Click += RemoveButton_Click;
            return new List<Control>() { addButton, insertButton, removeButton };
        }
        private void InsertButton_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            LabeledControl parent = (LabeledControl)control.Parent;
            T defaultT;
            if (GetNewItem != null)
            {
                defaultT = GetNewItem.Invoke();
            }
            else
            {
                defaultT = default(T);
                if (defaultT == null)
                {
                    defaultT = Extensions.GetDefaultFromConstructor<T>();
                }
            }
            int previousSelectedIndex = parent.RadioButtonGroup.SelectedIndex;
            if (Insert(parent.RadioButtonGroup.SelectedIndex, defaultT))
            {
                ControlNode parentNode = parent.ControlNode;
                Form1.UpdateItem(ref parentNode, parentNode.Control.GetSetFunc, parentNode.Control.Index, parentNode.Control.Form);
                parent.ControlNode = parentNode;
                parent.RadioButtonGroup.ChangeIndex(previousSelectedIndex);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            LabeledControl parent = (LabeledControl)control.Parent;
            int previousSelectedIndex = parent.RadioButtonGroup.SelectedIndex;
            RemoveAt(parent.RadioButtonGroup.SelectedIndex);
            parent.RadioButtonGroup.Buttons.RemoveAt(parent.RadioButtonGroup.SelectedIndex);
            ControlNode parentNode = parent.ControlNode;
            Form1.UpdateItem(ref parentNode, parentNode.Control.GetSetFunc, parentNode.Control.Index, parentNode.Control.Form);
            parent.ControlNode = parentNode;
            parent.RadioButtonGroup.ChangeIndex(Math.Min(previousSelectedIndex, Math.Max(0, parent.RadioButtonGroup.Buttons.Count - 1)));
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            LabeledControl parent = (LabeledControl)control.Parent;
            T defaultT;
            if (GetNewItem != null)
            {
                defaultT = GetNewItem.Invoke();
            }
            else
            {
                defaultT = default(T);
                if (defaultT == null)
                {
                    defaultT = Extensions.GetDefaultFromConstructor<T>();
                }
            }
            int previousSelectedIndex = parent.RadioButtonGroup.SelectedIndex;
            if (Add(defaultT))
            {
                ControlNode parentNode = parent.ControlNode;
                Form1.UpdateItem(ref parentNode, parentNode.Control.GetSetFunc, parentNode.Control.Index, parentNode.Control.Form);
                parent.ControlNode = parentNode;
                parent.RadioButtonGroup.ChangeIndex(parent.RadioButtonGroup.Buttons.Count - 1);
            }
        }

        public override VisulizableItem Copy()
        {
            VisulizeableList<T> item = new VisulizeableList<T>();
            item.CanAdd = CanAdd;
            item.GetNewItem = GetNewItem;
            return CopyItems(item, this);
        }

        public static implicit operator VisulizeableList<T>(List<T> list)
        {
            return new VisulizeableList<T>(list);
        }

        protected override void Deserialize(Span<char> span)
        {
            List = new List<T>();
            var newList = DeserializeItems(span);
            for (int i = 0; i < newList.Count; i++)
            {
                List.Add((T)newList[i].Value);
            }
            SetVisItems();
            for (int i = 0; i < newList.Count; i++)
            {
                VisulizeItems[i].Variable = newList[i].Variable;
            }
        }

        //public class ControlWrapper :Control
        //{
        //    public event EventHandler<VisulizedItemEventArgs> EventHappened;

        //}
    }
    public interface IVisulizeableList
    {
    }
}
