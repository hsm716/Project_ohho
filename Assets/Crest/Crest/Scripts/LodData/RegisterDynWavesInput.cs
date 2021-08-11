// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

using UnityEditor;
using UnityEngine;

namespace Crest
{
    /// <summary>
    /// Registers a custom input to the dynamic wave simulation. Attach this GameObjects that you want to influence the sim to add ripples etc.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu(MENU_PREFIX + "Dynamic Waves Input")]
    public class RegisterDynWavesInput : RegisterLodDataInput<LodDataMgrDynWaves>
    {
        public override float Wavelength => 0f;

        public override bool Enabled => true;

        protected override Color GizmoColor => new Color(0f, 1f, 0f, 0.5f);

        protected override string ShaderPrefix => "Crest/Inputs/Dynamic Waves";

#if UNITY_EDITOR
        protected override bool FeatureEnabled(OceanRenderer ocean) => ocean.CreateDynamicWaveSim;
        protected override string FeatureDisabledErrorMessage => "<i>Create Dynamic Wave Sim</i> must be enabled on the OceanRenderer component to enable the dynamic wave simulation.";
        protected override void FixOceanFeatureDisabled(SerializedObject oceanComponent)
        {
            oceanComponent.FindProperty("_createDynamicWaveSim").boolValue = true;
        }
#endif
    }
}
