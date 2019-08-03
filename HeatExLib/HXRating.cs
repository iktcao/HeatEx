using System;

namespace HeatExLib
{
    public class HXRating
    {
        public static double Area(double duty, double meanTD, double totalHTCoefficient)
        {
            double A = duty / meanTD / totalHTCoefficient;
            return A;
        }

        /// <summary>
        /// 根据质量流量和流通面积计算质量流速，单位：kg/(m2*s)
        /// </summary>
        /// <param name="flowRate">质量流量，单位：kg/h</param>
        /// <param name="flowArea">流通面积，单位：m2</param>
        /// <returns></returns>
        public static double MassFlux(double flowRate, double flowArea)
        {
            double W = flowRate / 3600;
            double S = flowArea;
            double G = W / S;
            return G;
        }

        /// <summary>
        /// 根据管道内径、流体质量流速和动力粘度计算雷诺数，单位：无量纲
        /// </summary>
        /// <param name="diameter">管道内径，单位：mm</param>
        /// <param name="massFlux">流体质量流速，单位：kg/(m2*s)</param>
        /// <param name="dynamicViscosity">流体动力粘度，单位：Pa*s</param>
        /// <returns></returns>
        public static double Reynolds(double diameter, double massFlux, double dynamicViscosity)
        {
            double d = diameter / 1000;
            double G = massFlux;
            double mu = dynamicViscosity;
            double Re = d * G / mu;
            return Re;
        }

        /// <summary>
        /// 根据流体导热系数、比热容和动力粘度计算普朗特数，单位：无量纲
        /// </summary>
        /// <param name="thermalConductivity">介质的导热系数，单位：W/(m*K)</param>
        /// <param name="capacity">介质的比热容，单位：J/(kg*K)</param>
        /// <param name="dynamicViscosity">介质的动力粘度，单位：Pa*s</param>
        /// <returns></returns>
        public static double Prandtl(double thermalConductivity, double capacity, double dynamicViscosity)
        {
            double lambda = thermalConductivity;
            double Cp = capacity;
            double mu = dynamicViscosity;
            double Pr = Cp * mu / lambda;
            return Pr;
        }

        /// <summary>
        /// 根据管内流体雷诺数、管内径和管长计算管内传热因子，单位：无量纲
        /// </summary>
        /// <param name="reynolds">管内流体的雷诺数，单位：无量纲</param>
        /// <param name="diameter">管子内径，单位：mm</param>
        /// <param name="tubeLength">管长，单位m</param>
        /// <returns></returns>
        public static double TubeHTFactor(double reynolds, double diameter, double tubeLength)
        {
            double Re = reynolds;
            double d = diameter / 1000;
            double L = tubeLength;
            double J_H;
            if (Re <= 2100)
            {
                J_H = 1.86 * Math.Pow(Re, (1.0 / 3)) * Math.Pow((d / L), (1.0 / 3));
                return J_H;
            }
            else if (Re < 10000)
            {
                J_H = 0.116 * (Math.Pow(Re, (2.0 / 3)) - 125) * (1 + Math.Pow((d / L), (2.0 / 3)));
                return J_H;
            }
            else
            {
                J_H = 0.023 * Math.Pow(Re, 0.8);
                return J_H;
            }
        }

        /// <summary>
        /// 根据管内流体的导热系数、管外径、传热因子、普朗特数、壁温校正系数计算以管外表面积为基准的管内膜传热系数，单位：W/(m2*K)
        /// </summary>
        /// <param name="thermalConductivity">介质的导热系数，单位：W/(m*K)</param>
        /// <param name="tubeOutsideDiameter">管子外径，单位：mm</param>
        /// <param name="tubeHTFactor">管内传热因子，单位：无量纲</param>
        /// <param name="prandtl">管内流体的普朗特数，单位：无量纲</param>
        /// <param name="wallTempCorrectionFactor">管壁温校正系数</param>
        /// <returns></returns>
        public static double TubeIOFilmCoefficient(double thermalConductivity, double tubeOutsideDiameter,
            double tubeHTFactor, double prandtl, double wallTempCorrectionFactor)
        {
            double lambda = thermalConductivity;
            double d = tubeOutsideDiameter / 1000;
            double J_H = tubeHTFactor;
            double Pr = prandtl;
            double phi = wallTempCorrectionFactor;
            double h_io = lambda / d * J_H * Math.Pow(Pr, (1.0 / 3)) * phi;
            return h_io;
        }

        /// <summary>
        /// 根据壳径、中心管排数、管外径和折流板间距计算壳程流通面积，单位：m2
        /// </summary>
        /// <param name="shellInsideDiameter">壳径，单位：mm</param>
        /// <param name="centerTubePasses">中心管排数，单位：无量纲</param>
        /// <param name="tubeOutsideDiameter">管外径，单位：mm</param>
        /// <param name="buffleSpacing">折流板间距，单位：mm</param>
        /// <returns></returns>
        public static double ShellFlowArea(double shellInsideDiameter, int centerTubePasses, double tubeOutsideDiameter, double buffleSpacing)
        {
            double D_i = shellInsideDiameter / 1000;
            int n = centerTubePasses;
            double d_o = tubeOutsideDiameter / 1000;
            double B = buffleSpacing / 1000;
            double delta;
            if (D_i <= 700)
            {
                delta = 0.006;
            }
            else if (D_i <= 900)
            {
                delta = 0.008;
            }
            else if (D_i <= 1500)
            {
                delta = 0.01;
            }
            else
            {
                delta = 0.012;
            }
            double S = (D_i - n * d_o) * (B - delta);
            return S;
        }

        /// <summary>
        /// 根据管心距、管外径和管子排列方式计算管子的当量直径，单位：mm
        /// </summary>
        /// <param name="tubePitch">管心距，单位：mm</param>
        /// <param name="tubeOutsideDiameter">管外径，单位：mm</param>
        /// <param name="tubeLayout">管子排列方式，单位：°</param>
        /// <returns></returns>
        public static double EquivalentDiameter(double tubePitch, double tubeOutsideDiameter, string tubeLayout)
        {
            double p_t = tubePitch;
            double d_o = tubeOutsideDiameter;
            double d_e;
            if (tubeLayout == "45°" || tubeLayout == "90°")
            {
                d_e = 4 * (p_t * p_t - Math.PI / 4 * d_o * d_o) / (Math.PI * d_o);
                return d_e;
            }
            else
            {
                d_e = 4 * (Math.Sqrt(3.0) / 4.0 * p_t * p_t - Math.PI / 8 * d_o * d_o) / (0.5 * Math.PI * d_o);
                return d_e;
            }
        }

        /// <summary>
        /// 根据壳径查询旁路挡板[传热校正系数，压力校正系数]，单位：无量纲
        /// </summary>
        /// <param name="shellID">壳径，单位：mm</param>
        /// <returns></returns>
        public static double[] BypassCorrectionFactor(double shellID)
        {
            double[] shellIDs = new double[] { 325, 400, 500, 600, 700, 800, 900,
                1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800 };
            double[] HTFactors = new double[] { 1.30, 1.26, 1.23, 1.20, 1.18, 1.17, 1.15,
                1.14, 1.13, 1.12, 1.11, 1.10, 1.09, 1.08, 1.07, 1.06 };
            double[] DPFactors = new double[] { 1.90, 1.87, 1.85, 1.73, 1.64, 1.58, 1.52,
                1.51, 1.50, 1.45, 1.40, 1.35, 1.30, 1.25, 1.20, 1.15 };
            double[] factors = new double[2];
            double ID = shellID;

            for (int i = 0; i < shellIDs.Length; i++)
            {
                if (ID <= shellIDs[i])
                {
                    factors[0] = HTFactors[i];
                    factors[1] = DPFactors[i];
                    return factors;
                }
            }
            factors[0] = HTFactors[HTFactors.Length - 1];
            factors[1] = DPFactors[HTFactors.Length - 1];
            return factors;
        }

        /// <summary>
        /// 根据壳程流体雷诺数、弓缺百分比计算壳程传热因子，单位：无量纲
        /// </summary>
        /// <param name="reynolds">壳程流体雷诺数，单位：无量纲</param>
        /// <param name="cut">弓形折流板缺圆高度百分数，单位：%</param>
        /// <param name="tubeLayout">管子排列方式，单位：°</param>
        /// <returns></returns>
        public static double ShellHTFactor(double reynolds, double cut, string tubeLayout)
        {
            double Re = reynolds;
            double Z = cut;
            double J;
            if (tubeLayout == "45°" || tubeLayout == "90°")
            {
                if (Re <= 200)
                    J = 0.641 * Math.Pow(Re, 0.46) * ((Z - 15) / 10) + 0.731 * Math.Pow(Re, 0.473) * ((25 - Z) / 10);
                else if (Re < 1000)
                    J = 0.491 * Math.Pow(Re, 0.51) * ((Z - 15) / 10) + 0.673 * Math.Pow(Re, 0.49) * ((25 - Z) / 10);
                else
                    J = 0.378 * Math.Pow(Re, 0.554) * ((Z - 15) / 10) + 0.41 * Math.Pow(Re, 0.5634) * ((25 - Z) / 10);
            }
            else
            {
                if (Re <= 200)
                    J = 0.641 * Math.Pow(Re, 0.46) * ((Z - 15) / 10) + 0.713 * Math.Pow(Re, 0.473) * ((25 - Z) / 10);
                else if (Re < 1000)
                    J = 0.491 * Math.Pow(Re, 0.51) * ((Z - 15) / 10) + 0.673 * Math.Pow(Re, 0.49) * ((25 - Z) / 10);
                else
                    J = 0.350 * Math.Pow(Re, 0.55) * ((Z - 15) / 10) + 0.473 * Math.Pow(Re, 0.539) * ((25 - Z) / 10);
            }
            return J;
        }

        public static double WallTemperatue(ProcessStream hotStream, ProcessStream coldStream, Geometry geometry)
        {
            double h_o = (double)geometry.TubeOutFilmCoefficient;
            double h_io = (double)geometry.TubeIOFilmCoefficient;
            double t_W;
            if (coldStream.Location == "管程")
            {
                double t_oD = (double)hotStream.SetTemperature;
                double t_iD = (double)coldStream.SetTemperature;
                t_W = (h_o / (h_o + h_io)) * (t_oD - t_iD) + t_iD;
            }
            else
            {
                double t_iD = (double)hotStream.SetTemperature;
                double t_oD = (double)coldStream.SetTemperature;
                t_W = (h_io / (h_o + h_io)) * (t_iD - t_oD) + t_oD;
            }
            return t_W;
        }

        /// <summary>
        /// 根据定性温度下的动力粘度和壁温下的动力粘度计算壁温校正因子，单位：无量纲
        /// </summary>
        /// <param name="dynamicViscosity">定性温度下的动力粘度，单位：Pa*s</param>
        /// <param name="dynamicViscosityAtWT">壁温下的动力粘度，单位：Pa*s</param>
        /// <returns></returns>
        public static double WallTempCorrectionFactor(double dynamicViscosity, double dynamicViscosityAtWT)
        {
            double mu = dynamicViscosity;
            double mu_W = dynamicViscosityAtWT;
            double phi = Math.Pow((mu / mu_W), 0.14);
            return phi;
        }

        /// <summary>
        /// 计算换热器的总传热系数[总传热系数，清洁总传热系数]，单位：W/(m2*K)
        /// </summary>
        /// <param name="tubeInsideDiameter">换热管内径，单位：mm</param>
        /// <param name="tubeOutsideDiameter">换热管外径，单位：mm</param>
        /// <param name="TubeIOFilmCoefficient">管内流体膜传热系数（以管外表面积为基准），单位：W/(m2*K)</param>
        /// <param name="TubeOutFilmCoefficient">管外流体膜传热系数（以管外表面积为基准），单位：W/(m2*K)</param>
        /// <param name="tubeFoulResistance">管内流体结垢热阻（以管内表面积为基准），单位：m2*K/W</param>
        /// <param name="shellFoulResistance">管外流体结垢热阻（以管外表面积为基准），单位：m2*K/W</param>
        /// <param name="tubeMaterial">换热管材质</param>
        /// <returns></returns>
        public static double[] TotalHTCoefficient(double tubeInsideDiameter,
            double tubeOutsideDiameter, double TubeIOFilmCoefficient,
            double TubeOutFilmCoefficient, double tubeFoulResistance,
            double shellFoulResistance, string tubeMaterial)
        {
            double d_i = tubeInsideDiameter / 1000;
            double d_o = tubeOutsideDiameter / 1000;
            double h_io = TubeIOFilmCoefficient;
            double h_o = TubeOutFilmCoefficient;
            double r_i = tubeFoulResistance;
            double r_o = shellFoulResistance;
            string material = tubeMaterial;

            double[] Ks = new double[2];
            double lambda_W;
            if (material == "碳钢")
                lambda_W = 46.7;
            else if (material == "铬钼钢")
                lambda_W = 43.3;
            else
                lambda_W = 19.0;

            double r_p = d_o / 2 / lambda_W * Math.Log(d_o / d_i);
            //double K = 1 / (1 / h_io + Math.Pow(d_o / d_i, 2) * r_i + 1 / h_o + r_o + r_p);
            Ks[0] = 1 / (1 / h_io + d_o / d_i * r_i + 1 / h_o + r_o + r_p);
            Ks[1] = 1 / (1 / h_io + 1 / h_o + r_p);
            return Ks;
        }

        /// <summary>
        /// 根据换热管内介质的雷诺数计算摩擦因子，单位：无量纲
        /// </summary>
        /// <param name="reynolds">管内介质的雷诺数，单位：无量纲</param>
        /// <returns></returns>
        public static double TubeFrictionFactor(double reynolds)
        {
            double Re_i = reynolds;
            if (Re_i < 1000)
                return 67.63 * Math.Pow(Re_i, -0.9873);
            else if (Re_i <= 100000)
                return 0.4513 * Math.Pow(Re_i, -0.2653);
            else
                return 0.2864 * Math.Pow(Re_i, -0.2258);
        }

        /// <summary>
        /// 计算管程直管压力降，单位：kPa
        /// </summary>
        /// <param name="tubeInsideMassFlux">管内流体质量流速，单位：kg/(m2*s)</param>
        /// <param name="tubeDensity">管内流体在定性温度下的密度，单位：kg/m3</param>
        /// <param name="tubeLength">管长，单位：m</param>
        /// <param name="tubePasses">管程数，单位：无量纲</param>
        /// <param name="tubeInsideDiameter">换热管内径，单位：mm</param>
        /// <param name="tubeFrictionFactor">管内摩擦因子，单位：无量纲</param>
        /// <param name="tubeWallFactor">管内壁温校正因子，单位：无量纲</param>
        /// <returns></returns>
        public static double TubeStraightDP(double tubeInsideMassFlux, double tubeDensity,
            double tubeLength, int tubePasses, double tubeInsideDiameter,
            double tubeFrictionFactor, double tubeWallFactor)
        {
            double G_i = tubeInsideMassFlux;
            double rho_iD = tubeDensity;
            double L = tubeLength;
            int N_tp = tubePasses;
            double d_i = tubeInsideDiameter / 1000;
            double f_i = tubeFrictionFactor;
            double phi_i = tubeWallFactor;
            double DP_i = G_i * G_i * L * N_tp * f_i / (2 * rho_iD * d_i * phi_i);
            return DP_i / 1000;
        }

        /// <summary>
        /// 计算管程回弯压力降，单位：kPa
        /// </summary>
        /// <param name="tubeInsideMassFlux">管内流体质量流速，单位：kg/(m2*s)</param>
        /// <param name="tubeDensity">管内流体在定性温度下的密度，单位：kg/m3</param>
        /// <param name="tubePasses">管程数，单位：无量纲</param>
        /// <returns></returns>
        public static double TubeBendDP(double tubeInsideMassFlux, double tubeDensity,
            int tubePasses)
        {
            double G_i = tubeInsideMassFlux;
            double rho_iD = tubeDensity;
            int N_tp = tubePasses;
            double DP_r = G_i * G_i * 4 * N_tp / (2 * rho_iD);
            return DP_r / 1000;
        }

        /// <summary>
        /// 计算进出口管嘴压力降，单位：kPa
        /// </summary>
        /// <param name="flowrate">流体流量，单位：kg/h</param>
        /// <param name="density">流体在定性温度下的密度，单位：kg/m3</param>
        /// <param name="inletNozzle">进口管嘴尺寸，单位：mm</param>
        /// <param name="outletNozzle">出口管嘴尺寸，单位：mm</param>
        /// <returns></returns>
        public static double NozzleDP(double flowrate, double density,
            double inletNozzle, double outletNozzle)
        {
            double W = flowrate / 3600;
            double rho = density;
            double d_N1 = inletNozzle / 1000;
            double d_N2 = outletNozzle / 1000;
            double G_N1 = W / (Math.PI * d_N1 * d_N1 / 4);
            double G_N2 = W / (Math.PI * d_N2 * d_N2 / 4);
            double DP_N = (G_N1 * G_N1 + 0.5 * G_N2 * G_N2) / (2 * rho);
            return DP_N / 1000;
        }

        /// <summary>
        /// 根据流体结垢热阻查询压力降结垢校正系数[管程，壳程]，单位：无量纲
        /// </summary>
        /// <param name="foulResistance">流体的结垢热阻，单位：m2*K/W</param>
        /// <returns></returns>
        public static double[] FoulDPCorrectionFactor(double foulResistance)
        {
            double[] frs = new double[] { 0, 0.00017, 0.00034, 0.00043, 0.00052, 0.00069,
                0.00086, 0.00129, 0.00172 };
            double[] Fis = new double[] { 1.00, 1.20, 1.35, 1.40, 1.45, 1.50, 1.60,
                1.70, 1.80 };
            double[] Fos = new double[] { 1.00, 1.20, 1.30, 1.35, 1.40, 1.45, 1.50,
                1.65, 1.75 };
            double[] factors = new double[2];
            double fr = foulResistance;

            for (int i = 0; i < frs.Length; i++)
            {
                if (fr <= frs[i])
                {
                    factors[0] = Fis[i];
                    factors[1] = Fos[i];
                    return factors;
                }
            }
            factors[0] = Fis[Fis.Length - 1];
            factors[1] = Fos[Fos.Length - 1];
            return factors;
        }


        /// <summary>
        /// 根据壳程介质的雷诺数、管子排列方式和折流板弓缺计算管程摩擦因子，单位：无量纲
        /// </summary>
        /// <param name="reynolds">壳程流体的雷诺数，单位：无量纲</param>
        /// <param name="tubeLayout">管子排列方式，单位：°</param>
        /// <param name="buffleCut">折流板弓缺，单位：%</param>
        /// <returns></returns>
        public static double ShellFrictionFactor(double reynolds, string tubeLayout, double buffleCut)
        {
            double Re_o = reynolds;
            double Z = buffleCut;
            double f_o;
            if (tubeLayout == "45°" || tubeLayout == "90°")
            {
                if (Re_o <= 100)
                    f_o = 119.3 * Math.Pow(Re_o, -0.93);
                else if (Re_o <= 1500)
                    f_o = 0.402 + 3.1 / Re_o + 3.51E4 * Math.Pow(Re_o, -2) - 6.85E6 * Math.Pow(Re_o, -3) + 4.175E8 * Math.Pow(Re_o, -4);
                else if (Re_o <= 15000)
                    f_o = 0.731 * Math.Pow(Re_o, -0.0774);
                else
                    f_o = 1.52 * Math.Pow(Re_o, -0.153);
            }
            else
            {
                if (Re_o <= 100)
                    f_o = 207.4 * Math.Pow(Re_o, -1.106);
                else if (Re_o <= 1500)
                    f_o = 0.354 + 240.6 / Re_o - 8.28E4 * Math.Pow(Re_o, -2) + 1.852E7 * Math.Pow(Re_o, -3) - 1.107E8 * Math.Pow(Re_o, -4);
                else if (Re_o <= 15000)
                    f_o = 1.148 * Math.Pow(Re_o, -0.1475);
                else
                    f_o = 1.52 * Math.Pow(Re_o, -0.153);
            }
            f_o = f_o * (35 / (Z + 10));
            return f_o;
        }

        /// <summary>
        /// 计算管程直管压力降，单位：kPa
        /// </summary>
        /// <param name="shellMassFlux">壳程流体质量流速，单位：kg/(m2*s)</param>
        /// <param name="shellDensity">壳程流体在定性温度下的密度，单位：kg/m3</param>
        /// <param name="shellInsideDiameter">壳径，单位：mm</param>
        /// <param name="tubeLength">管长，单位：m</param>
        /// <param name="buffleSpacing">折流板间距，单位：mm</param>
        /// <param name="equivalentDiameter">管子的当量直径，单位：mm</param>
        /// <param name="shellFrictionFactor">管程摩擦因子，单位：无量纲</param>
        /// <param name="shellWallFactor">管外壁温校正因子，单位：无量纲</param>
        /// <param name="bypassCorrectionFactor">旁路挡板压力校正系数，单位：无量纲</param>
        /// <returns></returns>
        public static double ShellBundleDP(double shellMassFlux, double shellDensity,
            double shellInsideDiameter, double tubeLength, double buffleSpacing,
            double equivalentDiameter, double shellFrictionFactor,
            double shellWallFactor, double bypassCorrectionFactor)
        {
            double G_o = shellMassFlux;
            double rho_oD = shellDensity;
            double D_s = shellInsideDiameter / 1000;
            double L = tubeLength;
            double B = buffleSpacing / 1000;
            double d_e = equivalentDiameter / 1000;
            double f_o = shellFrictionFactor;
            double phi_o = shellWallFactor;
            double epsilon_dp = bypassCorrectionFactor;

            double N_b = Math.Ceiling(L / B) - 1;
            double DP_o = G_o * G_o * D_s * (N_b + 1) * f_o * epsilon_dp / (2 * rho_oD * d_e * phi_o);
            return DP_o / 1000;
        }

        /// <summary>
        /// 计算壳程导流板或导流筒压力降
        /// </summary>
        /// <param name="shellFlowrate">壳程质量流量，单位：kg/h</param>
        /// <param name="shellInletNozzle">壳程进口管嘴尺寸，单位：mm</param>
        /// <param name="shellDensity">壳程流体密度，单位：kg/m3</param>
        /// <param name="cylinderDPFactor">导流板或导流筒的压力降系数，单位：无量纲</param>
        /// <returns></returns>
        public static double ShellCylinderDP(double shellFlowrate, double shellInletNozzle,
            double shellDensity, double cylinderDPFactor)
        {
            double W_o = shellFlowrate / 3600;
            double d_No1 = shellInletNozzle / 1000;
            double rho_oD = shellDensity;
            double epsilon_IP = cylinderDPFactor;

            double G_No = W_o / (Math.PI * d_No1 * d_No1 / 4);
            double DP_ro = G_No * G_No * epsilon_IP / (2 * rho_oD);
            return DP_ro / 1000;
        }
    }
}
