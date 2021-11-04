#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<float4x4> Matrices;
#endif

void ConfigureProcedural() {
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    unity_ObjectToWorld = Matrices[unity_InstanceID];
    #endif
}

void FractalFunction_float(float3 In, out float3 Out) {
    Out = In;
}

void FractalFunction_half(half3 In, out half3 Out) {
    Out = In;
}
