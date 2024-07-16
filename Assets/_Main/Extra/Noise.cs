using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
	public static float PerlinNoise3D(float x, float y, float z) {
		float xy = Mathf.PerlinNoise(x, y) * Random.Range(1.5f, 100);
		float xz = Mathf.PerlinNoise(x, z) * Random.Range(1.5f, 100);
		float yz = Mathf.PerlinNoise(y, z) * Random.Range(1.5f, 100);
		float yx = Mathf.PerlinNoise(y, x) * Random.Range(1.5f, 100);
		float zx = Mathf.PerlinNoise(z, x) * Random.Range(1.5f, 100);
		float zy = Mathf.PerlinNoise(z, y) * Random.Range(1.5f, 100);

		return (xy + xz + yz + yx + zx + zy) / 6;
	}
}
