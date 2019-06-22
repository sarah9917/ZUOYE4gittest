using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUOYE4
{
    class Matrix
    {
        private int numColumns = 0;
        private int numRows = 0;
        private double[] elements = null;//矩阵数据缓冲区
        public int Columns//列数
        {
            get
            {
                return numColumns;
            }
        }
        public int GetNumColumns()//获取列数
        {
            return numColumns;
        }
        public int Rows//行数
        {
            get
            {
                return numRows;
            }
        }
        public int GetNumRows()//获取列数
        {
            return numRows;
        }
        public double GetElement(int row, int col)//得到元素的值
        {
            return elements[col + row * numColumns];
        }
        public void SetElement(int row, int col, double val)
        {
            elements[col + row * numColumns] = val;
        }//给新矩阵赋值
        public double this[int row, int col]//索引器
        {
            get
            {
                return elements[col + row * numColumns];
            }
            set
            {
                elements[col + row * numColumns] = value;
            }
        }
        public Matrix()//构造函数
        {
            numColumns = 1;
            numRows = 1;
            Init(numRows, numColumns);
        }
        public Matrix(int row, int col)
        {
            numRows = row;
            numColumns = col;
            Init(numRows, numColumns);
        }//传递构造函数
        public bool Init(int nRows, int nCols)//分配空间
        {
            numRows = nRows;
            numColumns = nCols;
            int nSize = nRows * nCols;
            if (nSize < 0)
                return false;
            elements = new double[nSize];
            return true;
        }
        public Matrix Add(Matrix other)
        {
            if (numColumns != other.GetNumColumns() || numRows != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            Matrix result = new Matrix(this.GetNumRows(), this.GetNumColumns());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                    result.SetElement(i, j, this.GetElement(i, j) + other.GetElement(i, j));
            }
            return result;
        }//加法
        public static Matrix operator +(Matrix m1, Matrix m2)//加号重载
        {
            return m1.Add(m2);
        }
        public Matrix Substract(Matrix other)
        {
            if (numColumns != other.GetNumColumns() || numRows != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            Matrix result = new Matrix(this.GetNumRows(), this.GetNumColumns());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                    result.SetElement(i, j, this.GetElement(i, j) - other.GetElement(i, j));
            }
            return result;
        }//减法
        public static Matrix operator -(Matrix m1, Matrix m2)//减号重载
        {
            return m1.Substract(m2);
        }
        public Matrix Multiply(double value)
        {
            Matrix result = new Matrix(this.GetNumRows(), this.GetNumColumns());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                    result.SetElement(i, j, value * GetElement(i, j));
            }
            return result;
        }//数乘矩阵
        public static Matrix operator *(Matrix m1, double value)//数乘矩阵符号重载
        {
            return m1.Multiply(value);
        }
        public Matrix Multiply(Matrix other)
        {
            if (numColumns != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            Matrix result = new Matrix(numRows, other.GetNumColumns());
            double value;
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < other.GetNumColumns(); j++)
                {
                    value = 0.0;
                    for (int k = 0; k < numColumns; k++)
                    {
                        value += GetElement(i, k) * other.GetElement(k, j);
                    }
                    result.SetElement(i, j, value);
                }
            }
            return result;
        }//矩阵乘矩阵
        public static Matrix operator *(Matrix m1, Matrix m2)//矩阵乘矩阵重载
        {
            return m1.Multiply(m2);
        }
        public Matrix Transpose()
        {
            Matrix Trans = new Matrix(numColumns, numRows);
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                    Trans.SetElement(j, i, GetElement(i, j));
            }
            return Trans;
        }//转置矩阵
        public Matrix InvertGaussJordan()//求逆矩阵
        {
            Matrix Invert = new Matrix(numRows, numColumns);
            int i, j, k, l, u, v;
            double d = 0, p = 0;
            //分配内存
            int[] pnRow = new int[numColumns];
            int[] pnCol = new int[numColumns];
            //消元
            for (k = 0; k <= numColumns - 1; k++)
            {
                d = 0.0;
                for (i = k; i <= numColumns - 1; i++)
                {
                    for (j = k; j <= numColumns - 1; j++)
                    {
                        l = i * numColumns + j; p = Math.Abs(elements[l]);
                        if (p > d)
                        {
                            d = p;
                            pnRow[k] = i;
                            pnCol[k] = j;
                        }
                    }
                }

                //失败
                if (d == 0.0)
                {
                    throw new Exception("该矩阵不存在逆矩阵");
                }

                if (pnRow[k] != k)
                {
                    for (j = 0; j <= numColumns - 1; j++)
                    {
                        u = k * numColumns + j;
                        v = pnRow[k] * numColumns + j;
                        p = elements[u];
                        elements[u] = elements[v];
                        elements[v] = p;
                    }
                }

                if (pnCol[k] != k)
                {
                    for (i = 0; i <= numColumns - 1; i++)
                    {
                        u = i * numColumns + k;
                        v = i * numColumns + pnCol[k];
                        p = elements[u];
                        elements[u] = elements[v];
                        elements[v] = p;
                    }
                }

                l = k * numColumns + k;
                elements[l] = 1.0 / elements[l];
                for (j = 0; j <= numColumns - 1; j++)
                {
                    if (j != k)
                    {
                        u = k * numColumns + j;
                        elements[u] = elements[u] * elements[l];
                    }
                }

                for (i = 0; i <= numColumns - 1; i++)
                {
                    if (i != k)
                    {
                        for (j = 0; j <= numColumns - 1; j++)
                        {
                            if (j != k)
                            {
                                u = i * numColumns + j;
                                elements[u] = elements[u] - elements[i * numColumns + k]
                                    * elements[k * numColumns + j];
                            }
                        }
                    }
                }

                for (i = 0; i <= numColumns - 1; i++)
                {
                    if (i != k)
                    {
                        u = i * numColumns + k;
                        elements[u] = -elements[u] * elements[l];
                    }
                }
            }

            //调整恢复行列次序
            for (k = numColumns - 1; k >= 0; k--)
            {
                if (pnCol[k] != k)
                {
                    for (j = 0; j <= numColumns - 1; j++)
                    {
                        u = k * numColumns + j;
                        v = pnCol[k] * numColumns + j;
                        p = elements[u];
                        elements[u] = elements[v];
                        elements[v] = p;
                    }
                }


                if (pnRow[k] != k)
                {
                    for (i = 0; i <= numColumns - 1; i++)
                    {
                        u = i * numColumns + k;
                        v = i * numColumns + pnRow[k];
                        p = elements[u];
                        elements[u] = elements[v];
                        elements[v] = p;
                    }
                }
            }

            for (i = 0; i <= numRows - 1; i++)
            {
                for (j = 0; j <= numColumns - 1; j++)
                {
                    Invert[i, j] = elements[j + i * numColumns];
                }

            }

            return Invert;
        }
    }
}
