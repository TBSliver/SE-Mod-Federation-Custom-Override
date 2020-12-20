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
            CustomOverrideSettings();
        }

        private const float BaseAtmosphericThrust = 9600 * 2f;
        private const float BaseAtmosphericEfficiency = 0.8f;
        private const float BaseAtmosphericPower = 0.2f;
        private const float BaseSkybladeThrust = BaseAtmosphericThrust * 1.2f;
        private const float BasePowerConsumptionDivision = 4000f;

        private static void ModifyAtmospheric(MyThrustDefinition myThrustDefinition, float multiplier)
        {
            ModifyAtmospheric(BaseAtmosphericThrust, myThrustDefinition, multiplier, 1f);
        }

        private static void ModifySkyblade(MyThrustDefinition myThrustDefinition, float multiplier)
        {
            ModifyAtmospheric(BaseSkybladeThrust, myThrustDefinition, multiplier, 10f);
        }

        private static void ModifyAtmospheric(float baseThrust, MyThrustDefinition myThruster, float multiplier,
            float efficiency)
        {
            myThruster.ForceMagnitude = baseThrust * multiplier;
            myThruster.MaxPowerConsumption
                = (
                    (myThruster.ForceMagnitude / BasePowerConsumptionDivision)
                    * BaseAtmosphericEfficiency
                    * BaseAtmosphericPower
                ) / efficiency;
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
                        ModifySkyblade(thrustDefinition, 18f);
                        break;
                    // 4 Blades Large Propeller
                    case "HeloThrusterLarge4Blade":
                        ModifySkyblade(thrustDefinition, 24f);
                        break;
                    // 5 Blades Small Propeller
                    case "HeloThrusterSmall5Blade":
                        ModifySkyblade(thrustDefinition, 12f);
                        break;
                    // 5 Blades Large Propeller
                    case "HeloThrusterLarge5Blade":
                        ModifySkyblade(thrustDefinition, 24f);
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
                        oxygenGeneratorDefinition.OperationalPowerConsumption *= 5f;
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
                        oxygenGeneratorDefinition.IceConsumptionPerSecond = 1.5f;
                        break;
                    // USGC Small
                    case "OxygenGenerator3_3223_USGC":
                        // 10th the size of the Small Grid original, so 10th the power
                        oxygenGeneratorDefinition.OperationalPowerConsumption = 0.05f;
                        oxygenGeneratorDefinition.IceConsumptionPerSecond = 0.5f;
                        oxygenGeneratorDefinition.InventoryMaxVolume = 0.2f;
                        oxygenGeneratorDefinition.InventorySize.X = 0.07f;
                        oxygenGeneratorDefinition.InventorySize.Y = 0.07f;
                        oxygenGeneratorDefinition.InventorySize.Z = 0.07f;
                        break;
                }
            }

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
            }

            // // This doesnt work??
            // IEnumerable<MyOxygenTankDefinition> oxygenTankDefinitions =
            //     MyDefinitionManager.Static.GetAllDefinitions().OfType<MyOxygenTankDefinition>();
            // foreach (var oxygenTankDefinition in oxygenTankDefinitions)
            // {
            //     switch (oxygenTankDefinition.Id.SubtypeName)
            //     {
            //         case "LargeHydrogenTank":
            //             oxygenTankDefinition.Capacity = 150000000f;
            //             break;
            //     }
            // }
        }
    }
}