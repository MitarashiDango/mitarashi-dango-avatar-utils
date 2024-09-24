using System.Collections.Generic;
using nadena.dev.modular_avatar.core;

namespace MitarashiDango.AvatarUtils
{
    public class PhysBonesSwitcherParameters
    {
        public static readonly string PBS_PHYS_BONES_OFF = "PBS_PhysBonesOff";
        public List<ParameterConfig> GetParameterConfigs()
        {
            var parameterConfigs = new List<ParameterConfig>
            {
                new ParameterConfig
                {
                    nameOrPrefix = PBS_PHYS_BONES_OFF,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Bool,
                    saved = false,
                    localOnly = false,
                },
            };

            return parameterConfigs;
        }
    }
}