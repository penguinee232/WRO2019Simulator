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
        public static ControlWrapper Debug;
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
        public override List<Control> GetManatoryControls(IGetSetFunc getSetFunc, int index)
        {
            (IGetSetFunc getSetFunc, int index) info = (getSetFunc, index);
            Button addButton = new Button();
            addButton.Text = "Add";
            var addWrapper = new ControlWrapper(addButton, info);
            Debug = addWrapper;
            addButton.Click += addWrapper.Control_ValueChanged;
            addWrapper.ValueChanged += AddButton_Click;

            Button insertButton = new Button();
            insertButton.Text = "Insert";
            var insertWrapper = new ControlWrapper(insertButton, info);
            insertButton.Click += insertWrapper.Control_ValueChanged;
            insertWrapper.ValueChanged += InsertButton_Click;

            Button removeButton = new Button();
            removeButton.Text = "Remove";
            var removeWrapper = new ControlWrapper(removeButton, info);
            removeButton.Click += removeWrapper.Control_ValueChanged;
            removeWrapper.ValueChanged += RemoveButton_Click;

            return new List<Control>() { addButton, insertButton, removeButton };
        }
        private void InsertButton_Click(object sender, InfoEventArgs e)
        {
            var info = ((IGetSetFunc getSetFunc, int index))e.Info;
            VisulizeableList<T> list = (VisulizeableList<T>)info.getSetFunc.ObjGet(info.index);
            LabeledControl currentControl = list.ControlNode.Control;
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
            int previousSelectedIndex = currentControl.RadioButtonGroup.SelectedIndex;
            if (Insert(currentControl.RadioButtonGroup.SelectedIndex, defaultT))
            {
                ControlNode parentNode = list.ControlNode;
                Form1.UpdateItem(ref parentNode, parentNode.Control.GetSetFunc, parentNode.Control.Index, parentNode.Control.Form);
                list.ControlNode = parentNode;
                currentControl = list.ControlNode.Control;
                currentControl.RadioButtonGroup.ChangeIndex(previousSelectedIndex);
            }
        }

        private void RemoveButton_Click(object sender, InfoEventArgs e)
        {
            var info = ((IGetSetFunc getSetFunc, int index))e.Info;
            VisulizeableList<T> list = (VisulizeableList<T>)info.getSetFunc.ObjGet(info.index);

            LabeledControl currentControl = list.ControlNode.Control;
            int previousSelectedIndex = currentControl.RadioButtonGroup.SelectedIndex;
            RemoveAt(currentControl.RadioButtonGroup.SelectedIndex);
            currentControl.RadioButtonGroup.Buttons.RemoveAt(currentControl.RadioButtonGroup.SelectedIndex);
            ControlNode parentNode = list.ControlNode;
            Form1.UpdateItem(ref parentNode, parentNode.Control.GetSetFunc, parentNode.Control.Index, parentNode.Control.Form);
            list.ControlNode = parentNode;
            currentControl = list.ControlNode.Control;
            currentControl.RadioButtonGroup.ChangeIndex(Math.Min(previousSelectedIndex, Math.Max(0, currentControl.RadioButtonGroup.Buttons.Count - 1)));
        }

        private static void AddButton_Click(object sender, InfoEventArgs e)
        {
            var info = ((IGetSetFunc getSetFunc, int index))e.Info;
            VisulizeableList<T> list = (VisulizeableList<T>)info.getSetFunc.ObjGet(info.index);
            T defaultT;
            if (list.GetNewItem != null)
            {
                defaultT = list.GetNewItem.Invoke();
            }
            else
            {
                defaultT = default(T);
                if (defaultT == null)
                {
                    defaultT = Extensions.GetDefaultFromConstructor<T>();
                }
            }
            LabeledControl currentControl = list.ControlNode.Control;
            int previousSelectedIndex = currentControl.RadioButtonGroup.SelectedIndex;
            if (list.Add(defaultT))
            {
                ControlNode parentNode = list.ControlNode;
                Form1.UpdateItem(ref parentNode, parentNode.Control.GetSetFunc, parentNode.Control.Index, parentNode.Control.Form);
                list.ControlNode = parentNode;
                currentControl = list.ControlNode.Control;
                currentControl.RadioButtonGroup.ChangeIndex(currentControl.RadioButtonGroup.Buttons.Count - 1);
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
