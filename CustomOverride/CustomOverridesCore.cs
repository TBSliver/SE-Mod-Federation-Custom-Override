using System.Collections.Generic;
using System.Linq;
using Sandbox.Definitions;
using VRage.Game.Components;

namespace CustomOverride
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class CustomOverridesCore : MySessionComponentBase
    {
        public override void BeforeStart()
        {
            SetUpdateOrder(MyUpdateOrder.AfterSimulation);
        }

        public override void LoadData()
        {
            ReduceMinimalPricePerUnit();
            CustomOverrideSettings();
        }

        private const float BaseAtmosphericThrust = 9600 * 25f;
        private const float BaseAtmosphericEfficiency = 0.8f;
        private const float BaseAtmosphericPower = 0.1f;
        private const float BaseSkybladeThrust = BaseAtmosphericThrust * 0.1f;
        private const float BasePowerConsumptionDivision = 4000f;

        private static void ModifyAtmospheric(MyThrustDefinition myThrustDefinition, float multiplier)
        {
            ModifyAtmospheric(BaseAtmosphericThrust, myThrustDefinition, multiplier, 1f);
        }

        private static void ModifySkyblade(MyThrustDefinition myThrustDefinition, float multiplier)
        {
            ModifyAtmospheric(BaseSkybladeThrust, myThrustDefinition, multiplier, 4.5f);
        }

        private static void ModifyAtmospheric(float baseThrust, MyThrustDefinition myThruster, float multiplier,
            float efficiency)
        {
            // measured in N
            myThruster.ForceMagnitude = baseThrust * multiplier;
            // measured in MW
            myThruster.MaxPowerConsumption
                = (
                    (myThruster.ForceMagnitude / BasePowerConsumptionDivision)
                    * BaseAtmosphericEfficiency
                    * BaseAtmosphericPower
                ) / efficiency;
        }

        private static void ReduceMinimalPricePerUnit()
        {
            IEnumerable<MyPhysicalItemDefinition> physicalItemDefinitions =
                MyDefinitionManager.Static.GetAllDefinitions().OfType<MyPhysicalItemDefinition>();
            foreach (MyPhysicalItemDefinition physicalItemDefinition in physicalItemDefinitions)
            {
                physicalItemDefinition.MinimalPricePerUnit = 1;
            }
        }

        private static void CustomOverrideSettings()
        {
            // Thruster Changes
            IEnumerable<MyThrustDefinition> thrustDefinitions =
                MyDefinitionManager.Static.GetAllDefinitions().OfType<MyThrustDefinition>();
            foreach (MyThrustDefinition thrustDefinition in thrustDefinitions)
            {
                switch (thrustDefinition.Id.SubtypeName)
                {
                    // Large Block Atmospheric
                    case "LargeBlockLargeAtmosphericThrustSciFi_3223_USGC":
                    case "LargeBlockLargeAtmosphericThrustSciFi":
                    case "LargeBlockLargeAtmosphericThrust_3223_USGC":
                    case "LargeBlockLargeAtmosphericThrust":
                        ModifyAtmospheric(thrustDefinition, 300f);
                        break;
                    case "LargeBlockSmallAtmosphericThrustSciFi_3223_USGC":
                    case "LargeBlockSmallAtmosphericThrustSciFi":
                    case "LargeBlockSmallAtmosphericThrust_3223_USGC":
                    case "LargeBlockSmallAtmosphericThrust":
                        ModifyAtmospheric(thrustDefinition, 30f);
                        break;

                    // Small Block Atmospheric
                    case "SmallBlockLargeAtmosphericThrustSciFi":
                    case "SmallBlockLargeAtmosphericThrust":
                        ModifyAtmospheric(thrustDefinition, 10f);
                        break;
                    case "SmallBlockSmallAtmosphericThrustSciFi":
                    case "SmallBlockSmallAtmosphericThrust":
                        ModifyAtmospheric(thrustDefinition, 1f);
                        break;

                    // Modifications for Skyblades https://steamcommunity.com/sharedfiles/filedetails/?id=577079238
                    // 4 Blades Medium Propeller
                    case "HeloThrusterMedium4Blade":
                        ModifySkyblade(thrustDefinition, 24f);
                        break;
                    // 4 Blades Large Propeller
                    case "HeloThrusterLarge4Blade":
                        ModifySkyblade(thrustDefinition, 32f);
                        break;
                    // 5 Blades Small Propeller
                    case "HeloThrusterSmall5Blade":
                        ModifySkyblade(thrustDefinition, 12f);
                        break;
                    // 5 Blades Large Propeller
                    case "HeloThrusterLarge5Blade":
                        ModifySkyblade(thrustDefinition, 32f);
                        break;
                    // 5 Blades Tail Propeller
                    case "HeloThrusterTailRotor5Blade":
                        ModifySkyblade(thrustDefinition, 6f);
                        break;
                    // 4 Blades Tail Propeller
                    case "HeloThrusterTailRotor4Blade":
                        ModifySkyblade(thrustDefinition, 4f);
                        break;
                    // Opposite Tail Rotor
                    case "HeloThrusterTailRotor":
                        // Atmospheric not Skyblade
                        ModifyAtmospheric(thrustDefinition, 1f);
                        break;
                    // 2 Blade Plane Propeller
                    case "HeloThrusterTailRotor2BladeSmall":
                        ModifySkyblade(thrustDefinition, 8f);
                        break;
                    // 2 Blade Plane Small Propeller
                    case "HeloThrusterTailRotor2BladeSmaller":
                        ModifySkyblade(thrustDefinition, 4f);
                        break;
                    // 2 Blade Plane Propeller (With Rotary Engine)
                    case "PlanePropellerSmall":
                        ModifySkyblade(thrustDefinition, 6f);
                        break;
                }
            }

            // Hydrogen Engine Changes
            IEnumerable<MyHydrogenEngineDefinition> hydrogenEngineDefinitions =
                MyDefinitionManager.Static.GetAllDefinitions().OfType<MyHydrogenEngineDefinition>();
            foreach (MyHydrogenEngineDefinition engineDefinition in hydrogenEngineDefinitions)
            {
                switch (engineDefinition.Id.SubtypeName)
                {
                    // USGC Havent updated yet...
                    case "LargeHydrogenEngine_3223_USGC":
                        engineDefinition.FuelCapacity = 100000;
                        engineDefinition.FuelProductionToCapacityMultiplier = 0.01f;
                        break;
                }
            }

            // Oxygen Generator Changes
            IEnumerable<MyOxygenGeneratorDefinition> oxygenGeneratorDefinitions =
                MyDefinitionManager.Static.GetAllDefinitions().OfType<MyOxygenGeneratorDefinition>();
            foreach (MyOxygenGeneratorDefinition oxygenGeneratorDefinition in oxygenGeneratorDefinitions)
            {
                switch (oxygenGeneratorDefinition.Id.SubtypeName)
                {
                    // Large Grid
                    case "":
                        oxygenGeneratorDefinition.OperationalPowerConsumption *= 10f;
                        break;
                    // Small Grid
                    case "OxygenGeneratorSmall":
                        oxygenGeneratorDefinition.OperationalPowerConsumption *= 10f;
                        break;
                    case "H2 O2 Generator Large":
                        oxygenGeneratorDefinition.OperationalPowerConsumption *= 10f;
                        break;
                    // USGC Large
                    case "OxygenGenerator_3223_USGC":
                        // Match Large Grid
                        oxygenGeneratorDefinition.OperationalPowerConsumption = 5f;
                        oxygenGeneratorDefinition.IceConsumptionPerSecond = 25f;
                        break;
                    // USGC Medium
                    case "OxygenGenerator2_3223_USGC":
                        // 3 times the size of the Small Grid, so 3x the power
                        oxygenGeneratorDefinition.OperationalPowerConsumption = 1.5f;
                        oxygenGeneratorDefinition.IceConsumptionPerSecond = 15f;
                        break;
                    // USGC Small
                    case "OxygenGenerator3_3223_USGC":
                        // 10th the size of the Small Grid original, so 10th the power
                        oxygenGeneratorDefinition.OperationalPowerConsumption = 0.1f;
                        oxygenGeneratorDefinition.IceConsumptionPerSecond = 1f;
                        oxygenGeneratorDefinition.InventoryMaxVolume = 0.2f;
                        oxygenGeneratorDefinition.InventorySize.X = 0.07f;
                        oxygenGeneratorDefinition.InventorySize.Y = 0.07f;
                        oxygenGeneratorDefinition.InventorySize.Z = 0.07f;
                        ReworkGasses(oxygenGeneratorDefinition, 10, 20);
                        break;
                }
            }

            // Battery Changes
            IEnumerable<MyBatteryBlockDefinition> batteryBlockDefinitions =
                MyDefinitionManager.Static.GetAllDefinitions().OfType<MyBatteryBlockDefinition>();
            foreach (MyBatteryBlockDefinition batteryBlockDefinition in batteryBlockDefinitions)
            {
                switch (batteryBlockDefinition.Id.SubtypeName)
                {
                    case "LargeBlockBatteryBlock":
                        batteryBlockDefinition.MaxPowerOutput *= 2f;
                        batteryBlockDefinition.RequiredPowerInput *= 2f;
                        batteryBlockDefinition.MaxStoredPower *= 2f;
                        break;
                    case "LargeBlockBatteryBlock_3223_USGC":
                        batteryBlockDefinition.MaxPowerOutput *= 2f;
                        batteryBlockDefinition.RequiredPowerInput *= 2f;
                        batteryBlockDefinition.MaxStoredPower *= 2f;
                        break;
                }

                batteryBlockDefinition.MaxPowerOutput *= 6f;
            }

            // Hydrogen Storage Changes
            IEnumerable<MyGasTankDefinition> gasTankDefinitions =
                MyDefinitionManager.Static.GetAllDefinitions().OfType<MyGasTankDefinition>();
            foreach (MyGasTankDefinition gasTankDefinition in gasTankDefinitions)
            {
                switch (gasTankDefinition.Id.SubtypeName)
                {
                    // Large
                    case "LargeHydrogenTankSmall_3223_USGC":
                        gasTankDefinition.Capacity = 1000000f;
                        break;
                    // Medium
                    case "LargeHydrogenTankSmall2_3223_USGC":
                        gasTankDefinition.Capacity = 216000f;
                        // Actually ok, never mind
                        break;
                    // Small
                    case "LargeHydrogenTankSmall3_3223_USGC":
                        gasTankDefinition.Capacity = 7500f;
                        break;
                }
                
                gasTankDefinition.Capacity *= 10f;
            }

            // Reactor Changes
            IEnumerable<MyReactorDefinition> reactorDefinitions = MyDefinitionManager.Static.GetAllDefinitions().OfType<MyReactorDefinition>();
            foreach (MyReactorDefinition reactorDefinition in reactorDefinitions)
            {
                switch (reactorDefinition.Id.SubtypeName)
                {
                    case "SmallBlockSmallGenerator":
                        reactorDefinition.MaxPowerOutput = 0.01f;
                        break;
                    case "SmallBlockLargeGenerator":
                        reactorDefinition.MaxPowerOutput = 0.5f;
                        break;
                }
            }
        }
        
        private static void ReworkGasses(MyOxygenGeneratorDefinition oxygenGeneratorDefinition, float oxygen, float hydrogen)
        {
            // Damned structs and their immutability
            List<MyOxygenGeneratorDefinition.MyGasGeneratorResourceInfo> producedGases =
                new List<MyOxygenGeneratorDefinition.MyGasGeneratorResourceInfo>(oxygenGeneratorDefinition
                    .ProducedGases.Count);
            foreach (MyOxygenGeneratorDefinition.MyGasGeneratorResourceInfo gas in
                oxygenGeneratorDefinition.ProducedGases)
            {
                MyOxygenGeneratorDefinition.MyGasGeneratorResourceInfo newGas = gas;
                switch (gas.Id.SubtypeName)
                {
                    case "Oxygen":
                        newGas.IceToGasRatio = oxygen;
                        break;
                    case "Hydrogen":
                        newGas.IceToGasRatio = hydrogen;
                        break;
                }
                producedGases.Add(newGas);
            }
            oxygenGeneratorDefinition.ProducedGases = producedGases;
        }
    }
}