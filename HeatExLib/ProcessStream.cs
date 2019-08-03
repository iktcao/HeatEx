using System;

namespace HeatExLib
{
    [Serializable]
    public class ProcessStream
    {
        private string location, streamName;
        private double? flowrate, inletTemperature, outletTemperature, inletPressure,
            allowPressureDrop, foulResistance, setTemperature, enthalpyDifference,
            density, capacity, thermalConductivity, dynamicViscosity;
        private double? sp, watsonK, viscosityTemp1, viscosity1, viscosityTemp2, viscosity2;
        private double? inletDensity, outletDensity, inletCapacity, outletCapacity,
            inletThermalConductivity, outletThermalConductivity, inletDynamicViscosity,
            outletDynamicViscosity, heatDuty;
        private bool PropertyMethod = true;

        public string Location { get => location; set => location = value; }
        public string StreamName { get => streamName; set => streamName = value; }
        public double? Flowrate { get => flowrate; set => flowrate = value; }
        public double? InletTemperature { get => inletTemperature; set => inletTemperature = value; }
        public double? OutletTemperature { get => outletTemperature; set => outletTemperature = value; }
        public double? InletPressure { get => inletPressure; set => inletPressure = value; }
        public double? AllowPressureDrop { get => allowPressureDrop; set => allowPressureDrop = value; }
        public double? FoulResistance { get => foulResistance; set => foulResistance = value; }
        public double? SetTemperature { get => setTemperature; set => setTemperature = value; }
        public double? EnthalpyDifference { get => enthalpyDifference; set => enthalpyDifference = value; }
        public double? Density { get => density; set => density = value; }
        public double? Capacity { get => capacity; set => capacity = value; }
        public double? ThermalConductivity { get => thermalConductivity; set => thermalConductivity = value; }
        public double? DynamicViscosity { get => dynamicViscosity; set => dynamicViscosity = value; }
        public double? Sp { get => sp; set => sp = value; }
        public double? WatsonK { get => watsonK; set => watsonK = value; }
        public double? ViscosityTemp1 { get => viscosityTemp1; set => viscosityTemp1 = value; }
        public double? Viscosity1 { get => viscosity1; set => viscosity1 = value; }
        public double? ViscosityTemp2 { get => viscosityTemp2; set => viscosityTemp2 = value; }
        public double? Viscosity2 { get => viscosity2; set => viscosity2 = value; }
        public double? InletDensity { get => inletDensity; set => inletDensity = value; }
        public double? OutletDensity { get => outletDensity; set => outletDensity = value; }
        public double? InletCapacity { get => inletCapacity; set => inletCapacity = value; }
        public double? OutletCapacity { get => outletCapacity; set => outletCapacity = value; }
        public double? InletThermalConductivity { get => inletThermalConductivity; set => inletThermalConductivity = value; }
        public double? OutletThermalConductivity { get => outletThermalConductivity; set => outletThermalConductivity = value; }
        public double? InletDynamicViscosity { get => inletDynamicViscosity; set => inletDynamicViscosity = value; }
        public double? OutletDynamicViscosity { get => outletDynamicViscosity; set => outletDynamicViscosity = value; }
        public double? HeatDuty { get => heatDuty; set => heatDuty = value; }
        public bool PropertyMethod1 { get => PropertyMethod; set => PropertyMethod = value; }
    }
}
