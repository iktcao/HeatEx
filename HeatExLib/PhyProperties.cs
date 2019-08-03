using System;

namespace HeatExLib
{
    public static class PhyProperties
    {

        /// <summary>
        /// 根据油品的相对密度计算油品的API，无量纲
        /// </summary>
        /// <param name="sp">油品的相对密度（4/20)</param>
        /// <returns>油品的API值</returns>
        public static double API(double sp)
        {
            return (141.5 / (0.99417 * sp + 0.009181) - 131.5);
        }

        /// <summary>
        /// 根据油品的温度和相对密度返回密度，单位：kg/m3
        /// </summary>
        /// <param name="T">油品的温度，单位：℃</param>
        /// <param name="sp">油品的相对密度(4/20)，无量纲</param>
        /// <returns></returns>
        public static double Density(double T, double sp)
        {
            double rho = T * (1.307 * sp - 1.817) + 973.86 * sp + 36.34;
            return rho;
        }
        /// <summary>
        /// 根据油品的温度和相对密度返回导热系数，单位：W/(m*K)
        /// </summary>
        /// <param name="T">油品的温度，单位：℃</param>
        /// <param name="sp">油品的相对密度(4/20)，无量纲</param>
        /// <returns></returns>
        public static double ThermalConductivity(double T, double sp)
        {
            //double lambda = (0.0199 - 0.0000656 * T + 0.098) / (0.99417 * sp + 0.009181);
            double lambda = 0.0199 - 0.0000656 * T + 0.098 / (0.99417 * sp + 0.009181);
            return lambda;
        }

        /// <summary>
        /// 根据油品的温度、API和特性因数返回焓值，单位：J/kg
        /// </summary>
        /// <param name="T">油品的温度，单位：℃</param>
        /// <param name="API">油品的API指数，无量纲</param>
        /// <param name="K_F">油品的特性因数，无量纲</param>
        /// <returns></returns>
        public static double Enthapy(double T, double API, double K_F)
        {
            double H = 4.1855 * (0.0533 * K_F + 0.3604) * (3.8192 + 0.2483 * API -
                0.002706 * API * API + 0.3718 * T + 0.001972 * T * API + 0.0004754 * T * T);
            return H * 1000;
        }

        /// <summary>
        /// 根据油品的两点温度、API和特性因数返回焓差，单位：J/kg
        /// </summary>
        /// <param name="T_1">油品第一点的温度，单位：℃</param>
        /// <param name="T_2">油品第二点的温度，单位：℃</param>
        /// <param name="API">油品的API指数，无量纲</param>
        /// <param name="K_F">油品的特性因数，无量纲</param>
        /// <returns></returns>
        public static double EnthapyDifference(double T_1, double T_2, double API, double K_F)
        {
            double deltaH = Enthapy(T_1, API, K_F) - Enthapy(T_2, API, K_F);
            return deltaH;
        }

        /// <summary>
        /// 根据油品的温度、相对密度和特性因数返回比热容，单位：J/(kg*℃)
        /// </summary>
        /// <param name="T">油品的温度，单位：℃</param>
        /// <param name="sp">油品的相对密度(4/20)，无量纲</param>
        /// <param name="K_F">油品的特性因数，无量纲</param>
        /// <returns></returns>
        public static double HeatCapacity(double T, double sp, double K_F)
        {
            double C_p = 4.1855 * ((0.6811 - 0.308 * (0.99417 * sp + 0.009181) + (1.8 * T + 32) *
                (0.000815 - 0.000306 * (0.99417 * sp + 0.009181))) * (0.055 * K_F + 0.35));
            return C_p * 1000;
        }

        /// <summary>
        /// 根据油品的温度、相对密度和两点粘度和温度返回动力粘度，单位：Pa*s
        /// </summary>
        /// <param name="T">油品的温度，单位：℃</param>
        /// <param name="sp">油品的相对密度(4/20)，无量纲</param>
        /// <param name="T_1">测试第一点粘度的温度，单位：℃</param>
        /// <param name="nu_1">第一点运动粘度，单位：mm2/s</param>
        /// <param name="T_2">测试第二点粘度的温度，单位：℃</param>
        /// <param name="nu_2">第二点运动粘度，单位：mm2/s</param>
        /// <returns></returns>
        public static double DynamicViscosity(double T, double sp, double T_1, double nu_1,
            double T_2, double nu_2)
        {
            double C;
            if (sp <= 0.8)
                C = 0.8;
            else if (sp >= 0.9)
                C = 0.6;
            else
                C = 2.4 - 2.0 * sp;

            double b = (Math.Log(Math.Log(nu_1 + C)) - Math.Log(Math.Log(nu_2 + C))) /
                (Math.Log(T_1 + 273) - Math.Log(T_2 + 273));
            double a = Math.Log(Math.Log(nu_1 + C)) - b * Math.Log(T_1 + 273);
            double nu = Math.Exp(Math.Exp(a + b * Math.Log(T + 273))) - C;
            double mu = Density(T, sp) * nu * 0.001;
            return mu / 1000;
        }

        /// <summary>
        /// 根据油品的两点温度返回定性温度，单位：℃
        /// </summary>
        /// <param name="t1">第一点温度，单位：℃</param>
        /// <param name="t2">第二点温度，单位：℃</param>
        /// <returns></returns>
        public static double SetTemperature(double t1, double t2)
        {
            double t_h = Math.Max(t1, t2);
            double t_c = Math.Min(t1, t2);
            double t_d = 0.4 * t_h + 0.6 * t_c;
            return t_d;
        }

        // 根据两点温度和对应的物性进行线性插值
        public static double TempInterpolation(double T_1, double V_1, double T_2, double V_2, double T)
        {
            return V_1 + (V_2 - V_1) / (T_2 - T_1) * (T - T_1);
        }

    }
}
