using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AngouriMath;
using AngouriMath.Core.Exceptions;
using AngouriMath.Extensions;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

        }
        //Запрет на ввод букв в текстбокс
        void TextBox1KeyPress(object sender, KeyPressEventArgs e)
        {
            var number = e.KeyChar;
            //   Число                   Запятая         Backspace      Минус
            if (!Char.IsDigit(number) && number != 44 && number != 8 && number != 45)
            {
                e.Handled = true;
            }
        }
        //Для точности
        void TextBox2KeyPress(object sender, KeyPressEventArgs e)
        {
            var number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 44 && number != 8)
            {
                e.Handled = true;
            }
        }

        private void TextBox6_TextChanged(object sender, EventArgs e)
        {
            count = 1;
        }


        //Счет сразу
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //очистка графиков
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();

                textBox6.Text = "-";
                count = 1;
                double firstLimit = Convert.ToDouble(textBox2.Text);
                double secondLimit = Convert.ToDouble(textBox3.Text);
                if (firstLimit <= secondLimit)
                {
                    //Парсинг формулы и границ
                    Entity formula = Convert.ToString(textBox1.Text);
                    formula = formula.Simplify();

                    double y = 0;
                    //постройка графиков
                    for (double i = firstLimit; i < secondLimit+0.5; i += 0.1)
                    {
                        //Основной график
                        y = (double)formula.Substitute("x", i).EvalNumerical();
                        chart1.Series[0].Points.AddXY(i, y);

                    }
                    //Реализация метода
                    double t = (Math.Sqrt(5) - 1) / 2;
                    double x1 = secondLimit - t * (secondLimit - firstLimit);
                    double x2 = firstLimit + t * (secondLimit - firstLimit);
                    double precision = Convert.ToDouble(textBox5.Text);
                    int countSteps = 0;
                    do
                    {
                        countSteps++;
                        double Fx1 = (double)formula.Substitute("x", x1).EvalNumerical();
                        double Fx2 = (double)formula.Substitute("x", x2).EvalNumerical();
                        if (Fx2 < Fx1)
                        {
                            firstLimit = x1;
                        }
                        else
                        {
                            secondLimit = x2;
                        }
                        x1 = secondLimit - t * (secondLimit - firstLimit);
                        x2 = firstLimit + t * (secondLimit - firstLimit);
                    } while (precision <= Math.Abs(secondLimit - firstLimit));
                    //Постройка точки и вывод результатов
                    double minX = (firstLimit + secondLimit) * 0.5;
                    double Fminx = (double)formula.Substitute("x", minX).EvalNumerical();
                    chart1.Series[1].Points.AddXY(minX, Fminx);
                    textBox7.Text = Convert.ToString(Math.Round(minX, 6));
                    textBox4.Text = Convert.ToString(countSteps);

                }
                else
                {
                    chart1.Series[0].Points.Clear();
                    chart1.Series[1].Points.Clear();
                    MessageBox.Show("Начальная граница не может быть больше конечной ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (AngouriMathBaseException)
            {
                MessageBox.Show("Ошибка в формуле или введенных границах X", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Source}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Счет по шагам
        int count = 1;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Очищаем точки
                textBox6.Clear();
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();

                double firstLimit = Convert.ToDouble(textBox2.Text);
                double secondLimit = Convert.ToDouble(textBox3.Text);
                if (firstLimit <= secondLimit)
                {
                    int countSteps = 0;
                    double[] funcMas = new double[40];
                    Entity formula = Convert.ToString(textBox1.Text);
                    formula = formula.Simplify();
                    double y = 0;
                    for (double i = firstLimit; i < secondLimit+0.5; i += 0.1)
                    {
                        //Основной график
                        y = (double)formula.Substitute("x", i).EvalNumerical();
                        chart1.Series[0].Points.AddXY(i, y);

                    }
                    double t = (Math.Sqrt(5) - 1) / 2;
                    double x1 = secondLimit - t * (secondLimit - firstLimit);
                    double x2 = firstLimit + t * (secondLimit - firstLimit);
                    double precision = Convert.ToDouble(textBox5.Text);
                    do
                    {
                        countSteps++;
                        double Fx1 = (double)formula.Substitute("x", x1).EvalNumerical();
                        double Fx2 = (double)formula.Substitute("x", x2).EvalNumerical();
                        if (Fx2 < Fx1)
                        {
                            firstLimit = x1;
                        }
                        else
                        {
                            secondLimit = x2;
                        }
                        x1 = secondLimit - t * (secondLimit - firstLimit);
                        x2 = firstLimit + t * (secondLimit - firstLimit);
                        double minX = (firstLimit + secondLimit) * 0.5;
                        funcMas[countSteps] = minX;

                    } while (precision <= Math.Abs(secondLimit - firstLimit));
                    double Fminx = (double)formula.Substitute("x", funcMas[count]).EvalNumerical();
                    chart1.Series[1].Points.AddXY(funcMas[count], Fminx);
                    textBox7.Text = Convert.ToString(Math.Round(funcMas[count], 6));
                    textBox4.Text = Convert.ToString(countSteps);
                    textBox6.Text = Convert.ToString(count);
                    count++;

                    if (count - 1 == countSteps)
                    {
                        count = 1;
                    }
                } else
                {
                    chart1.Series[0].Points.Clear();
                    chart1.Series[1].Points.Clear();

                    MessageBox.Show("Начальная граница не может быть больше конечной ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (AngouriMathBaseException)
            {
                MessageBox.Show("Ошибка в формуле или введенных границах X", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Source}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }    
}