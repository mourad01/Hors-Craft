// DecompilerFi decompiler from Assembly-CSharp.dll class: ScreenShotMaker
using System.IO;
using UnityEngine;

public class ScreenShotMaker : MonoBehaviour
{
	private bool renderActive;

	private GameObject tempCameraHolder;

	private RenderTexture renderTexture;

	private int width;

	private int height;

	private string path;

	public void TakeScreenshoot(int width, int height, string path)
	{
		this.width = width;
		this.height = height;
		this.path = path;
		renderTexture = new RenderTexture(width, height, 24);
		GetComponent<Camera>().targetTexture = renderTexture;
		RenderTexture.active = renderTexture;
		renderActive = true;
	}

	public void OnPostRender()
	{
		if (renderActive)
		{
			TakeScreenshoot();
			renderActive = false;
		}
	}

	private void TakeScreenshoot()
	{
		UnityEngine.Debug.Log("Taking Screenshot");
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		TryToWriteFile(bytes);
		CleanAfterRender();
	}

	private void CleanAfterRender()
	{
		RenderTexture.active = null;
		renderTexture.Release();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void TryToWriteFile(byte[] bytes)
	{
		File.WriteAllBytes(path, bytes);
	}
}
