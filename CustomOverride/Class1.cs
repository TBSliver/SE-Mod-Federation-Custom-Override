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

        private static void CustomOverrideSettings()
        {
            // Thruster Changes
            IEnumerable<MyThrustDefinition> allDefinitions = MyDefinitionManager.Static.GetAllDefinitions().OfType<MyThrustDefinition>();
            foreach (MyThrustDefinition def in allDefinitions)
            {
                float baseAtmosphericThrust = 9600;
                switch (def.Id.SubtypeName)
                {
                    // Large Block Atmospheric
                    case "LargeBlockLargeAtmosphericThrustSciFi_3223_USGC":
                    case "LargeBlockLargeAtmosphericThrustSciFi":
                    case "LargeBlockLargeAtmosphericThrust_3223_USGC":
                    case "LargeBlockLargeAtmosphericThrust":
                        def.ForceMagnitude = baseAtmosphericThrust * 300f;
                        break;
                    case "LargeBlockSmallAtmosphericThrustSciFi_3223_USGC":
                    case "LargeBlockSmallAtmosphericThrustSciFi":
                    case "LargeBlockSmallAtmosphericThrust_3223_USGC":
                    case "LargeBlockSmallAtmosphericThrust":
                        def.ForceMagnitude = baseAtmosphericThrust * 30f;
                        break;

                    // Small Block Atmospheric
                    case "SmallBlockLargeAtmosphericThrustSciFi":
                    case "SmallBlockLargeAtmosphericThrust":
                        def.ForceMagnitude = baseAtmosphericThrust * 10f;
                        break;
                    case "SmallBlockSmallAtmosphericThrustSciFi":
                    case "SmallBlockSmallAtmosphericThrust":
                        def.ForceMagnitude = baseAtmosphericThrust;
                        break;
                }
            }
        }
    }
}