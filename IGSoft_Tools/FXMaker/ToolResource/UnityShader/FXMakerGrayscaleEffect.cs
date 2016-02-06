using UnityEngine;

[ExecuteInEditMode]
public class FXMakerGrayscaleEffect : FXMakerImageEffectBase
{
	public Texture  textureRamp;
	public float    rampOffset;

	// Called by camera to apply image effect
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetTexture("_RampTex", textureRamp);
		material.SetFloat("_RampOffset", rampOffset);
		Graphics.Blit (source, destination, material);
	}
}