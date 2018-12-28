using System;
using System.Drawing;
using System.Windows.Forms;

namespace WROSimulatorV2
{
    public class MiscItemControls
    {
        Form1 form;
        Robot robot;
        Control textBox;
        bool getDistanceMode = false;
        Button button;
        bool drawLine;
        object storedVal;
        Func<LabeledControl,Form1, object> storeVal;
        Action<LabeledControl, Form1, object> cancel;
        Action<LabeledControl, Form1, PointF> update;
        LabeledControl parent;
        string text;
        public MiscItemControls(bool drawLine, Func<LabeledControl, Form1, object> storeVal, Action<LabeledControl, Form1, object> cancel, Action<LabeledControl, Form1, PointF> update)
        {
            this.drawLine = drawLine;
            this.storeVal = storeVal;
            this.cancel = cancel;
            this.update = update;
        }
        public Control GetInfoFromFieldButton(string text)
        {
            this.text = text;
            button = new Button();
            button.AutoSize = true;
            button.Text = text;
            button.Click += Button_Click;
            return button;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            parent = (LabeledControl)control.Parent;
            if (!getDistanceMode)
            {
                button.Text = "Cancel";
                getDistanceMode = true;
                textBox = parent.Control;
                storedVal = storeVal?.Invoke(parent, form);
                parent.Form.EnableMiscControls(false, null);
                Form1.ApplyToControlNodes(parent.Form.root, (c, o) =>
                {
                    c.Control.SetEnable(false, false, o, true);
                }, control);
                form = parent.Form;
                robot = new Robot(parent.Form.trasparentRobotImage);
                robot.Location = form.robot.Location;
                robot.Rotation = form.robot.Rotation;
                parent.Form.possibleCanvas.MouseMove += PossibleCanvas_MouseMove;
                parent.Form.possibleCanvas.MouseClick += PossibleCanvas_MouseClick;
            }
            else
            {
                StopGetDistance(true);
            }
        }
        void StopGetDistance(bool useStoredVal)
        {
            getDistanceMode = false;
            form.possibleCanvasGfx.Clear(Color.Transparent);
            button.Text =text;
            if (useStoredVal)
            {
                cancel?.Invoke(parent, form, storedVal);
            }
            form.EnableMiscControls(true, null);
            Form1.ApplyToControlNodes(form.root, (c, o) =>
            {
                c.Control.SetEnable(true, true);
            }, null);
            form.possibleCanvas.MouseMove -= PossibleCanvas_MouseMove;
            form.possibleCanvas.MouseClick -= PossibleCanvas_MouseClick;
        }

        private void PossibleCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            StopGetDistance(false);
        }

        private void PossibleCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;

            Point originalMousePos = Control.MousePosition.Subtract(form.FieldPictureBox.Location).Subtract(form.Location).Subtract(new Point(10,30));
            //PointF mousePos = control.PointToClient(e.Location);
            PointF mousePos = FieldAndRobotInfo.ToMillis(originalMousePos);
            robot.Location = mousePos;
            update(parent, form, mousePos);
            //textBox.Text = Math.Round(robot.Location.Distance(form.robot.Location), 2).ToString();
            form.possibleCanvasGfx.Clear(Color.Transparent);
            if (drawLine)
            {
                form.possibleCanvasGfx.DrawLine(new Pen(GetColorWithNewAlpha(Color.Magenta, form.possibleAlphaValue)), FieldAndRobotInfo.ToPixels(robot.Location), FieldAndRobotInfo.ToPixels(form.robot.Location));
            }
                robot.Draw(form.possibleCanvasGfx);
            //form.possibleCanvasGfx.FillRectangle(Brushes.Red, new RectangleF(originalMousePos.Subtract(new Point(3, 3)), new SizeF(6, 6)));
            //form.possibleCanvasGfx.FillRectangle(Brushes.Black, new RectangleF(FieldAndRobotInfo.ToPixels(robot.Location).Subtract(new PointF(3,3)), new SizeF(6, 6)));
        }
        Color GetColorWithNewAlpha(Color c, int alpha)
        {
            return Color.FromArgb(alpha, c.R, c.G, c.B);
        }

        #region DistanceFuncs
        public static object DistanceStoreVal(LabeledControl labeledControl, Form1 form)
        {
            return labeledControl.Control.Text;
        }
        public static void DistanceCancel(LabeledControl labeledControl, Form1 form, object storedVal)
        {
            labeledControl.Control.Text = (string)storedVal;
        }
        public static void DistanceUpdate(LabeledControl labeledControl, Form1 form, PointF mousePoint)
        {
            labeledControl.Control.Text = Math.Round(mousePoint.Distance(form.robot.Location), 2).ToString();
        }
        #endregion
        #region PositionFuncs
        public static object PositionStoreVal(LabeledControl labeledControl, Form1 form)
        {
            var v2 = (MyVector2)labeledControl.GetSetFunc.ObjGet(labeledControl.Index);
            return v2.Copy();
        }
        public static void PositionCancel(LabeledControl labeledControl, Form1 form, object storedVal)
        {
            var v2 = (MyVector2)storedVal;
            labeledControl.ControlNode.Children[0].Control.Control.Text = v2.X.ToString();
            labeledControl.ControlNode.Children[1].Control.Control.Text = v2.Y.ToString();
            //labeledControl.GetSetFunc.ObjSet(storedVal, labeledControl.Index);
        }
        public static void PositionUpdate(LabeledControl labeledControl, Form1 form, PointF mousePoint)
        {
            var v2 = new MyVector2(mousePoint.X, mousePoint.Y);
            //labeledControl.GetSetFunc.ObjSet(v2, labeledControl.Index);
            labeledControl.ControlNode.Children[0].Control.Control.Text = v2.X.ToString();
            labeledControl.ControlNode.Children[1].Control.Control.Text = v2.Y.ToString();
        }
        #endregion
    }
}
