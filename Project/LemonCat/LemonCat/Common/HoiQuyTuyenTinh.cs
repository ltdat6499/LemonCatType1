using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Common
{
    public class HoiQuyTuyenTinh
    {
        string[] X = new string[10000];
        string[] Y = new string[10000];
        int n = 0;
        // MyInput = log(TheLoai)
        // MyInput * weight + bias = IDMovie
        public int Result(string[,] Input, int MyInput, int count)
        {
            n = count;
            for (int i = 0; i < n; i++)
            {
                X[i] = Input[i, 0];
                Y[i] = Input[i, 1];
            }
            double weight = 0.03, bias = 0.0014, learning_rate = 0.001, iter = 50;
            double[] cos_his = train(ref weight, ref bias, learning_rate, iter);
            double result = Predict(Math.Log(MyInput), weight, bias);
            return (int)(result + 0.5);
        }

        private double Predict(double new_radio, double weight, double bias)
        {
            return weight * new_radio + bias;
        }
        private double CostFunction(double weight, double bias)
        {
            double sum_error = 0;
            for (int i = 1; i < n; i++)
            {
                sum_error += (double.Parse(Y[i]) - (weight * double.Parse(X[i]) + bias)) * (double.Parse(Y[i]) - (weight * double.Parse(X[i]) + bias));
            }
            return sum_error / n;
        }
        private void update_weight(ref double weight, ref double bias, double learning_rate)
        {
            double weight_temp = 0;
            double bias_temp = 0;
            for (int i = 1; i < n; i++)
            {
                // Đạo hàm
                weight_temp += -2 * double.Parse(X[i]) * (double.Parse(Y[i]) - (double.Parse(X[i]) * weight + bias));
                bias_temp += -2 * (double.Parse(Y[i]) - (double.Parse(X[i]) * weight + bias));

            }
            weight -= (weight_temp / n) * learning_rate;
            bias -= (bias / n) * learning_rate;
        }
        //Train
        private double[] train(ref double weight, ref double bias, double learning_rate, double iter)
        {
            double[] cos_his = new double[1000000];
            double cost = 0;
            for (int i = 0; i < iter; i++)
            {
                update_weight(ref weight, ref bias, learning_rate);
                cost = CostFunction(weight, bias);
                cos_his[i] = cost;
            }
            return cos_his;
        }
    }
}