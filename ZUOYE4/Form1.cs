using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ZUOYE4
{
    struct WaiPoint   //外方位元素 两组
    {
        public string Name;
        public double Xs;  //m
        public double Ys;  //m
        public double Zs;  //m
        public double fai;  //du
        public double womig; //du
        public double k;   //du
    }
    struct R  //结构体  旋转矩阵a1,b1,c1 两组
    {
        public double a1, a2, a3, b1, b2, b3, c1, c2, c3;
    }


    struct TongminD   //六对框标坐标
    {
        public string Name;
        public double zuox;
        public double zuoy;
        public double youx;
        public double youy;
    }
    struct Kongjian  //像空间坐标系
    {
        public double X1;
        public double Y1;
        public double Z1;
        public double X2;
        public double Y2;
        public double Z2;
    }
    struct Dxyz   //地面坐标
    {
        public string name;
        public double X;
        public double Y;
        public double Z;
    }

    struct Ajuzheng
    {
        public double l11, l12, l13, l14, l15, l16;
        public double l21, l22, l23, l24, l25, l26;
    }
    struct Ashu
    {
        public double l1x, l1y;
        public double l2x, l2y;
    }
       public partial class Form1 : Form
       {
        public Form1()
        {
            InitializeComponent();
            m_ptlst = new List<WaiPoint>();   //外方位元素
            tongmin = new List<TongminD>();   //同名元素
            RZHUAN = new List<R>();    //旋转矩阵  a1,b1,c1
            kongjians = new List<Kongjian>();   //像空间坐标
            dixyz = new List<Dxyz>();   //地面点坐标
            Aa = new List<Ajuzheng>();   //严密计算的系数矩阵
            ap = new List<Ashu>();   //严密计算的L部分
        }
        private List<WaiPoint> m_ptlst;    //外方位   两组
        private List<TongminD> tongmin;    //同名像点框标坐标  六对
        private List<R> RZHUAN;    //三个计算值链表   旋转矩阵  两组
        private List<Kongjian> kongjians;   //像空间辅助坐标
        private List<Dxyz> dixyz;
        private List<Ajuzheng> Aa;
        private List<Ashu> ap;
        public List<double[]> ToList(string[] str, out int row, out int column)
        {
            List<double[]> rowList = new List<double[]>();
            column = -1;//列数
            row = -1;//行数
            for (int i = 0; i < str.Length; i++)
            {
                double[] Row = str[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select<string, double>(item => Convert.ToDouble(item)).ToArray();
                rowList.Add(Row);
                if (i == 0)
                {
                    column = Row.Length;
                }//只执行一次获得列数
            }
            row = str.Length;
            return rowList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(txt)|*.txt|所有文件(*.*)|*.*";
            string sepatator = ",";
            char[] cgap = sepatator.ToCharArray();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                StreamReader sr1 = new StreamReader(fs);
                if (m_ptlst != null || tongmin != null || RZHUAN != null)
                {
                    m_ptlst.Clear();   //清空List中的元素
                    tongmin.Clear();
                    RZHUAN.Clear();
                }
                for (int i = 0; i < 9999; i++)
                {
                    string str1 = sr1.ReadLine();
                    if (str1 == null)
                        break;
                    string[] str2 = str1.Split(cgap);
                    ListViewItem b = new ListViewItem(str2[0]);
                    b.SubItems.Add(str2[1]);
                    b.SubItems.Add(str2[2]);
                    b.SubItems.Add(str2[3]);
                    b.SubItems.Add(str2[4]);
                    b.SubItems.Add(str2[5]);
                    b.SubItems.Add(str2[6]);
                    listView1.BeginUpdate();
                    listView1.Items.Add(b);
                    listView1.EndUpdate();
                    WaiPoint point = new WaiPoint();   //外方位元素链表
                    point.Name = str2[0];       //像片位置
                    point.Xs = double.Parse(str2[1]); //Xs
                    point.Ys = double.Parse(str2[2]); //Ys
                    point.Zs = double.Parse(str2[3]);  //Zs
                    point.fai = double.Parse(str2[4]);   //角元素fai
                    point.womig = double.Parse(str2[5]);  //角元素womig
                    point.k = double.Parse(str2[6]);  //角元素k
                    m_ptlst.Add(point);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(txt)|*.txt|所有文件(*.*)|*.*";
            string sepatator = ",";
            char[] cgap = sepatator.ToCharArray();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                StreamReader sr1 = new StreamReader(fs, Encoding.ASCII);
                for (int i = 0; i < 9999; i++)
                {
                    string str1 = sr1.ReadLine();
                    if (str1 == null)
                        break;
                    string[] str2 = str1.Split(cgap, StringSplitOptions.None);
                    ListViewItem b = new ListViewItem((listView2.Items.Count + 1).ToString());
                    b.SubItems.Add(str2[0]);
                    b.SubItems.Add(str2[1]);
                    b.SubItems.Add(str2[2]);
                    b.SubItems.Add(str2[3]);
                    listView2.BeginUpdate();
                    listView2.Items.Add(b);
                    listView2.EndUpdate();
                    TongminD compoint = new TongminD(); 
                    compoint.Name = listView2.Items.Count+1.ToString ();       //点名
                    compoint.zuox = double.Parse(str2[0]); //左边的x
                    compoint.zuoy = double.Parse(str2[1]);    //左边的y
                    compoint.youx = double.Parse(str2[2]);  //右边的x
                    compoint.youy = double.Parse(str2[3]);   //右边的y
                    tongmin.Add(compoint);
                }
            }
        }
        private void Rjisuan(double fai, double womi, double k)   //R旋转矩阵的值  没有单位  //两个R1,R2
        {
            R R1 = new R();
            R1.a1 = Math.Cos(fai) * Math.Cos(k) - Math.Sin(fai) * Math.Sin(womi) * Math.Sin(k);
            R1.a2 = (-1) * Math.Cos(fai) * Math.Sin(k) - Math.Sin(fai) * Math.Sin(womi) * Math.Cos(k);
            R1.a3 = (-1) * Math.Sin(fai) * Math.Cos(womi);
            R1.b1 = Math.Cos(womi) * Math.Sin(k);
            R1.b2 = Math.Cos(womi) * Math.Cos(k);
            R1.b3 = (-1) * Math.Sin(womi);
            R1.c1 = Math.Sin(fai) * Math.Cos(k) + Math.Cos(fai) * Math.Sin(womi) * Math.Sin(k);
            R1.c2 = (-1) * Math.Sin(fai) * Math.Sin(k) + Math.Cos(fai) * Math.Sin(womi) * Math.Cos(k);
            R1.c3 = Math.Cos(fai) * Math.Cos(womi);
            RZHUAN.Add(R1);
        }
        private void Xkongjianfuzhu(double x0, double y0, double f)
        {
            for (int i = 0; i < m_ptlst.Count; i++)   //循环外方位元素
            {
                Rjisuan(m_ptlst[i].fai * Math.PI / 180, m_ptlst[i].womig * Math.PI / 180, m_ptlst[i].k * Math.PI / 180);   //转换矩阵R a1,b1,c1 
            }
            Kongjian kong = new Kongjian();
            for (int i = 0; i < tongmin.Count; i++)   //六个框标坐标  转换为像平面坐标   转换成m为单位  
            {
                kong.X1 = (RZHUAN[0].a1 * (tongmin[i].zuox - x0) + RZHUAN[0].a2 * (tongmin[i].zuoy - y0) + RZHUAN[0].a3 * (-1) * f) * 0.001;   //左边的三个像空间辅助坐标
                kong.Y1 = (RZHUAN[0].b1 * (tongmin[i].zuox - x0) + RZHUAN[0].b2 * (tongmin[i].zuoy - y0) + RZHUAN[0].b3 * (-1) * f) * 0.001;
                kong.Z1 = (RZHUAN[0].c1 * (tongmin[i].zuox - x0) + RZHUAN[0].c2 * (tongmin[i].zuoy - y0) + RZHUAN[0].c3 * (-1) * f) * 0.001;

                kong.X2 = (RZHUAN[1].a1 * (tongmin[i].youx - x0) + RZHUAN[1].a2 * (tongmin[i].youy - y0) + RZHUAN[1].a3 * (-1) * f) * 0.001;    //右边的三个像空间辅助坐标
                kong.Y2 = (RZHUAN[1].b1 * (tongmin[i].youx - x0) + RZHUAN[1].b2 * (tongmin[i].youy - y0) + RZHUAN[1].b3 * (-1) * f) * 0.001;
                kong.Z2 = (RZHUAN[1].c1 * (tongmin[i].youx - x0) + RZHUAN[1].c2 * (tongmin[i].youy - y0) + RZHUAN[1].c3 * (-1) * f) * 0.001;
                kongjians.Add(kong);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            double Bx = m_ptlst[1].Xs - m_ptlst[0].Xs;   //三个基线分量
            double By = m_ptlst[1].Ys - m_ptlst[0].Ys;
            double Bz = m_ptlst[1].Zs - m_ptlst[0].Zs;
            double N1, N2;

            string sx3 = textBox1.Text;
            double x0 = double.Parse(sx3);  //输入x0主距 mm 
            string sx2 = textBox2.Text;
            double y0 = double.Parse(sx2);  //输入y0主距 mm 
            string sx1 = textBox3.Text;
            double f = double.Parse(sx1);  //输入f主距 mm 

            Xkongjianfuzhu(x0, y0, f);

            Dxyz di = new Dxyz();
            for (int i = 0; i < kongjians.Count; i++)
            {
                N1 = (Bx * kongjians[i].Z2 - Bz * kongjians[i].X2) / (kongjians[i].X1 * kongjians[i].Z2 - kongjians[i].X2 * kongjians[i].Z1);
                N2 = (Bx * kongjians[i].Z1 - Bz * kongjians[i].X1) / (kongjians[i].X1 * kongjians[i].Z2 - kongjians[i].X2 * kongjians[i].Z1);
                di.name = tongmin[i].Name;
                di.X = m_ptlst[0].Xs + N1 * kongjians[i].X1;
                di.Y = 1.0 / 2 * (m_ptlst[0].Ys + N1 * kongjians[i].Y1 + m_ptlst[1].Ys + N2 * kongjians[i].Y2);
                di.Z = m_ptlst[0].Zs + N1 * kongjians[i].Z1;
                dixyz.Add(di);
            }

            foreach (Dxyz dii in dixyz)
            {
                listView3.Items.Add(new ListViewItem(new string[] { dii.name, dii.X.ToString("#0.00000000"), dii.Y.ToString("#0.00000000"), dii.Z.ToString("#0.00000000") }));
            }
            
            Ajuzheng A = new Ajuzheng();
            Ashu s = new Ashu();
            for (int i = 0; i < tongmin.Count; i++)   //六个同名像点
            {
                A.l11 = (f * RZHUAN[0].a1 + (tongmin[i].zuox - x0) * RZHUAN[0].a3) * 0.001;
                A.l12 = (f * RZHUAN[0].b1 + (tongmin[i].zuox - x0) * RZHUAN[0].b3) * 0.001;
                A.l13 = (f * RZHUAN[0].c1 + (tongmin[i].zuox - x0) * RZHUAN[0].c3) * 0.001;
                s.l1x = (f * RZHUAN[0].a1 * m_ptlst[0].Xs + f * RZHUAN[0].b1 * m_ptlst[0].Ys + f * RZHUAN[0].c1 * m_ptlst[0].Zs + (tongmin[i].zuox - x0) * RZHUAN[0].a3 * m_ptlst[0].Xs + (tongmin[i].zuox - x0) * RZHUAN[0].b3 * m_ptlst[0].Ys + (tongmin[i].zuox - x0) * RZHUAN[0].c3 * m_ptlst[0].Zs) * 0.001;
                A.l14 = (f * RZHUAN[0].a2 + (tongmin[i].zuoy - y0) * RZHUAN[0].a3) * 0.001;
                A.l15 = (f * RZHUAN[0].b2 + (tongmin[i].zuoy - y0) * RZHUAN[0].b3) * 0.001;
                A.l16 = (f * RZHUAN[0].c2 + (tongmin[i].zuoy - y0) * RZHUAN[0].c3) * 0.001;
                s.l1y = (f * RZHUAN[0].a2 * m_ptlst[0].Xs + f * RZHUAN[0].b2 * m_ptlst[0].Ys + f * RZHUAN[0].c2 * m_ptlst[0].Zs + (tongmin[i].zuoy - y0) * RZHUAN[0].a3 * m_ptlst[0].Xs + (tongmin[i].zuoy - y0) * RZHUAN[0].b3 * m_ptlst[0].Ys + (tongmin[i].zuoy - y0) * RZHUAN[0].c3 * m_ptlst[0].Zs) * 0.001;

                A.l21 = (f * RZHUAN[1].a1 + (tongmin[i].youx - x0) * RZHUAN[1].a3) * 0.001;
                A.l22 = (f * RZHUAN[1].b1 + (tongmin[i].youx - x0) * RZHUAN[1].b3) * 0.001;
                A.l23 = (f * RZHUAN[1].c1 + (tongmin[i].youx - x0) * RZHUAN[1].c3) * 0.001;
                s.l2x = (f * RZHUAN[1].a1 * m_ptlst[1].Xs + f * RZHUAN[1].b1 * m_ptlst[1].Ys + f * RZHUAN[1].c1 * m_ptlst[1].Zs + (tongmin[i].youx - x0) * RZHUAN[1].a3 * m_ptlst[1].Xs + (tongmin[i].youx - x0) * RZHUAN[1].b3 * m_ptlst[1].Ys + (tongmin[i].youx - x0) * RZHUAN[1].c3 * m_ptlst[1].Zs) * 0.001;
                A.l24 = (f * RZHUAN[1].a2 + (tongmin[i].youy - y0) * RZHUAN[1].a3) * 0.001;
                A.l25 = (f * RZHUAN[1].b2 + (tongmin[i].youy - y0) * RZHUAN[1].b3) * 0.001;
                A.l26 = (f * RZHUAN[1].c2 + (tongmin[i].youy - y0) * RZHUAN[1].c3) * 0.001;
                s.l2y = (f * RZHUAN[1].a2 * m_ptlst[1].Xs + f * RZHUAN[1].b2 * m_ptlst[1].Ys + f * RZHUAN[1].c2 * m_ptlst[1].Zs + (tongmin[i].youy - y0) * RZHUAN[1].a3 * m_ptlst[1].Xs + (tongmin[i].youy - y0) * RZHUAN[1].b3 * m_ptlst[1].Ys + (tongmin[i].youy - y0) * RZHUAN[1].c3 * m_ptlst[1].Zs) * 0.001;

                Aa.Add(A);
                ap.Add(s);
            }
            dixyz.Clear();   //清空
            for (int i = 0; i < tongmin.Count; i++)    //输出系数矩阵A
            {
                textBox4.Text = textBox4.Text + Aa[i].l11.ToString() + " " + Aa[i].l12.ToString() + " " + Aa[i].l13.ToString() + '\r' + '\n';
                textBox4.Text = textBox4.Text + Aa[i].l14.ToString() + " " + Aa[i].l15.ToString() + " " + Aa[i].l16.ToString() + '\r' + '\n'; ;
                textBox4.Text = textBox4.Text + Aa[i].l21.ToString() + " " + Aa[i].l22.ToString() + " " + Aa[i].l23.ToString() + '\r' + '\n';
                textBox4.Text = textBox4.Text + Aa[i].l24.ToString() + " " + Aa[i].l25.ToString() + " " + Aa[i].l26.ToString();

                textBox6.Text = textBox6.Text + ap[i].l1x.ToString() + '\r' + '\n';
                textBox6.Text = textBox6.Text + ap[i].l1y.ToString() + '\r' + '\n';
                textBox6.Text = textBox6.Text + ap[i].l2x.ToString() + '\r' + '\n';
                textBox6.Text = textBox6.Text + ap[i].l2y.ToString();

                string[] str1 = textBox4.Lines;   //系数矩阵A
                int rowA1, columnA1;
                List<double[]> rowListA1 = ToList(str1, out rowA1, out columnA1);
                Matrix A1 = new Matrix(rowA1, columnA1);
                for (int i3 = 0; i3 < rowA1; i3++)
                {
                    for (int j = 0; j < columnA1; j++)
                    {
                        A1[i3, j] = rowListA1[i3][j];
                    }
                }


                string[] str2 = textBox6.Lines;   //L=x-x0
                int rowL, columnL;
                List<double[]> rowListL = ToList(str2, out rowL, out columnL);
                Matrix L = new Matrix(rowL, columnL);     //B矩阵是差值矩阵
                for (int i1 = 0; i1 < rowL; i1++)
                {
                    for (int j = 0; j < columnL; j++)
                    {
                        L[i1, j] = rowListL[i1][j];
                    }
                }

                Matrix C = A1.Transpose();   //C矩阵是A矩阵的转置
                Matrix D = C * A1;   //D是aT*a
                Matrix E = D.InvertGaussJordan();  //aTa的求逆
                Matrix F = C * L;   //D是aT*L
                Matrix x = E * F;   //D是aTa求逆*(aT*L)
                Dxyz Si = new Dxyz();   //严密计算求值
                StringBuilder ss18 = new StringBuilder();
                for (int i2 = 0; i2 < x.GetNumRows(); i2++)
                {
                    for (int j = 0; j < x.GetNumColumns(); j++)
                    {
                        ss18.Append(x[i2, j] + " ");//ss加入所有元素
                        Si.name = (i + 1).ToString();
                        if (i2 == 0) Si.X = x[i2, j];
                        if (i2 == 1) Si.Y = x[i2, j];
                        if (i2 == 2) Si.Z = x[i2, j];
                        if (i2 == 2) dixyz.Add(Si);
                    }
                    ss18 = new StringBuilder();//清空
                   textBox6.Clear();
                   textBox4.Clear();
                }

            }
            foreach (Dxyz dii in dixyz)
            {
                listView4.Items.Add(new ListViewItem(new string[] { dii.name, dii.X.ToString("#0.00000000"), dii.Y.ToString("#0.00000000"), dii.Z.ToString("#0.00000000") }));
            }
        }
    }

}
