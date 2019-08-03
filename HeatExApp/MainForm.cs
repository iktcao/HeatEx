using HeatExLib;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace HeatExApp
{
    public partial class MainForm : Form
    {
        // 全局变量声明
        private ProcessStream hotStream = new ProcessStream();
        private ProcessStream coldStream = new ProcessStream();
        private PrimarySizing primarySizing = new PrimarySizing();
        private Geometry geometry = new Geometry();

        private BindingSource hotBindingSource = new BindingSource();
        private BindingSource coldBindingSource = new BindingSource();
        private BindingSource primarySizingBindingSource = new BindingSource();
        private BindingSource geometryBindingSource = new BindingSource();
        private string presentFile = "";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //// 临时赋值（供测试用）
            //ReadPreset(tlpProcess);
            //ReadPreset(tlpProperty);
            //ReadPreset(tlpTubeGeometry);
            //ReadPreset(tlpShellGeometry);
            //primarySizing.ShellNo = Int32.Parse(txtShellNo.Text);
            //primarySizing.EmpTotalHTCoefficient = double.Parse(txtEmpTotalHTCoefficient.Text);

            // 添加数据绑定
            hotBindingSource.DataSource = hotStream;
            coldBindingSource.DataSource = coldStream;
            primarySizingBindingSource.DataSource = primarySizing;
            geometryBindingSource.DataSource = geometry;

            AddDataBindings(ref tlpProcess);
            AddDataBindings(ref tlpProperty);
            AddDataBindings(ref tlpPrimarySizingInput);
            AddDataBindings(ref tlpPrimarySizingResult);
            AddDataBindings(ref tlpTubeGeometry);
            AddDataBindings(ref tlpShellGeometry);
            AddDataBindings(ref tlpRatingResult);
            AddDataBindings(ref tlpInterResult);
        }

        // 根据基础物性数据计算定性温度下的物性
        private void BtnBasicProperty_Click(object sender, EventArgs e)
        {
            tabControl1.Focus();
            try
            {
                // 计算定性温度
                double hotSetTemp = PhyProperties.SetTemperature((double)hotStream.InletTemperature,
                    (double)hotStream.OutletTemperature);
                double coldSetTemp = PhyProperties.SetTemperature((double)coldStream.InletTemperature,
                    (double)coldStream.OutletTemperature);
                hotStream.SetTemperature = hotSetTemp;
                coldStream.SetTemperature = coldSetTemp;

                // 弹出基础物性计算窗口
                BasicPropForm equaCalcForm = new BasicPropForm(ref hotStream, ref coldStream);
                equaCalcForm.ShowDialog();

                // 刷新数据显示
                hotBindingSource.ResetBindings(false);
                coldBindingSource.ResetBindings(false);
            }
            catch
            {
                MessageBox.Show("Oops!\n请检查数据输入！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // 换热器初步选型
        private void BtnPrimarySizing_Click(object sender, EventArgs e)
        {
            tabControl1.Focus();
            try
            {
                primarySizing.InitializeDuty((double)hotStream.Flowrate,
                (double)coldStream.Flowrate,
                (double)hotStream.EnthalpyDifference,
                (double)coldStream.EnthalpyDifference);

                primarySizing.InitializeTD((double)hotStream.InletTemperature,
                    (double)hotStream.OutletTemperature,
                    (double)coldStream.InletTemperature,
                    (double)coldStream.OutletTemperature);

                primarySizing.LMTD = primarySizing.GetLMTD();
                primarySizing.TDCorrectFactor = primarySizing.GetTDCorrectFactor();
                primarySizing.CMTD = primarySizing.GetCMTD();
                primarySizing.HDuty = primarySizing.GetHDuty();
                primarySizing.CDuty = primarySizing.GetCDuty();
                primarySizing.DutyDifference = primarySizing.GetDutyDifference();
                primarySizing.Duty = primarySizing.GetDuty();

                double area = HXRating.Area((double)primarySizing.Duty,
                    (double)primarySizing.CMTD, (double)primarySizing.EmpTotalHTCoefficient);
                primarySizing.EstimatedArea = area;

                // 刷新控件显示值
                primarySizingBindingSource.ResetBindings(false);
            }
            catch
            {
                MessageBox.Show("Oops!\n请检查数据输入！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 换热器核算
        private void BtnRating_Click(object sender, EventArgs e)
        {
            tabControl1.Focus();
            try
            {
                // 定义壳程物流和管程物流
                ProcessStream shellStream;
                ProcessStream tubeStream;
                if (hotStream.Location == "管程")
                {
                    tubeStream = hotStream;
                    shellStream = coldStream;
                }
                else
                {
                    tubeStream = coldStream;
                    shellStream = hotStream;
                }

                // 计算管内流体质量流速
                double W_i = (double)tubeStream.Flowrate;
                double S_i = (double)geometry.GetTubeFlowArea();
                geometry.TubeFlowArea = S_i;
                double G_i = HXRating.MassFlux(W_i, S_i);
                geometry.TubeInsideMassFlux = G_i;

                // 计算管内流体的雷诺准数
                double d_i = (double)geometry.TubeInsideDiameter;
                double mu_iD = (double)tubeStream.DynamicViscosity;
                double Re_i = HXRating.Reynolds(d_i, G_i, mu_iD);
                geometry.TubeReynolds = Re_i;

                // 计算管内流体的普朗特数
                double Cp_iD = (double)tubeStream.Capacity;
                double lambda_iD = (double)tubeStream.ThermalConductivity;
                double Pr_i = HXRating.Prandtl(lambda_iD, Cp_iD, mu_iD);
                geometry.TubePrandtl = Pr_i;

                // 计算管内传热因子
                double L = (double)geometry.TubeLength;
                double J_Hi = HXRating.TubeHTFactor(Re_i, d_i, L);
                geometry.TubeHTFactor = J_Hi;

                // 计算以管外表面积为基准的管内膜传热系数
                double phi_i = 1;
                double d_o = (double)geometry.TubeOutsideDiameter;
                // 移至下方迭代区
                //double h_io = HXRating.TubeIOFilmCoefficient(lambda_iD, d_o, J_Hi, Pr_i, phi_i);
                //geometry.TubeIOFilmCoefficient = h_io;

                // 计算壳程流通面积
                int n_c = (int)geometry.CenterTubePasses;
                double B = (double)geometry.BuffleSpacing;
                double D_i = (double)geometry.ShellInsideDiameter;
                double S_o = HXRating.ShellFlowArea(D_i, n_c, d_o, B);
                geometry.ShellFlowArea = S_o;

                // 计算壳程流体质量流速
                double W_o = (double)shellStream.Flowrate;
                double G_o = HXRating.MassFlux(W_o, S_o);
                geometry.ShellMassFlux = G_o;

                // 计算管子的当量直径
                double p_t = (double)geometry.TubePitch;
                string tubeLayout = geometry.TubeLayout;
                double d_e = HXRating.EquivalentDiameter(p_t, d_o, tubeLayout);
                geometry.EquivalentDiameter = d_e;

                // 计算壳程流体的雷诺数
                double mu_oD = (double)shellStream.DynamicViscosity;
                double Re_o = HXRating.Reynolds(d_e, G_o, mu_oD);
                geometry.ShellReynolds = Re_o;

                // 计算壳程流体的普朗特数
                double Cp_oD = (double)shellStream.Capacity;
                double lambda_oD = (double)shellStream.ThermalConductivity;
                double Pr_o = HXRating.Prandtl(lambda_oD, Cp_oD, mu_oD);
                geometry.ShellPrandtl = Pr_o;

                // 计算管外传热因子
                double Z = (double)geometry.BuffleCut;
                double J_Ho = HXRating.ShellHTFactor(Re_o, Z, tubeLayout);
                geometry.ShellHTFactor = J_Ho;

                // 计算以光管外表面积为基准的管外膜传热系数
                double epsilon_h = HXRating.BypassCorrectionFactor(D_i)[0];
                double phi_o = 1;
                // 移至下方迭代区
                //double h_o = HXRating.TubeIOFilmCoefficient(lambda_oD, d_e, J_Ho, Pr_o, phi_o) * epsilon_h;
                //geometry.TubeOutFilmCoefficient = h_o;

                // 迭代区
                double err_i;
                double err_o;
                double t_W;
                double h_io;
                double h_o;
                do
                {
                    // 计算以管外表面积为基准的管内膜传热系数
                    h_io = HXRating.TubeIOFilmCoefficient(lambda_iD, d_o, J_Hi, Pr_i, phi_i);
                    geometry.TubeIOFilmCoefficient = h_io;

                    // 计算以光管外表面积为基准的管外膜传热系数
                    h_o = HXRating.TubeIOFilmCoefficient(lambda_oD, d_e, J_Ho, Pr_o, phi_o) * epsilon_h;
                    geometry.TubeOutFilmCoefficient = h_o;

                    // 计算管壁温度
                    t_W = HXRating.WallTemperatue(hotStream, coldStream, geometry);
                    geometry.WallTemperature = t_W;

                    // 计算壁温校正因子
                    double mu_iW;
                    double mu_oW;
                    if (hotStream.PropertyMethod1)
                    {
                        mu_iW = PhyProperties.DynamicViscosity(t_W,
                        (double)tubeStream.Sp,
                        (double)tubeStream.ViscosityTemp1,
                        (double)tubeStream.Viscosity1,
                        (double)tubeStream.ViscosityTemp2,
                        (double)tubeStream.Viscosity2);
                        mu_oW = PhyProperties.DynamicViscosity(t_W,
                        (double)shellStream.Sp,
                        (double)shellStream.ViscosityTemp1,
                        (double)shellStream.Viscosity1,
                        (double)shellStream.ViscosityTemp2,
                        (double)shellStream.Viscosity2);
                    }
                    else
                    {
                        mu_iW = PhyProperties.DynamicViscosity(t_W,
                        (double)tubeStream.Sp,
                        (double)tubeStream.InletTemperature,
                        (double)tubeStream.InletDynamicViscosity,
                        (double)tubeStream.OutletTemperature,
                        (double)tubeStream.OutletDynamicViscosity);
                        mu_oW = PhyProperties.DynamicViscosity(t_W,
                        (double)shellStream.Sp,
                        (double)shellStream.InletTemperature,
                        (double)shellStream.InletDynamicViscosity,
                        (double)shellStream.OutletTemperature,
                        (double)shellStream.OutletDynamicViscosity);
                    }
                    double phi_iF = HXRating.WallTempCorrectionFactor(mu_iD, mu_iW);
                    geometry.TubeWallFactor = phi_iF;
                    double phi_oF = HXRating.WallTempCorrectionFactor(mu_oD, mu_oW);
                    geometry.ShellWallFactor = phi_oF;
                    err_i = Math.Abs(phi_iF - phi_i);
                    err_o = Math.Abs(phi_oF - phi_o);
                    phi_i = phi_iF;
                    phi_o = phi_oF;
                } while (err_i > 0.0001 || err_o > 0.0001);

                // 校正膜传热系数
                h_io = h_io * phi_i;
                geometry.TubeIOFilmCoefficient = h_io;
                h_o = h_o * phi_o;
                geometry.TubeOutFilmCoefficient = h_o;

                // 计算总传热系数
                double r_i = (double)tubeStream.FoulResistance;
                double r_o = (double)shellStream.FoulResistance;
                string material = geometry.TubeMaterial;
                double[] Ks = new double[2];
                Ks = HXRating.TotalHTCoefficient(d_i, d_o, h_io, h_o, r_i, r_o, material);
                geometry.TotalHTCoefficient = Ks[0];
                geometry.CleanHTCoefficient = Ks[1];
                geometry.KDifference = ((Ks[0] / primarySizing.EmpTotalHTCoefficient - 1) * 100);

                // 计算换热面积余量
                BtnPrimarySizing_Click(btnPrimarySizing, EventArgs.Empty);
                double Q = (double)primarySizing.Duty;
                double A = HXRating.Area(Q, (double)primarySizing.CMTD, (double)geometry.TotalHTCoefficient);
                geometry.RequiredArea = A;
                double C_F = ((double)geometry.ActualArea / A - 1) * 100;
                geometry.OverDesign = C_F;

                // 计算管程直管压力降
                double f_i = HXRating.TubeFrictionFactor(Re_i);
                double rho_iD = (double)tubeStream.Density;
                int N_tp = (int)geometry.TubePasses;
                double DP_i = HXRating.TubeStraightDP(G_i, rho_iD, L, N_tp, d_i, f_i, phi_i);
                geometry.TubeStraightDP = DP_i;

                // 计算管程回弯压力降
                double DP_r = HXRating.TubeBendDP(G_i, rho_iD, N_tp);
                geometry.TubeBendDP = DP_r;

                // 计算管程进出口管嘴压力降
                double d_Ni1 = (double)geometry.TubeInletNozzle;
                double d_Ni2 = (double)geometry.TubeOutletNozzle;
                double DP_Ni = HXRating.NozzleDP(W_i, rho_iD, d_Ni1, d_Ni2);
                geometry.TubeNozzleDP = DP_Ni;

                // 计算管程总压力降
                double F_i = HXRating.FoulDPCorrectionFactor(r_i)[0];
                double DP_t = (DP_i + DP_r) * F_i + DP_Ni;
                geometry.TubeDP = DP_t;

                // 计算壳程管束压力降
                double f_o = HXRating.ShellFrictionFactor(Re_o, tubeLayout, Z);
                double rho_oD = (double)shellStream.Density;
                double epsilon_dp = HXRating.BypassCorrectionFactor(D_i)[1];
                double DP_o = HXRating.ShellBundleDP(G_o, rho_oD, D_i, L, B, d_e, f_o, phi_o, epsilon_dp);
                geometry.ShellBundleDP = DP_o;

                // 计算壳程导流板或导流筒压力降
                double d_No1 = (double)geometry.ShellInletNozzle;
                double DP_ro = HXRating.ShellCylinderDP(W_o, d_No1, rho_oD, 5.0);
                geometry.ShellCylinderDP = DP_ro;

                // 计算壳程进出口压力降
                double d_No2 = (double)geometry.ShellOutletNozzle;
                double DP_No = HXRating.NozzleDP(W_o, rho_oD, d_No1, d_No2);
                geometry.ShellNozzleDP = DP_No;

                // 计算壳程总压力降
                double F_o = HXRating.FoulDPCorrectionFactor(r_o)[1];
                double DP_s = DP_o * F_o + DP_ro + DP_No;
                geometry.ShellDP = DP_s;

                // 刷新控件显示值
                geometryBindingSource.ResetBindings(false);
            }
            catch
            {
                MessageBox.Show("Oops!\n请检查数据输入！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 管壳程下拉框切换事件
        private void CmbHotSide_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = cmbHotLocation.SelectedItem.ToString();
            if (str == "壳程")
            {
                txtColdLocation.Text = "管程";
                coldStream.Location = "管程";
            }
            else if (str == "管程")
            {
                txtColdLocation.Text = "壳程";
                coldStream.Location = "壳程";
            }
            hotStream.Location = str;
        }

        // 根据表格布局控件添加数据绑定
        private void AddDataBindings(ref TableLayoutPanel tlp)
        {
            foreach (Control control in tlp.Controls)
            {

                if (control is TextBox || control is ComboBox)
                {
                    string controlName = control.Name;
                    if (controlName.Contains("Hot"))
                    {
                        string propName = controlName.Substring(6, (controlName.Length - 6));
                        control.DataBindings.Add("Text", hotBindingSource, propName, true, DataSourceUpdateMode.OnValidation, "");
                    }
                    else if (controlName.Contains("Cold"))
                    {
                        string propName = controlName.Substring(7, (controlName.Length - 7));
                        control.DataBindings.Add("Text", coldBindingSource, propName, true, DataSourceUpdateMode.OnValidation, "");
                    }
                    else
                    {
                        string propName = controlName.Substring(3, (controlName.Length - 3));
                        if (tlp.Name == "tlpPrimarySizingInput" || tlp.Name == "tlpPrimarySizingResult")
                            control.DataBindings.Add("Text", primarySizingBindingSource, propName, true, DataSourceUpdateMode.OnValidation, "");
                        else
                        {
                            control.DataBindings.Add("Text", geometryBindingSource, propName, true, DataSourceUpdateMode.OnValidation, "");
                        }
                    }
                }
            }
        }

        // 临时赋值 (供测试）
        //public void ReadPreset(TableLayoutPanel tlp)
        //{
        //    foreach (Control control in tlp.Controls)
        //    {
        //        if (control is TextBox || control is ComboBox)
        //        {
        //            string controlName = control.Name;
        //            string str = control.Text;
        //            double dblValue;

        //            if (controlName.Contains("Hot"))
        //            {
        //                string propName = controlName.Substring(6, (controlName.Length - 6));
        //                if (double.TryParse(str, out dblValue))
        //                    hotStream.GetType().GetProperty(propName).SetValue(hotStream, dblValue);
        //                else
        //                    hotStream.GetType().GetProperty(propName).SetValue(hotStream, str);
        //            }
        //            else if (controlName.Contains("Cold"))
        //            {
        //                string propName = controlName.Substring(7, (controlName.Length - 7));
        //                if (double.TryParse(str, out dblValue))
        //                    coldStream.GetType().GetProperty(propName).SetValue(coldStream, dblValue);
        //                else
        //                    coldStream.GetType().GetProperty(propName).SetValue(coldStream, str);
        //            }
        //            else
        //            {
        //                string propName = controlName.Substring(3, (controlName.Length - 3));
        //                if (propName == "TubePasses" || propName == "TubeNo")
        //                {
        //                    geometry.GetType().GetProperty(propName).SetValue(geometry, int.Parse(str));
        //                }
        //                else if (double.TryParse(str, out dblValue))
        //                    geometry.GetType().GetProperty(propName).SetValue(geometry, dblValue);
        //                else
        //                    geometry.GetType().GetProperty(propName).SetValue(geometry, str);
        //            }
        //        }
        //    }
        //}

        private void TsbAbout_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void TsbOpen_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = openFileDialog1.FileName;
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        hotStream = (ProcessStream)formatter.Deserialize(fileStream);
                        coldStream = (ProcessStream)formatter.Deserialize(fileStream);
                        primarySizing = (PrimarySizing)formatter.Deserialize(fileStream);
                        geometry = (Geometry)formatter.Deserialize(fileStream);
                        fileStream.Close();
                    }
                    hotBindingSource.DataSource = hotStream;
                    coldBindingSource.DataSource = coldStream;
                    primarySizingBindingSource.DataSource = primarySizing;
                    geometryBindingSource.DataSource = geometry;

                    hotBindingSource.ResetBindings(false);
                    coldBindingSource.ResetBindings(false);
                    primarySizingBindingSource.ResetBindings(false);
                    geometryBindingSource.ResetBindings(false);

                    presentFile = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void TsbSaveAs_Click(object sender, EventArgs e)
        {
            tabControl1.Focus();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = saveFileDialog1.FileName;
                    using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fileStream, hotStream);
                        formatter.Serialize(fileStream, coldStream);
                        formatter.Serialize(fileStream, primarySizing);
                        formatter.Serialize(fileStream, geometry);
                        fileStream.Flush();
                        fileStream.Close();
                    }

                    presentFile = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void TsbSave_Click(object sender, EventArgs e)
        {
            tabControl1.Focus();
            try
            {
                string fileName = presentFile;
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                    using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fileStream, hotStream);
                        formatter.Serialize(fileStream, coldStream);
                        formatter.Serialize(fileStream, primarySizing);
                        formatter.Serialize(fileStream, geometry);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {
                    TsbSaveAs_Click(tsbSaveAs, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
