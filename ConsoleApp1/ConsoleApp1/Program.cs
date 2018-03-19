//#define Defined_Dense_Matrix_is_online
//#define Defined_dense_LU_matrix_is_online
#define Defined_sparse_LU_matrix_is_online
//#define Defined_sparse_MSG_is_online
//#define Defined_sparse_MSG
//#define Debug
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;//files
using System.Windows.Forms;
using System.Drawing;
namespace slae_project
{
    public class Program
    {
        static bool Show_first_three_elements_from_vectors_of_answers = true;

        public class data_for_main_work : data_other
        {
            static public double[] X, Y;
        }
        class Finite_Element_Method : data_for_main_work
        {
            Interface I = new Interface();
            public Finite_Element_Method()
            {
#if (Debug)
                debug = true;
#endif
                I.Greetings();
                Sub_Main();
                I.SaySomeQuote();
                //I.Pause();
            }
            public Finite_Element_Method(int X_count_next, int Y_count_next, int new_order_of_file)
            {
                X_count = X_count_next;
                Y_count = Y_count_next;
                order_of_file = new_order_of_file;

#if (Debug)
                debug = true;
#endif
                I.Greetings();
                Sub_Main();
                I.SaySomeQuote();
                //I.Pause();
            }
            public static int order_of_file = 1;
            static bool boundary_left_1_type = true;
            static bool boundary_right_1_type = true;
            static bool boundary_bottom_1_type = true;
            static bool boundary_top_1_type = true;
            static int binary_count = 4;
            public static int X_count = System.Convert.ToInt32(Math.Pow(2.0, binary_count)) + 1;
            public static int Y_count = 3;
            int Maxiter = 10000;
            double E = 1e-20;

            double U_analit(double x, double y) { return x + y; }
            double f(double x, double y)
            {
                return Gamma * (x + y);
                //return 0;
            }
            public class Sreda
            {
                public int Quanitity_of_areas = 0;

                public string ProjectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

                class SmallArea
                {
                    //Границы области
                    public double x0, x1, y0, y1;

                    //Магнитопрониц, сила тока, номер материала.
                    public double muo, j, nmat;
                    public SmallArea(double _x0, double _x1, double _y0, double _y1, double _muo, double _j, double _nmat)
                    {
                        x0 = _x0;
                        x1 = _x1;
                        y0 = _y0;
                        y1 = _y1;
                        muo = _muo;
                        j = _j;
                        nmat = _nmat;
                    }
                }
                List<SmallArea> areas = new List<SmallArea>();
                /*
                KolObl
                x0[1]       x1[1] y0[1]       y1[1] muo[1]       j[1] nmat[1]
                x0[2] x1[2]       y0[2] y1[2]       muo[2] j[2]       nmat[2]
                ...
                x0[KolObl] x1[KolObl]  y0[KolObl] y1[KolObl]  muo[KolObl] j[KolObl]  nmat[KolObl]

                x[1] KolX
                x[2] x[3] ... x[KolX]
                hXm[1]  hXm[2]  ...  hXm[KolX]
                dhX[1]  dhX[2]  ...  dhX[KolX]
                shX[1]  shX[2]  ...  shX[KolX]

                y[1] KolY
                y[2] y[3] ... y[KolY]
                hYm[1]  hYm[2]  ...  hYm[KolY]
                dhY[1]  dhY[2]  ...  dhY[KolY]
                shY[1]  shY[2]  ...  shY[KolY]

                DoubleToX DoubleToY*/

                public Sreda()
                {

                }
                public string StringExtraSpaceRemover(string NeedToBeSplitted)
                {
                    string NewNeed = NeedToBeSplitted;
                    string OldNeed = " ";
                    while (NewNeed.CompareTo(OldNeed) != 0)
                    {
                        OldNeed = NewNeed;
                        NewNeed = NewNeed.Replace("  ", " ");
                    }

                    return NewNeed;
                }
                public string[] StringSplitter(string SplitTarget)
                {
                    string NeedToBeSplitted = SplitTarget;
                    NeedToBeSplitted = NeedToBeSplitted.Replace('.', System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0]);
                    NeedToBeSplitted = NeedToBeSplitted.Replace(',', System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0]);
                    NeedToBeSplitted = NeedToBeSplitted.Replace('\t', ' ');
                    var Splitted = StringExtraSpaceRemover(NeedToBeSplitted).Split(' ');
                    return Splitted;
                }
                public void ReadAxe(StreamReader inputFile, ref Axe TargetAxe)
                {

                    string[] Splitted = StringSplitter(inputFile.ReadLine());
                    TargetAxe.StartPoint = System.Convert.ToDouble(Splitted[0]);
                    TargetAxe.Quantity = System.Convert.ToInt32(Splitted[1]);

                    string[] SplittedX = StringSplitter(inputFile.ReadLine());
                    string[] SplittedStep = StringSplitter(inputFile.ReadLine());
                    string[] SplittedRazryadka = StringSplitter(inputFile.ReadLine());
                    string[] SplittedForwardOrBacward = StringSplitter(inputFile.ReadLine());
                    for (int i = 0, end = TargetAxe.Quantity; i < end; i++)
                    {
                        TargetAxe.x.Add(System.Convert.ToDouble(SplittedX[i]));
                        TargetAxe.step_init.Add(System.Convert.ToDouble(SplittedStep[i]));
                        TargetAxe.koef_of_razryadka.Add(System.Convert.ToDouble(SplittedRazryadka[i]));

                        if (SplittedForwardOrBacward[i] == "1")
                            TargetAxe.ItsForwardOrBackward.Add(true);
                        else TargetAxe.ItsForwardOrBackward.Add(false);
                    }

                }
                public class Axe
                {
                    public double StartPoint;
                    public int Quantity;
                    public List<double> x = new List<double>();
                    public List<double> step_init = new List<double>();
                    public List<double> koef_of_razryadka = new List<double>();
                    public List<bool> ItsForwardOrBackward = new List<bool>();

                    //Дробление сетки
                    public int DoubleToAxe;
                }
                public Axe AxeX = new Axe(), AxeY = new Axe();
                public void reading_sreda()
                {
                    string Path = ProjectPath + "\\sreda";
                    using (StreamReader inputFile = new StreamReader(Path))
                    {
                        //for (int i = 0; i < Y.Count(); i++)
                        Quanitity_of_areas = System.Convert.ToInt32(inputFile.ReadLine());

                        //Считаем области из файла.
                        for (int i = 0; i < Quanitity_of_areas; i++)
                        {
                            string[] Splitted = StringSplitter(inputFile.ReadLine());

                            areas.Add(new SmallArea(
                                System.Convert.ToDouble(Splitted[0]),
                                System.Convert.ToDouble(Splitted[1]),
                                System.Convert.ToDouble(Splitted[2]),
                                System.Convert.ToDouble(Splitted[3]),
                                System.Convert.ToDouble(Splitted[4]),
                                System.Convert.ToDouble(Splitted[5]),
                                System.Convert.ToDouble(Splitted[6])
                                ));
                        }


                        //Настало время Осей.
                        ReadAxe(inputFile, ref AxeX);
                        ReadAxe(inputFile, ref AxeY);
                        {
                            string[] Splitted = StringSplitter(inputFile.ReadLine());
                            AxeX.DoubleToAxe = System.Convert.ToInt32(Splitted[0]);
                            AxeY.DoubleToAxe = System.Convert.ToInt32(Splitted[1]);
                        }
                        Console.WriteLine("Sreda has been read.");
                    }
                }
            }
            public Sreda areas = new Sreda();
            List<double> AxeGenerating(ref Sreda.Axe TargetAxe)
            {
                //Где будет сгенерирована Ось.
                List<double> Temp = new List<double>();

                //Добавили начальную точку
                Temp.Add(TargetAxe.StartPoint);

                //Добавили главные узлы
                foreach (var elem in TargetAxe.x)
                    Temp.Add(elem);

                //Генерируем то что между главных узлов(нелинейную фигню)
                List<List<double>> Between = new List<List<double>>();
                for (int i = 0; i < TargetAxe.Quantity; i++)
                {
                    Between.Add(new List<double>());
                    double start, end, step;
                    if (TargetAxe.ItsForwardOrBackward[i])
                    {
                        start = Temp[i]; end = Temp[i + 1]; step = TargetAxe.step_init[i];
                    }
                    else
                    {
                        start = Temp[i + 1]; end = Temp[i]; step = -TargetAxe.step_init[i];
                    }

                    while ((start + step < end && TargetAxe.ItsForwardOrBackward[i] == true)
                        || start + step > end && TargetAxe.ItsForwardOrBackward[i] == false)
                    {
                        if (TargetAxe.ItsForwardOrBackward[i] || Between[i].Count == 0)
                            Between[i].Add(start + step);
                        else Between[i].Insert(0, start + step);

                        start += step;
                        step *= TargetAxe.koef_of_razryadka[i];
                    }
                }

                //вставить то что между главным узлами.
                int counter = 1;
                foreach (var Range in Between)
                {
                    foreach (var number in Range)
                    {
                        Temp.Insert(counter, number);
                        counter++;
                    }
                    counter++;
                }

                //А теперь еще надо подробить сетку...
                int Fractionization = Convert.ToInt32(Math.Pow(2, TargetAxe.DoubleToAxe));
                List<double> TempFractioned = new List<double>();
                for (int i = 0; i < Temp.Count()-1; i++)
                {
                    TempFractioned.Add(Temp[i]);
                    double step = (Temp[i + 1] - Temp[i]) / ((double)Fractionization);
                    for (int j = 1; j < Fractionization; j++)
                        TempFractioned.Add(Temp[i] + step*j);
                }
                TempFractioned.Add(Temp[Temp.Count() - 1]);
                Temp = TempFractioned;
                
                //Проверка что каждый следующий элемент больше предыдущего.
                for (int i = 0; i < Temp.Count() - 1; i++)
                {
                    if (Temp[i + 1] < Temp[i]) { Console.WriteLine("Error: if (Temp[i + 1] < Temp[i])..."); Console.ReadKey(); }

                }
                TargetAxe.Quantity = Temp.Count();
                return Temp;
            }
            void generating_OX_OY_lyambda_gamma()
            {
                data_for_generating D = new data_for_generating(1, 1);//(double Lyambda1, double Gamma1)
                Console.WriteLine("double Lyambda1 = {0}, double Gamma1 = {1}", Lyambda, Gamma);

                using (StreamWriter outputFile = new StreamWriter("dd84ai_RGR_intput_OX.txt"))
                {
                    var Temp = AxeGenerating(ref areas.AxeX);
                    foreach (var item in r_x) outputFile.WriteLine("{0} ", item[0]);
                }
                using (StreamWriter outputFile = new StreamWriter("dd84ai_RGR_input_OY.txt"))
                {
                    var Temp = AxeGenerating(ref areas.AxeY);
                    foreach (var item in r_y) outputFile.WriteLine("{0} ", item[0]);
                }
                using (StreamWriter outputFile = new StreamWriter("dd84ai_RGR_input_other_data.txt"))
                {
                    outputFile.WriteLine("{0} ", r_x.Count());
                    outputFile.WriteLine("{0} ", r_y.Count());
                    outputFile.WriteLine("{0} ", Lyambda);
                    outputFile.WriteLine("{0} ", Gamma);
                }
            }
            void Save_vector(double[] Target_vector, string fname)
            {
                using (StreamWriter outputFile = new StreamWriter(fname))
                {
                    Console.WriteLine("______________________________________________");
                    Console.WriteLine("______________________________________________");
                    int quality_total = 0, quality_saved = 0;
                    double test1, test2;
                    for (int i = 0; i < Target_vector.Count(); i++)
                    {
                        if (((test1 = Math.Abs(Math.Truncate(Target_vector[i]) - Target_vector[i])) < 0.001) ||
                    ((test2 = Math.Abs(Math.Abs(Math.Truncate(Target_vector[i]) - Target_vector[i]) - 1)) < 0.001))
                        {
                            outputFile.WriteLine("{0:E15}", Target_vector[i]);
                            //outputFile.WriteLine(Target_vector[i].ToString().Replace(',', '.'));
                            quality_saved++;
                        }
                        quality_total++;
                    }
                    Console.WriteLine("Total = {0}", quality_total);
                    Console.WriteLine("Saved = {0}", quality_saved);
                }

                //+full
                using (StreamWriter outputFile = new StreamWriter("full_" + fname))
                {
                    for (int i = 0; i < Target_vector.Count(); i++)
                        outputFile.WriteLine(Target_vector[i].ToString().Replace(',', '.'));
                }
            }
            int x_index;
            int y_index;
            double f_value;
            void internal_adjusting_of_boundaries(int j, int i, int value, bool diag_mod, bool F_mod)
            {
                if (F_mod)
                {
                    x_index = j % X.Count(); //x = 2;
                    y_index = j / X.Count(); //y = 1;
                    f_value = U_analit(X[x_index], Y[y_index]);
                }


                if (boundary_left_1_type)
                {
                    if (diag_mod) i = j;
                    if (j % X.Count() == 0)
                    {
                        A[j, i] = value;
                        if (F_mod) F[j] = f_value;
                    }
                }

                if (boundary_right_1_type)
                {
                    //int k = j + X.Count() - 1;
                    if (diag_mod) i = j;
                    if ((j + 1) % X.Count() == 0)
                    {

                        A[j, i] = value;
                        if (F_mod) F[j] = f_value;
                    }
                }

                if (boundary_bottom_1_type)
                {
                    if (diag_mod) i = j;
                    if (j < X.Count())
                    {
                        A[j, i] = value;
                        if (F_mod) F[j] = f_value;

                    }
                }

                if (boundary_top_1_type)
                {
                    int k = j + (Y.Count() - 1) * X.Count();
                    if (diag_mod) i = j;
                    if
                        (j > (Y.Count() - 1) * X.Count())
                    {
                        A[j, i] = value;
                        if (F_mod) F[j] = f_value;
                    }
                }
            }

            void A_F_adjusting_for_boundaries()
            {
                //Занулил нужные строки матрицы
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        internal_adjusting_of_boundaries(j, i, 0, false, false);
                    }
                }
                //Поставил единички на диагонялях и нужные значение F_mod.
                for (int i = 0; i < Size; i++)
                {
                    internal_adjusting_of_boundaries(i, i, 1, true, true);
                }
            }

            void reading_input_data()
            {
                using (StreamReader inputFile = new StreamReader("dd84ai_RGR_input_other_data.txt"))
                {
                    int ox_size = System.Convert.ToInt32(inputFile.ReadLine());
                    X = new double[ox_size]; X_count = ox_size;

                    int oy_size = System.Convert.ToInt32(inputFile.ReadLine());
                    Y = new double[oy_size]; Y_count = oy_size;

                    Lyambda = System.Convert.ToDouble(inputFile.ReadLine());

                    Gamma = System.Convert.ToDouble(inputFile.ReadLine());
                }
                using (StreamReader inputFile = new StreamReader("dd84ai_RGR_intput_OX.txt"))
                {
                    for (int i = 0; i < X.Count(); i++)
                    {
                        X[i] = System.Convert.ToDouble(inputFile.ReadLine());
                    }
                }
                using (StreamReader inputFile = new StreamReader("dd84ai_RGR_input_OY.txt"))
                {
                    for (int i = 0; i < Y.Count(); i++)
                        Y[i] = System.Convert.ToDouble(inputFile.ReadLine());
                }

                //Anti-Bug!
                Size_sparse = Math.Max(X_count, Y_count) + 1; //Half size of tape
            }
            void G_left_filling_default()
            {
                G_left[0, 0] = 2; G_left[0, 1] = -2; G_left[0, 2] = 1; G_left[0, 3] = -1;
                G_left[1, 0] = -2; G_left[1, 1] = 2; G_left[1, 2] = -1; G_left[1, 3] = 1;
                G_left[2, 0] = 1; G_left[2, 1] = -1; G_left[2, 2] = 2; G_left[2, 3] = -2;
                G_left[3, 0] = -1; G_left[3, 1] = 1; G_left[3, 2] = -2; G_left[3, 3] = 2;
            }
            void G_right_filling_default()
            {
                G_right[0, 0] = 2; G_right[0, 1] = 1; G_right[0, 2] = -2; G_right[0, 3] = -1;
                G_right[1, 0] = 1; G_right[1, 1] = 2; G_right[1, 2] = -1; G_right[1, 3] = -2;
                G_right[2, 0] = -2; G_right[2, 1] = -1; G_right[2, 2] = 2; G_right[2, 3] = 1;
                G_right[3, 0] = -1; G_right[3, 1] = -2; G_right[3, 2] = 1; G_right[3, 3] = 2;
            }
            void G_filling(double hx, double hy, double value)
            {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        G[i, j] = (((1/(4 * Math.PI * Math.Pow(10, -7) * value)) * hy) / (6 * hx)) * (G_left[i, j] + G_right[i, j]);
                    }
            }
            void M_filling_default()
            {
                M_default[0, 0] = 4; M_default[0, 1] = 2; M_default[0, 2] = 2; M_default[0, 3] = 1;
                M_default[1, 0] = 2; M_default[1, 1] = 4; M_default[1, 2] = 1; M_default[1, 3] = 2;
                M_default[2, 0] = 2; M_default[2, 1] = 1; M_default[2, 2] = 4; M_default[2, 3] = 2;
                M_default[3, 0] = 1; M_default[3, 1] = 2; M_default[3, 2] = 2; M_default[3, 3] = 4;

                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        M[i, j] = 0;
            }
            void M_filling(double hx, double hy)
            {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        M[i, j] = ((Gamma * hx * hy) / (36)) * (M_default[i, j]);
                    }
            }
            void b_filling(double hx, double hy, double value)
            {
                double f1 = value;
                double f2 = value;
                double f3 = value;
                double f4 = value;

                b[0] = (hx * hy / 36) * (4 * f1 + 2 * f2 + 2 * f3 + f4);
                b[1] = (hx * hy / 36) * (2 * f1 + 4 * f2 + f3 + 2 * f4);
                b[2] = (hx * hy / 36) * (2 * f1 + f2 + 4 * f3 + 2 * f4);
                b[3] = (hx * hy / 36) * (f1 + 2 * f2 + 2 * f3 + 4 * f4);
            }
            void A_and_F_by_default_zero()
            {
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                        A[i, j] = 0;
                    F[i] = 0;
                }
            }

            void A_and_F_filling(int i, int j)
            {
                int[] ind = new int[4];
                ind[0] = i + (j) * (X.Count());
                ind[1] = (i + 1) + (j) * (X.Count());
                ind[2] = i + (j + 1) * (X.Count());
                ind[3] = (i + 1) + (j + 1) * (X.Count());

                //A
                for (int k = 0; k < 4; k++)
                    for (int m = 0; m < 4; m++)
                    {
                        A[ind[k], ind[m]] += G[k, m] + M[k, m];
                    }
                //F
                for (int k = 0; k < 4; k++)
                    F[ind[k]] += b[k];
            }

            void A_tranfroming_into_dense_LU()
            {
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (i <= j)
                        {
                            double s = 0;
                            for (int k = 0; k < i; k++)
                                s -= A[i, k] * A[k, j];
                            A[i, j] += s;
                        }
                        else
                        {
                            double s = 0;
                            for (int k = 0; k < j; k++)
                                s -= A[i, k] * A[k, j];
                            A[i, j] += s;
                            A[i, j] /= A[j, j];
                        }
                    }
                }
            }
            double[] Direct_for_dense_Ly_F(double[] F)
            {
                double[] y = new double[Size];
                for (int i = 0; i < Size; i++)
                    y[i] = F[i];

                for (int i = 0; i < Size; i++)
                {
                    double sum = 0;
                    for (int k = 0; k < i; k++)
                        sum -= A[i, k] * y[k];
                    y[i] = F[i] + sum;
                }
                return y;
            }
            double[] Reverse_for_dense_Ux_y(double[] y)
            {
                double[] x = y;

                for (int i = Size - 1; i >= 0; i--)
                {
                    for (int k = i + 1; k < Size; k++)
                        y[i] -= A[i, k] * x[k];
                    x[i] /= A[i, i];
                }

                return x;
            }
            bool debug = false;
            double[,] G = new double[4, 4];
            double[,] G_left = new double[4, 4];
            double[,] G_right = new double[4, 4];
            double[,] M = new double[4, 4];
            double[,] M_default = new double[4, 4];
            double[] b = new double[4];
            double[,] A; //global dense matrix
            double[] F; //for Ax=F
            double[] y;
            int Size;
            void Show_matrix(double[,] s)
            {
                int size = System.Convert.ToInt32(Math.Sqrt(s.Length));
                for (int j = 0; j < size; j++)
                    Console.Write("=");
                Console.WriteLine();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (i == j)
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        else if (Math.Abs(i - j) < 6)
                            Console.ForegroundColor = ConsoleColor.Yellow;

                        Console.Write("{0:G4}\t", s[i, j]);

                        Console.ResetColor(); // reset to normal text color;
                    }
                    Console.WriteLine();
                }
                for (int j = 0; j < size; j++)
                    Console.Write("=");
                Console.WriteLine();
            }
            void Show_vector(double[] v)
            {
                int size = v.Count();
                for (int j = 0; j < size; j++)
                    Console.Write("=");
                Console.WriteLine();
                for (int i = 0; i < size; i++)
                {
                    Console.Write("{0:G4}\t", v[i]);
                    Console.WriteLine();
                }
                for (int j = 0; j < size; j++)
                    Console.Write("=");
                Console.WriteLine();
            }
            List<List<int>> nvtr = new List<List<int>>();
            List<List<int>> nvkat2d = new List<List<int>>();
            List<List<double>> rz_xy = new List<List<double>>();
            List<List<int>> l1 = new List<List<int>>();
            double[,] Ggl;
            double[,] Ggu;
            double[] Ggd;
            double[] F_sparse;
            double[] Y_sparse; //for Ax=F
            double[] X_sparse;
            int Size_sparse = Math.Max(X_count, Y_count) + 1; //Half size of tape
            void A_and_F_by_default_zero__sparse_vers()
            {
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size_sparse; j++)
                    {
                        Ggl[i, j] = 0;
                        Ggu[i, j] = 0;
                    }
                    Ggd[i] = 0;
                    F_sparse[i] = 0;
                }
            }
            void A_and_F_filling__sparse_vers(int nvtr_i) //local i,j
            {
                //4 икса на 3 игрека.
                //Сколько иксов в ряду? = 4.
                //i + (j+1)*(X.Count(), (i + 1) + (j+1)*(X.Count()
                //i + (j)*(X.Count(), (i + 1)   + (j)*(X.Count()

                //int[] ind = new int[4];
                //ind[0] = nvtr[nvtr_i][0];
                //ind[1] = nvtr[nvtr_i][1];
                //ind[2] = nvtr[nvtr_i][2];
                //ind[3] = nvtr[nvtr_i][3];

                //A
                for (int k = 0; k < 4; k++)
                    for (int m = 0; m < 4; m++)
                    {
                        int i = nvtr[nvtr_i][k];
                        int j = nvtr[nvtr_i][m];
                        double value = G[k, m] + M[k, m];

                        if (i == j) Ggd[i] += value;
                        else if (i > j) Ggl[i, Size_sparse - (i - j)] += value;
                        else if (i < j) Ggu[j, Size_sparse - (j - i)] += value;
                    }
                //F
                for (int k = 0; k < 4; k++)
                    F_sparse[nvtr[nvtr_i][k]] += b[k];
            }
            static bool internal_adjusting_of_boundaries__sparse_vers(int j)
            {
                if (boundary_left_1_type)
                {
                    if (j % X.Count() == 0)
                    {
                        return true;
                    }
                }

                if (boundary_right_1_type)
                {
                    if ((j + 1) % X.Count() == 0)
                    {
                        return true;
                    }
                }

                if (boundary_bottom_1_type)
                {
                    if (j < X.Count())
                    {
                        return true;
                    }
                }

                if (boundary_top_1_type)
                {
                    int k = j + (Y.Count() - 1) * X.Count();
                    if (j > (Y.Count() - 1) * X.Count())
                    {
                        return true;
                    }
                }
                return false;
            }

            void A_F_adjusting_for_boundaries__sparse_vers()
            {
                int counter = 0;
                //Занулил нужные строки матрицы
                for (int K = 0; K < l1.Count(); K++)
                //for (int i = 0; i < Size; i++)
                {
                    int i = l1[K][0];
                    //x_index = i % X.Count(); //x = 2;
                    //y_index = i / X.Count(); //y = 1;
                    //f_value = U_analit(X[x_index], Y[y_index]);

                    //if (internal_adjusting_of_boundaries__sparse_vers(i))
                    //{
                        //Console.Write("i = {0}", i);

                        //bool found = false;
                        //foreach (var item in l1)
                        //    if (item[0] == i) { found = true; counter++; }
                        //if (found) Console.WriteLine(" OK");
                        //else Console.WriteLine(" Error");

                        //То надо занулить всю эту строку...
                        for (int j = 0; j < Size_sparse; j++)
                        {
                            Ggl[i, j] = 0;
                        }

                        for (int i2 = 0; i2 < Size; i2++)
                            for (int j = 0; j < Size_sparse; j++)
                            {
                                if (i2 + j - Size_sparse == i)
                                    Ggu[i2, j] = 0;
                            }

                        //Хорошо, теперь поставить единичку на диагональ.
                        Ggd[i] = 1;
                        //И осталось вектору F приписать вычисленное значение
                        //F_sparse[i] = f_value;
                        F_sparse[i] = 0;
                        //Вот и вроде всё с этим.
                    //}
                }
                Console.WriteLine("Counter = {0}", counter);
                Console.WriteLine("l1.Count() = {0}", l1.Count());
            }
            //static double[] test;
            void generating_global_matrix()
            {
                G_left_filling_default(); if (debug) Show_matrix(G_left);
                G_right_filling_default(); if (debug) Show_matrix(G_right);
                M_filling_default(); if (debug) Show_matrix(M_default);

                Size = rz_xy.Count();

                #if (Defined_Dense_Matrix_is_online)
                A = new double[Size, Size];
#endif
                //new adition
                Ggl = new double[Size, Size_sparse];
                Ggu = new double[Size, Size_sparse];
                Ggd = new double[Size];
                F = new double[Size];
                F_sparse = new double[Size];
                //

                A_and_F_by_default_zero__sparse_vers(); // new adition

                //for (int i = 0; i < X.Count() - 1; i++)
                    //for (int j = 0; j < Y.Count() - 1; j++)
                    for (int i = 0; i < nvtr.Count(); i++)
                    {
                        double hx = Math.Abs(rz_xy[nvtr[i][1]][0] - rz_xy[nvtr[i][0]][0]);
                        double hy = Math.Abs(rz_xy[nvtr[i][0]][1] - rz_xy[nvtr[i][2]][1]);
                        //Console.WriteLine("hx = {0}, hy = {1}", hx, hy);
                        G_filling(hx, hy, mu[nvkat2d[i][0]].Doubler); if (debug) Show_matrix(G);
                        //M_filling(hx, hy); if (debug) Show_matrix(M);
                        b_filling(hx, hy, toku[nvkat2d[i][0]].Doubler); if (debug) Show_vector(b);

//#if (Defined_Dense_Matrix_is_online)
                        //A_and_F_filling(i, j); if (debug) Show_matrix(A);
//#endif
                        A_and_F_filling__sparse_vers(i);
                    }

                if (debug) Show_vector(F);
                #if (Defined_Dense_Matrix_is_online)
                A_F_adjusting_for_boundaries();
#endif
                if (debug) Show_matrix(A);
                if (debug) Show_vector(F);

                A_F_adjusting_for_boundaries__sparse_vers();
            }
            double[,] aGgl;
            double[,] aGgu;
            double[] aGgd;
            void copy_M_to_LUM()
            {
                aGgl = new double[Size, Size_sparse];
                aGgu = new double[Size, Size_sparse];
                aGgd = new double[Size];
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size_sparse; j++)
                    {
                        aGgl[i, j] = Ggl[i, j];
                        aGgu[i, j] = Ggu[i, j];
                    }
                    aGgd[i] = Ggd[i];
                }
            }
            void A_sparse_transforming_into_sparse_LU()
            {
                for (int i = 0; i < Size; i++)
                {
                    double sd = 0;
                    int j = i - Size_sparse;
                    for (int jL = 0; jL < Size_sparse; jL++, j++)
                    {
                        if (j < 0) continue;
                        double sl = 0;
                        double su = 0;

                        int jU2 = Size_sparse - j;
                        int jL2 = Size_sparse - i;
                        for (; jL2 < Size_sparse && jU2 < Size_sparse; jU2++, jL2++)
                        {
                            if ((jL2 < 0) || (jU2 < 0)) continue;

                            su += Ggu[i, jL2] * Ggl[j, jU2];
                            sl += Ggl[i, jL2] * Ggu[j, jU2];
                        }

                        Ggu[i, jL] = Ggu[i, jL] - su;
                        Ggl[i, jL] = (Ggl[i, jL] - sl) / Ggd[j];
                        sd += Ggl[i, jL] * Ggu[i, jL];
                    }
                    Ggd[i] = Ggd[i] - sd;
                }
            }
            double[] Direct_for_sparse_Ly_F(double[] F)
            {
                double[] b = new double[Size];
                for (int i = 0; i < Size; i++)
                    b[i] = F[i];

                for (int i = 0; i < Size; i++)
                {
                    double s = 0;
                    for (int j = i - Size_sparse, jL = 0; jL < Size_sparse; j++, jL++)
                    {
                        if (j >= 0)
                            s += Ggl[i, jL] * b[j];
                    }
                    b[i] -= s;
                }
                return b;
            }
            double[] Direct(double[,] glgu, double[] di, double[] F)
            {
                double[] b = new double[Size];
                for (int i = 0; i < Size; i++)
                    b[i] = F[i];

                for (int i = 0; i < Size; i++)
                {
                    double s = 0;
                    for (int j = i - Size_sparse, jL = 0; jL < Size_sparse; j++, jL++)
                    {
                        if (j >= 0)
                            s += glgu[i, jL] * b[j];
                    }
                    b[i] -= s;
                    if (di != null) b[i] /= di[i];
                }
                return b;
            }
            double[] Reverse_for_sparse_Ux_y(double[] F)
            {
                double[] b = new double[Size];
                for (int i = 0; i < Size; i++)
                    b[i] = F[i];

                for (int i = Size - 1; i >= 0; i--)
                {
                    double s = 0;

                    for (int j = i + 1, stop = 0; (j < Size) && (stop < Size_sparse); j++, stop++) //stop - так как элементов не может быть более полуширины ленты
                        s += Ggu[j, i + Size_sparse - j] * b[j];

                    b[i] = (1 / Ggd[i]) * (b[i] - s);
                }

                return b;
            }
            double[] Reverse(double[,] glgu, double[] di, double[] F)
            {
                double[] b = new double[Size];
                for (int i = 0; i < Size; i++)
                    b[i] = F[i];

                for (int i = Size - 1; i >= 0; i--)
                {
                    double s = 0;

                    for (int j = i + 1, stop = 0; (j < Size) && (stop < Size_sparse); j++, stop++) //stop - так как элементов не может быть более полуширины ленты
                        s += glgu[j, i + Size_sparse - j] * b[j];

                    b[i] -= s;
                    if (di != null) b[i] /= di[i];
                }

                return b;
            }
            double[] multiplicate_Ax(double[] x)
            {
                double[] b = new double[Size];

                for (int i = 0; i < Size; i++)
                {
                    b[i] = aGgd[i] * x[i];
                }
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size_sparse; j++)
                    {
                        if (i - Size_sparse + j >= 0)
                        {
                            b[i] += aGgl[i, j] * x[i - Size_sparse + j];
                            b[i - Size_sparse + j] += aGgu[i, j] * x[i];
                        }
                    }
                }
                return b;
            }
            double[] multiplicate_ATx(double[] x)
            {
                double[] b = new double[Size];

                //Тут совершено священодействие.
                for (int i = 0; i < Size; i++)
                {
                    b[i] = aGgd[i] * x[i];
                }
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size_sparse; j++)
                    {
                        if (i - Size_sparse + j >= 0)
                        {
                            b[i] += aGgu[i, j] * x[i - Size_sparse + j];
                            b[i - Size_sparse + j] += aGgl[i, j] * x[i];
                        }
                    }
                }
                return b;
            }
            void vect_a_equals_b(double[] a, double[] b)
            {
                for (int i = 0; i < a.Count(); i++)
                    a[i] = b[i];
            }
            void vect_a_equals_0(double[] x)
            {
                for (int i = 0; i < x.Count(); i++)
                    x[i] = 0;
            }
            double vect_norma(double[] x)
            {
                double max = 0;
                for (int i = 0; i < Size; i++)
                    max += x[i] * x[i];
                return Math.Sqrt(max);

            }
            double[] vect_b_minus_c(double[] b, double[] c)
            {
                double[] result = new double[Size];
                for (int i = 0; i < Size; i++)
                    result[i] = b[i] - c[i];
                return result;
            }
            double[] vect_equals(double[] x)
            {
                double[] result = new double[Size];
                for (int i = 0; i < Size; i++)
                    result[i] = x[i];
                return result;
            }
            double vect_scalar_a_and_b(double[] a, double[] b)
            {
                double res = 0;
                for (int i = 0; i < Size; i++)
                    res += a[i] * b[i];
                return res;
            }
            double[] X0;
            double[] R;
            double[] Z;
            double[] P;
            double[] Ar;
            void vect_a_plus_equals_b_plus_c_times_const(double[] a, double[] b, double[] c, double cons)
            {
                for (int i = 0; i < Size; i++)
                    a[i] = b[i] + c[i] * cons;
            }

            void MSG_LU()
            {
                bool is_answer_correct = false;
                double scalarRR = 0, scalarRR2 = 0, a = 0, b = 0;

                X0 = new double[Size];
                vect_a_equals_0(X0); /* X0 = 0 */
                double vect_norma_F = vect_norma(F_sparse);

                //R = multiplicate_Ax(X0);  // r = A*x
                R = new double[Size];
                vect_a_equals_0(R);
                R = vect_b_minus_c(F_sparse, R); // R = F - R
                R = Direct(Ggl, Ggd, R); // r = L-1 * r
                R = Reverse(Ggl, Ggd, R);// r = L-t * r
                Y = multiplicate_ATx(R);// Y = At*R
                R = Direct(Ggu, null, Y);// r = U-t * y*/
                Z = vect_equals(R); // r = z	

                double test;
                for (int iter = 1; iter < Maxiter; iter++)
                {
                    if ((test = Math.Sqrt(vect_scalar_a_and_b(R, R)) / vect_norma_F) < E) { is_answer_correct = true; break; }
                    scalarRR = vect_scalar_a_and_b(R, R);

                    Y = Reverse(Ggu, null, Z);// y = U-1 * z
                    P = multiplicate_Ax(Y);// P = A * y
                    P = Direct(Ggl, Ggd, P);// p = L-t * p
                    P = Reverse(Ggl, Ggd, P);// p = L-t * p

                    Ar = multiplicate_ATx(P);// Ar = At*P
                    Ar = Direct(Ggu, null, Ar);// Ar = U-t * Ar

                    a = scalarRR / vect_scalar_a_and_b(Ar, Z); //a=(R,R)/(Ar,z)

                    vect_a_plus_equals_b_plus_c_times_const(X0, X0, Z, a); //X0 = X0 + a*Z
                    vect_a_plus_equals_b_plus_c_times_const(R, R, Ar, -a); //R = R - a*Ar

                    b = (scalarRR2 = vect_scalar_a_and_b(R, R)) / scalarRR; //b = (rk+1,rk+1)/(rk,rk)
                    scalarRR = scalarRR2;

                    vect_a_plus_equals_b_plus_c_times_const(Z, R, Z, b); //z = rk + beta*Z;
                }
                X0 = Reverse(Ggu, null, X0);// x0 = U-1 * x0
            }

            void MSG()
            {
                bool is_answer_correct = false;
                double scalarRR = 0, a = 0, b = 0;
                X0 = new double[Size];
                vect_a_equals_0(X0); /* X0 = 0 */
                double vect_norma_F = vect_norma(F);

                R = multiplicate_Ax(X0); // R = A*x
                R = vect_b_minus_c(F_sparse, R); // R = F - R
                Z = multiplicate_ATx(R); // z = At*R
                vect_a_equals_b(R, Z); // r = z

                for (int iter = 1; iter < Maxiter; iter++)
                {
                    if (Math.Sqrt(scalarRR = vect_scalar_a_and_b(R, R)) / vect_norma_F < E) { is_answer_correct = true; break; }

                    P = multiplicate_Ax(Z); // P = A*Z
                    Ar = multiplicate_ATx(P); // Ar = At*P
                    a = scalarRR / vect_scalar_a_and_b(Ar, Z); //a=(R,R)/(Ar,z)

                    vect_a_plus_equals_b_plus_c_times_const(X0, X0, Z, a); //X0 = X0 + a*Z
                    vect_a_plus_equals_b_plus_c_times_const(R, R, Ar, -a); //R = R - a*Ar

                    b = vect_scalar_a_and_b(R, R) / scalarRR; //b = (rk+1,rk+1)/(rk,rk)

                    vect_a_plus_equals_b_plus_c_times_const(Z, R, Z, b); //z = rk + beta*Z;
                }
            }
            void Make_Hilbert_Proud()
            {
                Size = 3;
                Size_sparse = 3;
                Ggd = new double[3];
                Ggl = new double[3, 3];
                Ggu = new double[3, 3];
                Ggd[0] = 2; Ggd[1] = (double)1 / 3; Ggd[2] = (double)1 / 5;

                Ggl[0, 0] = 0; Ggl[0, 1] = 0; Ggl[0, 2] = 0;
                Ggl[1, 0] = 0; Ggl[1, 1] = 0; Ggl[1, 2] = (double)1 / 2;
                Ggl[2, 0] = 0; Ggl[2, 1] = (double)1 / 3; Ggl[2, 2] = (double)1 / 4;

                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        Ggu[i, j] = Ggl[i, j];

                F = new double[3];
                F[0] = (double)4; F[1] = (double)23 / 12; F[2] = (double)43 / 30;

                F_sparse = new double[3];
                for (int i = 0; i < Size; i++)
                    F_sparse[i] = F[i];
            }
            void Make_Hilbert_Proud2()
            {
                A = new double[3, 3];
                A[0, 0] = (double)2; A[0, 1] = (double)1 / 2; A[0, 2] = (double)1 / 3;
                A[1, 0] = (double)1 / 2; A[1, 1] = (double)1 / 3; A[1, 2] = (double)1 / 4;
                A[2, 0] = (double)1 / 3; A[2, 1] = (double)1 / 4; A[2, 2] = (double)1 / 5;
            }
            void Make_Hilbert_Proud_C_sharp_LU()
            {
                Ggd[0] = 2; Ggd[1] = 0.20833333333333331; Ggd[2] = 0.011111111111111072;

                Ggl[0, 0] = 0; Ggl[0, 1] = 0; Ggl[0, 2] = 0;
                Ggl[1, 0] = 0; Ggl[1, 1] = 0; Ggl[1, 2] = 0.25;
                Ggl[2, 0] = 0; Ggl[2, 1] = 0.16666666666666666; Ggl[2, 2] = 0.80000000000000016;

                Ggu[0, 0] = 0; Ggu[0, 1] = 0; Ggu[0, 2] = 0;
                Ggu[1, 0] = 0; Ggu[1, 1] = 0; Ggu[1, 2] = 0.5;
                Ggu[2, 0] = 0; Ggu[2, 1] = 0.33333333333333331; Ggu[2, 2] = 0.16666666666666669;
            }
            void Make_Hilbert_Proud_C_plusplus_LU()
            {
                Ggd[0] = 2; Ggd[1] = 0.20833330000000000; Ggd[2] = -0.037037075703711758;

                Ggl[0, 0] = 0; Ggl[0, 1] = 0; Ggl[0, 2] = 0;
                Ggl[1, 0] = 0; Ggl[1, 1] = 0; Ggl[1, 2] = 0.25000000000000000;
                Ggl[2, 0] = 0; Ggl[2, 1] = 0.16666665000000000; Ggl[2, 2] = 0.93333353600002977;

                Ggu[0, 0] = 0; Ggu[0, 1] = 0; Ggu[0, 2] = 0;
                Ggu[1, 0] = 0; Ggu[1, 1] = 0; Ggu[1, 2] = 0.50000000000000000;
                Ggu[2, 0] = 0; Ggu[2, 1] = 0.33333330000000000; Ggu[2, 2] = 0.19444445555555501;

                F[0] = 4.0000000000000000; F[1] = 1.9166669999999999; F[2] = 1.4333330000000000;
                F_sparse[0] = 4.0000000000000000; F_sparse[1] = 1.9166669999999999; F_sparse[2] = 1.4333330000000000;

                aGgl[0, 0] = 0; aGgl[0, 1] = 0; aGgl[0, 2] = 0;
                aGgl[1, 0] = 0; aGgl[1, 1] = 0; aGgl[1, 2] = 0.5000000000000000;
                aGgl[2, 0] = 0; aGgl[2, 1] = 0.33333330000000000; aGgl[2, 2] = (float)1 / 4;

                aGgu[0, 0] = 0; aGgu[0, 1] = 0; aGgu[0, 2] = 0;
                aGgu[1, 0] = 0; aGgu[1, 1] = 0; aGgu[1, 2] = 0.50000000000000000;
                aGgu[2, 0] = 0; aGgu[2, 1] = 0.33333330000000000; aGgu[2, 2] = (float)1 / 4;

                aGgd[0] = 2; aGgd[1] = 0.33333330000000000; aGgd[2] = (float)1 / 5;

                Size = 3; Size_sparse = 3;
            }
            public string ProjectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            //"nvtr.dat"
            //CountOfNumbers = 6
            //CountOfUnreadableNumbers = 2
            public void readBinary(string FileName, int LimitToRead,int CountOfNumbers, int CountOfUnreadableNumbers,
                List<List<int>> IntList = null, List<List<double>> DoubleList = null)
            {
                if (IntList != null) IntList.Clear();
                else DoubleList.Clear();


                string Path = ProjectPath + "\\" + "Telma" + "\\" + FileName;
                using (BinaryReader reader = new BinaryReader(File.Open(Path, FileMode.Open)))
                {
                    Int32 IntNumber = 0;
                    Double DoubleNumber = 0;
                    int counter = 0;
                    int realCounter = 0;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        if (counter / CountOfNumbers < LimitToRead)
                        {
                            if (IntList != null)
                                IntNumber = reader.ReadInt32();
                            else DoubleNumber = reader.ReadDouble();

                            if (counter % CountOfNumbers == 0)
                            {
                                if (IntList != null)
                                    IntList.Add(new List<int>());
                                else DoubleList.Add(new List<double>());
                            }

                            if (counter % CountOfNumbers < CountOfNumbers - CountOfUnreadableNumbers)
                            {
                                if (IntList != null)
                                    IntList[IntList.Count() - 1].Add(IntNumber - 1);
                                else DoubleList[DoubleList.Count() - 1].Add(DoubleNumber);
                                realCounter++;
                                //Console.Write("{0};", number);
                            }
                        }
                        else break;
                        counter++;

                    }

                }
                Console.Write("");
            }
            public string RemoveExtraSpaces(string str)
            {
                string OldString = "";
                str = str.Replace('.', System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0]);
                str = str.Replace(',', System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0]);
                bool DoReplace = true;
                while (DoReplace)
                {
                    OldString = str;
                    str = str.Replace("  ", " ");
                    if (str == OldString) DoReplace = false;
                }

                return str;
            }
            public void readTextedFile(string FileName, int CountOfNumbers, int CountOfUnreadableNumbers,
                List<PointIntDouble> Listik)
            {
                string Path = ProjectPath + "\\" + "Telma" + "\\" + FileName;
                using (StreamReader inputFile = new StreamReader(Path))
                {
                    string ReadLiner;
                    while ((ReadLiner = inputFile.ReadLine()) != null)
                    {

                        ReadLiner = RemoveExtraSpaces(ReadLiner);
                        string[] Splitted = ReadLiner.Split(' ');

                        Listik.Add(new PointIntDouble(
                            Convert.ToInt32(Splitted[1]) - 1,
                            Convert.ToDouble(Splitted[2])
                            ));
                    }
                }
            }
            public class PointIntDouble
            {
                public int Integer;
                public double Doubler;
                public PointIntDouble(int _int, double _double)
                {
                    Integer = _int;
                    Doubler = _double;
                }
            }
            List<PointIntDouble> mu = new List<PointIntDouble>();
            List<PointIntDouble> toku = new List<PointIntDouble>();

            List<List<double>> r_x = new List<List<double>>();
            List<List<double>> r_y = new List<List<double>>();
            
            public void readNumber(string FileName, string FindNumber, ref int Number)
            {
                string Path = ProjectPath + "\\" + "Telma" + "\\" + FileName;
                using (StreamReader inputFile = new StreamReader(Path))
                {
                    string ReadLiner;
                    while ((ReadLiner = inputFile.ReadLine()) != null)
                    {

                        ReadLiner = RemoveExtraSpaces(ReadLiner);
                        string[] Splitted = ReadLiner.Split(' ');

                        int Found = 0; bool boolFound = false;
                        for (int i = 0; i < Splitted.Count(); i++)
                            if (Splitted[i] == FindNumber) { Found = i; boolFound = true; break; }

                        if (boolFound)
                        {
                            Number = Convert.ToInt32(Splitted[Found + 1]);
                        }

                    }
                }
            }
            public int kuzlov = 0, ktr = 0, kt1 = 0;
            public void Call_the_Graphic_Window()
            {
                List<List<double>> answer = new List<List<double>>();
                for (int i = 0; i < r_y.Count(); i++)
                {
                    answer.Add(new List<double>());
                    for (int j = 0; j < r_x.Count(); j++)
                    {
                        answer[i].Add(0);
                    }
                }
                //Теперь надо инвентировано X_sparse переписать в answer

                int counter = 0;
                for (int i = 0; i < r_y.Count(); i++)
                {
                    for (int j = 0; j < r_x.Count(); j++)
                    {
                        answer[r_y.Count() - 1 - i][j] = X_sparse[counter];
                        counter++;
                    }
                }

                SharpGL_limbo.List_Of_Objects.Add(new GraphicData.GraphicObject("Answer", answer));
                SharpGL_limbo.Refresh_Window();
                SharpGL_limbo.SharpGL_Open();
            }
            bool CalculatePreciseThing(double x, double y, ref double Answer)
            {
                bool found = false; int i = 0;

                double old1 = 0, old2 = 0;
                for (; i < nvtr.Count; i++)
                {
                    //nvtr[i][2]] nvtr[i][1]]

                    //double left_X = rz_xy[nvtr[i][2]][0];
                    //double right_X = rz_xy[nvtr[i][1]][0];
                    //double left_Y = rz_xy[nvtr[i][2]][1];
                    //double right_Y = rz_xy[nvtr[i][1]][1];

                    if (x > rz_xy[nvtr[i][2]][0]
                        && x < rz_xy[nvtr[i][1]][0])
                        if (y > rz_xy[nvtr[i][2]][1]
                        && y < rz_xy[nvtr[i][1]][1])
                        {
                            found = true; break;
                        }
                }
                if (found) {

                    //81, 82, 0, 1
                    //left-top, right-top, left-bottom, right-bottom;
                    //

                    double hx = Math.Abs(rz_xy[nvtr[i][1]][0] - rz_xy[nvtr[i][2]][0]);
                    double hy = Math.Abs(rz_xy[nvtr[i][1]][1] - rz_xy[nvtr[i][2]][1]);

                    double X1 = (rz_xy[nvtr[i][1]][0] - x) / hx;
                    double X2 = (x - rz_xy[nvtr[i][2]][0]) / hx;
                    double Y1 = (rz_xy[nvtr[i][1]][1] - y) / hy;
                    double Y2 = (y - rz_xy[nvtr[i][2]][1]) / hy;

                    Answer = 0;
                    Answer += X_sparse[nvtr[i][2]] * X1 * Y1;
                    Answer += X_sparse[nvtr[i][3]] * X2 * Y1;
                    Answer += X_sparse[nvtr[i][0]] * X1 * Y2;
                    Answer += X_sparse[nvtr[i][1]] * X2 * Y2;

                    /*Console.WriteLine("left-bottom = \t{0};{1}", rz_xy[nvtr[i][2]][0], rz_xy[nvtr[i][2]][1]);
                    Console.WriteLine("right-bottom = \t{0};{1}", rz_xy[nvtr[i][3]][0], rz_xy[nvtr[i][3]][1]);
                    Console.WriteLine("left-top = \t{0};{1}", rz_xy[nvtr[i][0]][0], rz_xy[nvtr[i][0]][1]);
                    Console.WriteLine("right-top = \t{0};{1}", rz_xy[nvtr[i][1]][0], rz_xy[nvtr[i][1]][1]);
                    Console.WriteLine("x-y = \t{0};{1}", x,y);

                    Console.WriteLine("left-bottom = \t{0}", X_sparse[nvtr[i][2]]);
                    Console.WriteLine("right-bottom = \t{0}", X_sparse[nvtr[i][3]]);
                    Console.WriteLine("left-top = \t{0}", X_sparse[nvtr[i][0]]);
                    Console.WriteLine("right-top = \t{0}", X_sparse[nvtr[i][1]]);
                    Console.WriteLine("value = \t{0}", Answer);*/

                    return true; }
                else return false;
            }
            List<List<double>> points = new List<List<double>>();
            public void readPointsFile()
            {
                string Path = ProjectPath + "\\" + "Telma" + "\\" + "Point";
                using (StreamReader inputFile = new StreamReader(Path))
                {
                    string ReadLiner = inputFile.ReadLine();
                    while ((ReadLiner = inputFile.ReadLine()) != null)
                    {

                        ReadLiner = RemoveExtraSpaces(ReadLiner);
                        string[] Splitted = ReadLiner.Split(' ');

                        points.Add(new List<double>());
                        points[points.Count() - 1].Add(Convert.ToDouble(Splitted[0]));
                        points[points.Count() - 1].Add(Convert.ToDouble(Splitted[1]));
                    }
                }
            }
            void Sub_Main()
            {


                //Шаг нулевой. Считывание среды
                areas.reading_sreda();

                //List<List<int>> nvtr = new List<List<int>>();
                //List<List<int>> nvkat2d = new List<List<int>>();
                //List<List<double>> rz_xy = new List<List<double>>();
                //List<List<int>> l1 = new List<List<int>>();
                //Шаг ноль с половинкой. Считывание телмы.

                readNumber("inf2tr.dat", "kuzlov=",ref kuzlov);
                readNumber("inf2tr.dat", "ktr=", ref ktr);
                readNumber("inf2tr.dat", "kt1=", ref kt1);
                I.Colorator("inf2tr.dat has been read.", ConsoleColor.Green);

                readBinary("nvtr.dat", ktr, 6,2,nvtr);
                readBinary("nvkat2d.dat", ktr, 1, 0, nvkat2d);
                readBinary("l1.dat", kt1, 1, 0, l1);
                readBinary("rz.dat", kuzlov, 2, 0, null,rz_xy);
                I.Colorator("nvtr.dat and e.t.c. have been read.", ConsoleColor.Green);

                readTextedFile("mu", 2, 0, mu);
                readTextedFile("toku", 2, 0, toku);
                I.Colorator("mu & toku.dat have been read.", ConsoleColor.Green);

                //Считываем Оси
                readBinary("r.dat", Int32.MaxValue, 1, 0, null, r_x);
                readBinary("z.dat", Int32.MaxValue, 1, 0, null, r_y);

                //Находим их лимит в соответствии с границей от
                //rz_xy послед числа
                int limit1 = 0;
                for (int i = 0; i < r_x.Count(); i++)
                    if (r_x[i][0] == rz_xy[rz_xy.Count() - 1][0])
                    {
                        limit1 = i; break;
                    }

                int limit2 = 0;
                for (int i = 0; i < r_x.Count(); i++)
                    if (r_y[i][0] == rz_xy[rz_xy.Count() - 1][1])
                    {
                        limit2 = i; break;
                    }

                limit1++; limit2++;
                //Считываем заново
                readBinary("r.dat", limit1, 1, 0, null, r_x);
                readBinary("z.dat", limit2, 1, 0, null, r_y);

                int test = limit1 * limit2;
                if (test != rz_xy.Count())
                { I.Colorator("test != rz_xy.Count()!", ConsoleColor.Red); Console.ReadKey(); }
                I.Colorator("r.dat & z.dat have been read.", ConsoleColor.Green);

                //Шаг первый. Сгенерировать данные.
                generating_OX_OY_lyambda_gamma();
                I.Colorator("generating_OX_OY_lyambda_gamma();", ConsoleColor.Green);

                //Шаг второй. Считать эти данные из файла.
                reading_input_data();
                I.Colorator("reading_input_data();", ConsoleColor.Green);


                //Шаг третий. Сгенерировать матрицу
                generating_global_matrix();
                I.Colorator("generating_global_matrix();", ConsoleColor.Green);

                //Шаг четвертый. Посчитать СЛАУ.
                y = new double[Size];
#if (Defined_Dense_Matrix_is_online)
                A_tranfroming_into_dense_LU(); if (debug) Show_matrix(A);
#endif
                copy_M_to_LUM();
                I.Colorator("copy_M_to_LUM();", ConsoleColor.Green);

#if (Defined_dense_LU_matrix_is_online)



                y = Direct_for_dense_Ly_F(F); if (debug) Show_vector(y);
                F = Reverse_for_dense_Ux_y(y); if (debug) Show_vector(F);
                if (Show_first_three_elements_from_vectors_of_answers)
                {
                    Save_vector(F, "dd84ai_RGR_output_X0_dense_Straight_LU.txt");
                    Show_three_elements_from_vector(F);
                }
#endif


#if (Defined_sparse_MSG)
                F = vect_equals(F_sparse);
                Console.WriteLine("MSG:");
                MSG();
                if (debug) Show_vector(X0);
                if (Show_first_three_elements_from_vectors_of_answers)
                {
                    Save_vector(X0, "dd84ai_RGR_output_X0_dense_MSG_LU.txt");
                    Show_three_elements_from_vector(X0);
                }
#endif
                //Надо ЛУ факторизацию и МСГ метод.
                //Кратчайшим путем вижу, ЛУ факторизацию
#if (Defined_sparse_LU_matrix_is_online)
                A_sparse_transforming_into_sparse_LU();
                I.Colorator("A_sparse_transforming_into_sparse_LU();", ConsoleColor.Green);

                //Make_Hilbert_Proud_C_sharp_LU();
                //Make_Hilbert_Proud_C_plusplus_LU();

                Y_sparse = Direct_for_sparse_Ly_F(F_sparse);
                I.Colorator("Y_sparse = Direct_for_sparse_Ly_F(F_sparse);", ConsoleColor.Green);
                X_sparse = Reverse_for_sparse_Ux_y(Y_sparse);
                I.Colorator("X_sparse = Reverse_for_sparse_Ux_y(Y_sparse);", ConsoleColor.Green);
                Console.WriteLine("Gauss:");
                if (debug) Show_vector(X_sparse);
                if (Show_first_three_elements_from_vectors_of_answers)
                {
                    Save_vector(X_sparse, "dd84ai_RGR_output_X0_sparse_Straight_LU.txt");
                    Show_three_elements_from_vector(X_sparse);
                }


                readPointsFile();

                double Thing = 0;
                for (int i = 0; i < points.Count();i++)
                    if (CalculatePreciseThing(points[i][0], points[i][1], ref Thing))
                        Console.WriteLine("PreciseValue #{0} = {1}",i,Thing);
                        else Console.WriteLine("Wring Coordinates");

                        Call_the_Graphic_Window();
                

#endif

#if (Defined_sparse_MSG_is_online)
                Console.WriteLine("MSG_LU:");

                MSG_LU();
                if (debug) Show_vector(X0);

                if (Show_first_three_elements_from_vectors_of_answers)
                {
                    Save_vector(X0, "dd84ai_RGR_output_X0_sparse_MSG_LU.txt");
                    Show_three_elements_from_vector(X0);
                }
#endif

            }

        }
        static void Show_three_elements_from_vector(double[] Target_vector)
        {
            Console.WriteLine("===Vector-begin===");
            //for (int i = 0; (i < 3) && (i < Target_vector.Count()); i++)
            for (int i = 0; (i < 6) && (i < Target_vector.Count()); i++)
                Console.WriteLine("{0:E8}", Target_vector[i]);
            Console.WriteLine("===Vector-end===");
        }

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SharpGL_limbo.SharpGL_Open_hidden();

            Finite_Element_Method W1 = new Finite_Element_Method();
            //Finite_Element_Method W1 = new Finite_Element_Method(1 + x_counting, y_counting, 1);
            //Finite_Element_Method W2 = new Finite_Element_Method(1 + Convert.ToInt32(Math.Pow(x_counting,2.0)),Convert.ToInt32(Math.Pow(y_counting,2.0)),2);
            //Finite_Element_Method W3 = new Finite_Element_Method(1 + Convert.ToInt32(Math.Pow(x_counting, 3.0)), Convert.ToInt32(Math.Pow(y_counting, 3.0)), 3);

            Application.Run();

            //Console.ReadKey();
        }
        public class Interface
        {
            public void Greetings()
            {
                Colorator("Yay!", ConsoleColor.Yellow);
                Colorator("Designation: 5sem_4islemetod_RGR", ConsoleColor.Magenta);
            }
            public void Colorator(string str, ConsoleColor color)
            {
                //Red,Green,Blue,Magenta,Cyan; example: ConsoleColor.Magenta;
                Console.ForegroundColor = color; // set text color;
                Console.WriteLine(str);
                Console.ResetColor(); // reset to normal text color;
            }
            public void Pause()
            {
                Colorator("Press Escape to exit.", ConsoleColor.Magenta);
                Console.ReadKey();
            }
            public string ProjectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            public Random random = new Random();
            public void SaySomeQuote()
            {
                string TargetPath = ProjectPath + "\\" + "Music" + "\\";

                if (Directory.Exists(@TargetPath))
                {

                    // Process the list of files found in the directory.
                    var fileEntries = Directory.GetFiles(TargetPath);

                    int Choice = random.Next(1, fileEntries.Count());
                    if (fileEntries.Count() != 0 && fileEntries[Choice].Contains(".wav"))
                        if (true)
                        {
                            //Colorator("Activating music file: " + fileEntries[Choice], ConsoleColor.Yellow);
                            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(fileEntries[Choice]);
                            try {
                                sp.Play();
                                string [] spliited = fileEntries[Choice].Split('\\');
                                Colorator("Music file " + spliited[spliited.Count() - 1] + " has been activated. ", ConsoleColor.Green);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine();
                                for (int i = 0; i < 3; i++)
                                    Colorator("Music file " + fileEntries[Choice] + " is not found!!! ", ConsoleColor.Red);
                            }

                        }
                }
                else
                {
                    Console.WriteLine();
                    for (int i = 0; i < 3; i++)
                        Colorator("Folder does not exist!!! ", ConsoleColor.Red);
                }
            }
            public void Time_pause(int value)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < value)
                { }
            }

        }
        public class data_for_O
        {
            public int size;
            public double t0;
            public double tn;

            public data_for_O(int size1, double t01, double tn1)
            {
                size = size1;
                t0 = t01;
                tn = tn1;
            }
        }
        public class data_other
        {
            static public double Lyambda, Gamma;
        }
        public class data_for_generating : data_other
        {

            public data_for_generating(double Lyambda1, double Gamma1)
            {
                Lyambda = new double();
                Gamma = new double();
                Lyambda = Lyambda1;
                Gamma = Gamma1;
            }
        }
    }
}
