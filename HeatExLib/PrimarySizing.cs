using System;

namespace HeatExLib
{
    [Serializable]
    public class PrimarySizing
    {
        // 声明私有字段
        private double T_1;
        private double T_2;
        private double t_1;
        private double t_2;

        private double W_h;
        private double W_c;
        private double deltaH_h;
        private double deltaH_c;

        // 对应属性的私有字段
        private int? shellNo = 1;
        private double? duty; // 热负荷
        private double? empTotalHTCoefficient; // 预选传热系数
        private double? estimatedArea; // 预选换热面积
        private double? lMTD;
        private double? tDCorrectFactor;
        private double? cMTD;
        private double? hDuty;
        private double? cDuty;
        private double? dutyDifference;

        public int? ShellNo { get => shellNo; set => shellNo = value; }
        public double? Duty { get => duty; set => duty = value; }
        public double? EmpTotalHTCoefficient { get => empTotalHTCoefficient; set => empTotalHTCoefficient = value; }
        public double? EstimatedArea { get => estimatedArea; set => estimatedArea = value; }
        public double? LMTD { get => lMTD; set => lMTD = value; }
        public double? TDCorrectFactor { get => tDCorrectFactor; set => tDCorrectFactor = value; }
        public double? CMTD { get => cMTD; set => cMTD = value; }
        public double? CDuty { get => cDuty; set => cDuty = value; }
        public double? DutyDifference { get => dutyDifference; set => dutyDifference = value; }
        public double? HDuty { get => hDuty; set => hDuty = value; }

        /// <summary>
        /// 初始化平均传热温差计算需要的参数
        /// </summary>
        /// <param name="coldInTemp">冷侧进口温度，单位：℃</param>
        /// <param name="coldOutTemp">冷侧出口温度，单位：℃</param>
        /// <param name="hotInTemp">热侧进口温度，单位：℃</param>
        /// <param name="hotOutTemp">热侧出口温度，单位：℃</param>
        public void InitializeTD(double hotInTemp, double hotOutTemp, double coldInTemp, double coldOutTemp)
        {
            T_2 = hotOutTemp;
            t_1 = coldInTemp;
            t_2 = coldOutTemp;
            T_1 = hotInTemp;
        }

        public void InitializeDuty(double hotFlowrate, double coldFlowrate, double hotEnthalpyDifference, double coldEnthalpyDifference)
        {
            this.W_h = hotFlowrate;
            this.W_c = coldFlowrate;
            this.deltaH_h = hotEnthalpyDifference;
            this.deltaH_c = coldEnthalpyDifference;
        }

        // 计算对数平均温差LMDT，单位：℃
        public double GetLMTD()
        {
            double delta_t_h = T_1 - t_2;
            double delta_t_c = T_2 - t_1;
            return (delta_t_h - delta_t_c) / Math.Log(delta_t_h / delta_t_c);
        }

        // 计算对数平均温差校正系数，单位：无量纲
        public double GetTDCorrectFactor()
        {
            double P = (t_2 - t_1) / (T_1 - t_1);
            double R = (T_1 - T_2) / (t_2 - t_1);
            double val = Math.Pow(((R * P - 1) / (P - 1)), (1.0 / (double)ShellNo));
            double P_Z = (1 - val) / (R - val);
            double F = (Math.Sqrt(R * R + 1) / (R - 1)) * ((Math.Log((1 - P_Z) / (1 - R * P_Z))) /
                Math.Log(((2 / P_Z) - 1 - R + Math.Sqrt(R * R + 1)) / ((2 / P_Z) - 1 - R - Math.Sqrt(R * R + 1))));
            return F;
        }

        // 计算对数平均温差CMDT，单位：℃
        public double GetCMTD()
        {
            return (double)LMTD * (double)TDCorrectFactor;
        }

        // 热侧负荷，单位：W
        public double GetHDuty()
        {
            double Q_h = (W_h / 3600) * deltaH_h;
            return Q_h;
        }

        // 冷侧负荷，单位：W
        public double GetCDuty()
        {
            double Q_c = (W_c / 3600) * deltaH_c;
            return Q_c;
        }

        // 热平衡误差，单位：%
        public double GetDutyDifference()
        {
            double deltaQ = Math.Abs(((double)HDuty / (double)CDuty) - 1) * 100;
            return deltaQ;
        }

        // 热负荷，单位：W
        public double GetDuty()
        {
            return Math.Max(Math.Abs((double)HDuty), Math.Abs((double)CDuty));
        }
    }
}
