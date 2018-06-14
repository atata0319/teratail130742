using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MouseLeaveTest2
{
    public partial class Form1 : Form, IMessageFilter
    {
        private bool flag = false;
        private Point previous;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowPanel();
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            flag = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            if (!panel1.ClientRectangle.Contains(panel1.PointToClient(Cursor.Position)))
            {
                HidePanel();
            }
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            const int WM_MOUSEMOVE = 0x0200;
            const int WM_NCMOUSEMOVE = 0x00A0;
            if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_NCMOUSEMOVE)
            {
                Point current;
                if (m.Msg == WM_NCMOUSEMOVE)
                {
                    current = panel1.PointToClient(new Point(m.LParam.ToInt32()));
                }
                else
                {
                    var control = Control.FromHandle(m.HWnd);
                    if (control != null)
                        current = panel1.PointToClient(control.PointToScreen(new Point(m.LParam.ToInt32())));
                    else
                        current = panel1.PointToClient(Cursor.Position);
                }
                //Debug.Print("WM_MOUSEMOVE: {0}", current);
                if (panel1.ClientRectangle.Contains(current))
                {
                    flag = true;
                }
                else
                {
                    if (flag)
                    {
                        HidePanel();
                    }
                    else
                    {
                        // 直前の座標と現在の座標を結ぶ線分がパネルの矩形と交差する場合、パネルを閉じる。
                        if (IsIntersected(panel1.ClientRectangle, current, previous))
                        {
                            HidePanel();
                        }
                    }
                }
                previous = current;
            }
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var current = panel1.PointToClient(Cursor.Position);
            // フォーム内から一気にフォーム外に出ていったカーソルはここで処理する。
            if (IsIntersected(panel1.ClientRectangle, current, previous))
            {
                HidePanel();
            }
            previous = current;
        }

        private void ShowPanel()
        {
            panel1.Visible = true;
            timer1.Start();
            Application.AddMessageFilter(this);
            previous = panel1.PointToClient(Cursor.Position);
        }

        private void HidePanel()
        {
            flag = false;
            timer1.Stop();
            Application.RemoveMessageFilter(this);
            panel1.Visible = false;
        }

        private static bool IsIntersected(Rectangle rectangle, Point a, Point b)
        {
            // 矩形と線分の交差は各辺と交差しているかどうかで判定する。
            var lefttop = new Point(rectangle.Left, rectangle.Top); // 左上座標
            var righttop = new Point(rectangle.Right, rectangle.Top); // 右上座標
            var leftbottom = new Point(rectangle.Left, rectangle.Bottom); // 左下座標
            var rightbottom = new Point(rectangle.Right, rectangle.Bottom); // 右下座標
            if (IsIntersected(a, b, lefttop, leftbottom)) // 左辺
                return true;
            if (IsIntersected(a, b, righttop, rightbottom)) // 右辺
                return true;
            if (IsIntersected(a, b, leftbottom, rightbottom)) // 下辺
                return true;
            if (IsIntersected(a, b, lefttop, righttop)) // 上辺
                return true;
            return false;
        }

        private static bool IsIntersected(Point a, Point b, Point c, Point d)
        {
            // https://qiita.com/ykob/items/ab7f30c43a0ed52d16f2
            // 線分(ab)と線分(cd)が交差しているか判定する。
            var ta = (c.X - d.X) * (a.Y - c.Y) + (c.Y - d.Y) * (c.X - a.X);
            var tb = (c.X - d.X) * (b.Y - c.Y) + (c.Y - d.Y) * (c.X - b.X);
            var tc = (a.X - b.X) * (c.Y - a.Y) + (a.Y - b.Y) * (a.X - c.X);
            var td = (a.X - b.X) * (d.Y - a.Y) + (a.Y - b.Y) * (a.X - d.X);
            return tc * td < 0 && ta * tb < 0;
        }
    }
}
