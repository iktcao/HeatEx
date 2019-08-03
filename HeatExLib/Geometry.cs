using System;

namespace HeatExLib
{
    [Serializable]
    public class Geometry
    {
        private double? tubeFlowArea; // 管程流通面积，单位：m2
        private double? tubeInsideMassFlux; // 管内流体质量流速，单位：kg/(m2*s)
        private double? tubeReynolds; // 管内流体的雷诺数，单位：无量纲
        private double? tubeOutsideDiameter; // 换热管外径，单位：mm
        private double? tubeWallThickness; // 换热管壁厚，单位：mm
        private int? tubePasses; // 管程数，单位：无量纲
        private int? tubeNo; // 换热管总数，单位：无量纲
        private double? tubePrandtl;  // 管内流体的普朗特准数，单位：无量纲
        private double? tubeHTFactor; // 管内传热因子，单位：无量纲
        private double? tubeLength; // 管长，单位：m
        private double? centerTubePasses; //中心管排数，单位：无量纲
        private double? tubeWallFactor; // 管内壁温校正因子，单位：无量纲
        private double? shellWallFactor; // 管外壁温校正因子，单位：无量纲
        private double? tubeIOFilmCoefficient; // 以管外表面积为基准的管内膜传热系数，单位：W/(m2*K)
        private double? shellInsideDiameter; // 壳径，单位：mm
        private double? buffleSpacing; // 折流板间距，单位：mm
        private double? shellFlowArea; // 壳程流通面积，单位：m2
        private double? shellMassFlux; // 壳程流体质量流速，单位：kg/(m2*s)
        private string tubeLayout = "45°"; // 管子排列形式，单位：°
        private string tubeType="光管"; // 换热管类型
        private string tubeMaterial="碳钢"; // 换热管材质
        private double? tubePitch; // 管心距，单位：mm
        private double? equivalentDiameter; // 管子的当量直径，单位：mm
        private double? shellReynolds; // 壳程流体的雷诺数，单位：无量纲
        private double? shellPrandtl;  // 壳程流体的普朗特准数，单位：无量纲
        private double? shellHTFactor; // 壳程传热因子，单位：无量纲
        private double? buffleCut; // 弓形折流板缺圆高度百分数，单位：%
        private double? tubeOutFilmCoefficient; // 以管外表面积为基准的管外膜传热系数，单位：W/(m2*K)
        private double? wallTemperature; // 管壁温度，单位：℃
        private double? totalHTCoefficient; // 总传热系数，单位：W/(m2*K)
        private double? cleanHTCoefficient; // 清洁总传热系数，单位：W/(m2*K)
        private double? actualArea; // 选择换热器的换热面积，单位：m2
        private double? requiredArea; // 需要换热面积，单位：m2
        private double? overDesign; // 面积裕量，单位：%
        private double? kDifference; // K计算值与K选用值的偏差，单位：%
        private double? tubeStraightDP; // 管程直管压力降，单位：kPa
        private double? tubeBendDP; // 管程回弯压力降，单位：kPa
        private double? tubeInletNozzle; // 管程进口管嘴尺寸，单位：mm
        private double? tubeOutletNozzle; // 管程出口管嘴尺寸，单位：mm
        private double? tubeNozzleDP; // 管程进出口管嘴压力降，单位：kPa
        private double? tubeDP; // 管程总压力降，单位：kPa
        private double? shellBundleDP; // 壳程管束压力降，单位：kPa
        private double? shellCylinderDP; // 壳程导流板或导流筒压力降，单位：kPa
        private double? shellInletNozzle; // 壳程进口管嘴尺寸，单位：mm
        private double? shellOutletNozzle; // 壳程出口管嘴尺寸，单位：mm
        private double? shellNozzleDP; // 壳程进出口管嘴压力降，单位：kPa
        private double? shellDP; // 壳程总压力降，单位：kPa

        public double? TubeFlowArea { get => tubeFlowArea; set => tubeFlowArea = value; }
        public double? TubeInsideMassFlux { get => tubeInsideMassFlux; set => tubeInsideMassFlux = value; }
        public double? TubeReynolds { get => tubeReynolds; set => tubeReynolds = value; }
        public double? TubeOutsideDiameter { get => tubeOutsideDiameter; set => tubeOutsideDiameter = value; }
        public double? TubeWallThickness { get => tubeWallThickness; set => tubeWallThickness = value; }
        public double? TubeInsideDiameter { get => tubeOutsideDiameter - 2 * tubeWallThickness; } // 换热管内径，单位：mm
        public int? TubePasses { get => tubePasses; set => tubePasses = value; }
        public int? TubeNo { get => tubeNo; set => tubeNo = value; }
        public double? TubePrandtl { get => tubePrandtl; set => tubePrandtl = value; }
        public double? TubeHTFactor { get => tubeHTFactor; set => tubeHTFactor = value; }
        public double? TubeLength { get => tubeLength; set => tubeLength = value; }
        public double? TubeIOFilmCoefficient { get => tubeIOFilmCoefficient; set => tubeIOFilmCoefficient = value; }
        public double? BuffleSpacing { get => buffleSpacing; set => buffleSpacing = value; }
        public double? ShellFlowArea { get => shellFlowArea; set => shellFlowArea = value; }
        public double? CenterTubePasses { get => centerTubePasses; set => centerTubePasses = value; }
        public double? ShellInsideDiameter { get => shellInsideDiameter; set => shellInsideDiameter = value; }
        public double? ShellMassFlux { get => shellMassFlux; set => shellMassFlux = value; }
        public string TubeLayout { get => tubeLayout; set => tubeLayout = value; }
        public double? TubePitch { get => tubePitch; set => tubePitch = value; }
        public double? EquivalentDiameter { get => equivalentDiameter; set => equivalentDiameter = value; }
        public double? ShellReynolds { get => shellReynolds; set => shellReynolds = value; }
        public double? ShellPrandtl { get => shellPrandtl; set => shellPrandtl = value; }
        public double? ShellHTFactor { get => shellHTFactor; set => shellHTFactor = value; }
        public double? BuffleCut { get => buffleCut; set => buffleCut = value; }
        public double? TubeOutFilmCoefficient { get => tubeOutFilmCoefficient; set => tubeOutFilmCoefficient = value; }
        public double? WallTemperature { get => wallTemperature; set => wallTemperature = value; }
        public string TubeMaterial { get => tubeMaterial; set => tubeMaterial = value; }
        public double? TotalHTCoefficient { get => totalHTCoefficient; set => totalHTCoefficient = value; }
        public double? CleanHTCoefficient { get => cleanHTCoefficient; set => cleanHTCoefficient = value; }
        public double? ActualArea { get => actualArea; set => actualArea = value; }
        public double? RequiredArea { get => requiredArea; set => requiredArea = value; }
        public double? OverDesign { get => overDesign; set => overDesign = value; }
        public double? KDifference { get => kDifference; set => kDifference = value; }
        public double? TubeWallFactor { get => tubeWallFactor; set => tubeWallFactor = value; }
        public double? ShellWallFactor { get => shellWallFactor; set => shellWallFactor = value; }
        public double? TubeStraightDP { get => tubeStraightDP; set => tubeStraightDP = value; }
        public double? TubeBendDP { get => tubeBendDP; set => tubeBendDP = value; }
        public double? TubeInletNozzle { get => tubeInletNozzle; set => tubeInletNozzle = value; }
        public double? TubeOutletNozzle { get => tubeOutletNozzle; set => tubeOutletNozzle = value; }
        public double? TubeNozzleDP { get => tubeNozzleDP; set => tubeNozzleDP = value; }
        public double? TubeDP { get => tubeDP; set => tubeDP = value; }
        public double? ShellBundleDP { get => shellBundleDP; set => shellBundleDP = value; }
        public double? ShellCylinderDP { get => shellCylinderDP; set => shellCylinderDP = value; }
        public double? ShellInletNozzle { get => shellInletNozzle; set => shellInletNozzle = value; }
        public double? ShellOutletNozzle { get => shellOutletNozzle; set => shellOutletNozzle = value; }
        public double? ShellNozzleDP { get => shellNozzleDP; set => shellNozzleDP = value; }
        public double? ShellDP { get => shellDP; set => shellDP = value; }
        public string TubeType { get => tubeType; set => tubeType = value; }

        public double? GetTubeFlowArea()
        {
            return ((double)TubeNo / TubePasses) * Math.PI * TubeInsideDiameter * TubeInsideDiameter / 4000000;
        }
    }
}
