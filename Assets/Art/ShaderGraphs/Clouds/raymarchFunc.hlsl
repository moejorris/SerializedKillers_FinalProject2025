void raymarchFunc_float(float3 rayOrigin, float3 rayDirection, float stepCount, float stepSize, float densityScale, UnityTexture3D volumeText, UnitySamplerState volumeSampler,
	float3 offset, float lightStepCount, float lightStepSize, float3 lightDir, float lightAbsorb, float darknessThreshold, float transmittance, out float3 result)
{
	float density = 0;
	float transmission = 0;
	float lightAccumulation = 0;
	float finalLight = 0;

	for (int i = 0; i < stepCount; i++)
	{
		rayOrigin += (rayDirection * stepSize);

		float3 samplePos = rayOrigin + offset;
		float sampledDensity =
			SAMPLE_TEXTURE3D(volumeText, volumeSampler, samplePos).r;
		density += sampledDensity * densityScale;

		float3 lightRayOrigin = samplePos;

		for (int j = 0; j < lightStepCount; j++)
		{
			lightRayOrigin += -lightDir * lightStepSize;
			float lightDensity = SAMPLE_TEXTURE3D(volumeText, volumeSampler, lightRayOrigin).r;
			lightAccumulation += lightDensity;
		}

		float lightTransmission = exp(-lightAccumulation);
		float shadow = darknessThreshold + lightTransmission * (1.0 - darknessThreshold);
		finalLight += density * transmittance * shadow;
		transmittance *= exp(-density * lightAbsorb);
	}

	transmission = exp(-density);
	result = float3(finalLight, transmission, transmittance);
}