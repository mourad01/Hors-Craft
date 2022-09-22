// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LinearBlur
using UnityEngine;

namespace States
{
	internal class LinearBlur
	{
		private float _rSum;

		private float _gSum;

		private float _bSum;

		private Texture2D _sourceImage;

		private int _sourceWidth;

		private int _sourceHeight;

		private int _windowSize;

		public Texture2D Blur(Texture2D image, int radius, int iterations)
		{
			_windowSize = radius * 2 + 1;
			_sourceWidth = image.width;
			_sourceHeight = image.height;
			Texture2D texture2D = image;
			for (int i = 0; i < iterations; i++)
			{
				texture2D = OneDimensialBlur(texture2D, radius, horizontal: true);
				texture2D = OneDimensialBlur(texture2D, radius, horizontal: false);
			}
			return texture2D;
		}

		private Texture2D OneDimensialBlur(Texture2D image, int radius, bool horizontal)
		{
			_sourceImage = image;
			Texture2D texture2D = new Texture2D(image.width, image.height, image.format, mipChain: false);
			if (horizontal)
			{
				for (int i = 0; i < _sourceHeight; i++)
				{
					ResetSum();
					for (int j = 0; j < _sourceWidth; j++)
					{
						if (j == 0)
						{
							for (int k = radius * -1; k <= radius; k++)
							{
								AddPixel(GetPixelWithXCheck(k, i));
							}
						}
						else
						{
							Color pixelWithXCheck = GetPixelWithXCheck(j - radius - 1, i);
							Color pixelWithXCheck2 = GetPixelWithXCheck(j + radius, i);
							SubstPixel(pixelWithXCheck);
							AddPixel(pixelWithXCheck2);
						}
						texture2D.SetPixel(j, i, CalcPixelFromSum());
					}
				}
			}
			else
			{
				for (int l = 0; l < _sourceWidth; l++)
				{
					ResetSum();
					for (int m = 0; m < _sourceHeight; m++)
					{
						if (m == 0)
						{
							for (int n = radius * -1; n <= radius; n++)
							{
								AddPixel(GetPixelWithYCheck(l, n));
							}
						}
						else
						{
							Color pixelWithYCheck = GetPixelWithYCheck(l, m - radius - 1);
							Color pixelWithYCheck2 = GetPixelWithYCheck(l, m + radius);
							SubstPixel(pixelWithYCheck);
							AddPixel(pixelWithYCheck2);
						}
						texture2D.SetPixel(l, m, CalcPixelFromSum());
					}
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		private Color GetPixelWithXCheck(int x, int y)
		{
			if (x <= 0)
			{
				return _sourceImage.GetPixel(0, y);
			}
			if (x >= _sourceWidth)
			{
				return _sourceImage.GetPixel(_sourceWidth - 1, y);
			}
			return _sourceImage.GetPixel(x, y);
		}

		private Color GetPixelWithYCheck(int x, int y)
		{
			if (y <= 0)
			{
				return _sourceImage.GetPixel(x, 0);
			}
			if (y >= _sourceHeight)
			{
				return _sourceImage.GetPixel(x, _sourceHeight - 1);
			}
			return _sourceImage.GetPixel(x, y);
		}

		private void AddPixel(Color pixel)
		{
			_rSum += pixel.r;
			_gSum += pixel.g;
			_bSum += pixel.b;
		}

		private void SubstPixel(Color pixel)
		{
			_rSum -= pixel.r;
			_gSum -= pixel.g;
			_bSum -= pixel.b;
		}

		private void ResetSum()
		{
			_rSum = 0f;
			_gSum = 0f;
			_bSum = 0f;
		}

		private Color CalcPixelFromSum()
		{
			return new Color(_rSum / (float)_windowSize, _gSum / (float)_windowSize, _bSum / (float)_windowSize);
		}

		public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
		{
			Texture2D texture2D = new Texture2D(targetWidth, targetHeight, source.format, mipChain: false);
			for (int i = 0; i < texture2D.height; i++)
			{
				for (int j = 0; j < texture2D.width; j++)
				{
					Color pixelBilinear = source.GetPixelBilinear((float)j / (float)texture2D.width, (float)i / (float)texture2D.height);
					texture2D.SetPixel(j, i, pixelBilinear);
				}
			}
			texture2D.Apply();
			return texture2D;
		}
	}
}
