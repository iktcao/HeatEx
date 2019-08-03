using HeatExLib;
using System;
using System.Windows.Forms;

namespace HeatExApp
{
    public partial class BasicPropForm : Form
    {
        // 定义全局变量
        private ProcessStream hotStream;
        private ProcessStream coldStream;
        private BindingSource hotBindingSource = new BindingSource();
        private BindingSource coldBindingSource = new BindingSource();

        public BasicPropForm()
        {
            InitializeComponent();
        }

        public BasicPropForm(ref ProcessStream hotStream, ref ProcessStream coldStream)
        {
            InitializeComponent();
            this.hotStream = hotStream;
            this.coldStream = coldStream;
        }

        private void EquaCalcForm_Load(object sender, EventArgs e)
        {
            // 初始化流体名称
            lbeHotStreamName.Text = hotStream.StreamName;
            lbeColdStreamName.Text = coldStream.StreamName;

            // 初始化方法选择
            radMethod1.Checked = hotStream.PropertyMethod1;
            radMethod2.Checked = !hotStream.PropertyMethod1;

            //// 临时赋值
            //InitializeData(ref tlpBasicPropertyForM1);
            //InitializeData(ref tlpBasicPropertyForM2);

            // 添加TextBox的数据绑定
            hotBindingSource.DataSource = hotStream;
            coldBindingSource.DataSource = coldStream;
            AddDataBindings(ref tlpBasicPropertyForM1);
            AddDataBindings(ref tlpBasicPropertyForM2);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (radMethod1.Checked)
            {
                if (InputCheck(ref tlpBasicPropertyForM1))
                {
                    CalcPropertyByM1(ref hotStream);
                    CalcPropertyByM1(ref coldStream);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("您输入的数据不完整", "错误提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                if (InputCheck(ref tlpBasicPropertyForM2))
                {
                    CalcPropertyByM2(ref hotStream);
                    CalcPropertyByM2(ref coldStream);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("您输入的数据不完整", "错误提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
        }

        // 方法一计算物性
        private void CalcPropertyByM1(ref ProcessStream stream)
        {
            tableLayoutPanel2.Focus();
            try
            {
                double SetTemp = (double)stream.SetTemperature;
                double Sp = (double)stream.Sp;
                double WatsonK = (double)stream.WatsonK;
                double T_1 = (double)stream.ViscosityTemp1;
                double Nu_1 = (double)stream.Viscosity1;
                double T_2 = (double)stream.ViscosityTemp2;
                double Nu_2 = (double)stream.Viscosity2;

                double API = PhyProperties.API(Sp);
                double EnthDiff = PhyProperties.EnthapyDifference((double)stream.OutletTemperature,
                    (double)stream.InletTemperature, API, WatsonK);
                double Density = PhyProperties.Density(SetTemp, Sp);
                double HeatCapa = PhyProperties.HeatCapacity(SetTemp, Sp, WatsonK);
                double Conductivity = PhyProperties.ThermalConductivity(SetTemp, Sp);
                double DynaVisc = PhyProperties.DynamicViscosity(SetTemp, Sp,
                    T_1, Nu_1, T_2, Nu_2);

                stream.EnthalpyDifference = EnthDiff;
                stream.Density = Density;
                stream.Capacity = HeatCapa;
                stream.ThermalConductivity = Conductivity;
                stream.DynamicViscosity = DynaVisc;
            }
            catch
            {
                MessageBox.Show("Oops!\n请检查数据输入！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        // 方法二计算物性
        private void CalcPropertyByM2(ref ProcessStream stream)
        {
            tableLayoutPanel2.Focus();
            try
            {
                double T_1 = (double)stream.InletTemperature; // 进口温度
                double T_2 = (double)stream.OutletTemperature; //出口温度
                double T = (double)stream.SetTemperature; // 定性温度
                double W = (double)stream.Flowrate / 3600.0; //质量流量，单位：kg/h
                double Q = (double)stream.HeatDuty * 1000; //热负荷，单位：kW
                double rho_1 = (double)stream.InletDensity; // 进口密度
                double rho_2 = (double)stream.OutletDensity; // 出口密度
                double Cp_1 = (double)stream.InletCapacity * 1000; //进口比热容，单位：kJ/(kg*K)
                double Cp_2 = (double)stream.OutletCapacity * 1000; //出口比热容，单位：kJ/(kg*K)
                double lambda_1 = (double)stream.InletThermalConductivity; //进口导热系数，单位：W/(m*K)
                double lambda_2 = (double)stream.OutletThermalConductivity; //出口导热系数，单位：W/(m*K)
                double mu_1 = (double)stream.InletDynamicViscosity; // 进口粘度，单位：cP
                double mu_2 = (double)stream.OutletDynamicViscosity; // 出口粘度，单位：cP

                double sp_1 = rho_1 / 1000;
                double sp_2 = rho_2 / 1000;
                double sp = PhyProperties.TempInterpolation(T_1, sp_1, T_2, sp_2, 20);
                stream.Sp = sp;
                double DH = Q / W;
                double rho = PhyProperties.TempInterpolation(T_1, rho_1, T_2, rho_2, T);
                double Cp = PhyProperties.TempInterpolation(T_1, Cp_1, T_2, Cp_2, T);
                double lambda = PhyProperties.TempInterpolation(T_1, lambda_1, T_2, lambda_2, T);
                double nu_1 = mu_1 / rho_1 * 1000;
                double nu_2 = mu_2 / rho_2 * 1000;
                double mu = PhyProperties.DynamicViscosity(T, sp, T_1, nu_1, T_2, nu_2);

                stream.EnthalpyDifference = DH;
                stream.Density = rho;
                stream.Capacity = Cp;
                stream.ThermalConductivity = lambda;
                stream.DynamicViscosity = mu;
            }
            catch
            {
                MessageBox.Show("Oops!\n请检查数据输入！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }            

        // 检查数据输入的完整性
        private bool InputCheck(ref TableLayoutPanel tlp)
        {
            double value;
            bool flag = false;
            foreach (Control control in tlp.Controls)
            {
                if (control is TextBox)
                {
                    if (double.TryParse(control.Text, out value))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }
            return flag;
        }

        // 添加数据绑定
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
                    else
                    {
                        string propName = controlName.Substring(7, (controlName.Length - 7));
                        control.DataBindings.Add("Text", coldBindingSource, propName, true, DataSourceUpdateMode.OnValidation, "");
                    }
                }
            }
        }

        //// 临时赋值方法
        //private void InitializeData(ref TableLayoutPanel tlp)
        //{
        //    foreach (Control control in tlp.Controls)
        //    {
        //        if (control is TextBox || control is ComboBox)
        //        {
        //            string controlName = control.Name;
        //            double value = double.Parse(control.Text);
        //            if (controlName.Contains("Hot"))
        //            {
        //                string propName = controlName.Substring(6, (controlName.Length - 6));
        //                hotStream.GetType().GetProperty(propName).SetValue(hotStream, value);
        //            }
        //            else
        //            {
        //                string propName = controlName.Substring(7, (controlName.Length - 7));
        //                coldStream.GetType().GetProperty(propName).SetValue(coldStream, value);
        //            }
        //        }
        //    }
        //}

        // 方法选择切换事件处理
        private void RadMethod2_CheckedChanged(object sender, EventArgs e)
        {
            if (radMethod1.Checked == true)
                hotStream.PropertyMethod1 = true;
            else
                hotStream.PropertyMethod1 = false;
        }
    }
}
