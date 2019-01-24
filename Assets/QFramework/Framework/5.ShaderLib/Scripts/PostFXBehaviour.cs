namespace QFramework
{
	using UnityEngine;

	//非运行时也触发效果
	[ExecuteInEditMode]
	//屏幕后处理特效一般都需要绑定在摄像机上
	[RequireComponent(typeof(Camera))]
	//提供一个后处理的基类，主要功能在于直接通过Inspector面板拖入shader，生成shader对应的材质
	public sealed class PostFXBehaviour : MonoBehaviour
	{
		//Inspector面板上直接拖入
		public Shader Shader = null;
		
		[SerializeField] Material mMaterial = null;

		public Material Material
		{
			get
			{
				if (mMaterial == null)
					mMaterial = GenerateMaterial(Shader);
				return mMaterial;
			}
		}

		//根据shader创建用于屏幕特效的材质
		private Material GenerateMaterial(Shader shader)
		{
			if (shader == null)
				return null;
			//需要判断shader是否支持
			if (shader.isSupported == false)
				return null;
			Material material = new Material(shader);
			material.hideFlags = HideFlags.DontSave;
			if (material)
				return material;
			return null;
		}


		//覆写OnRenderImage函数
		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			//仅仅当有材质的时候才进行后处理，如果_Material为空，不进行后处理
			if (Material)
			{
				//使用Material处理Texture，dest不一定是屏幕，后处理效果可以叠加的！
				Graphics.Blit(src, dest, Material);
			}
			else
			{
				//直接绘制
				Graphics.Blit(src, dest);
			}
		}
	}
}