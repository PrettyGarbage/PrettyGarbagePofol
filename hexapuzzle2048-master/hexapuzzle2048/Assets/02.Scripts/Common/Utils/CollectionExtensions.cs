using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Random = System.Random;

public static class CollectionExtensions
{
	public static void Shuffle<T>(this T[] array)
	{
		var provider = new RNGCryptoServiceProvider();
		int n = array.Length;
		while (n > 1)
		{  
			var box = new byte[1];

			do
			{
				provider.GetBytes(box);
			}
			while (!(box[0] < n * (Byte.MaxValue / n)));

			var k = (box[0] % n);
			n--;
			var value = array[k];
			array[k] = array[n];
			array[n] = value;
		}
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		var provider = new RNGCryptoServiceProvider();
		int n = list.Count;
		while (n > 1)
		{  
			var box = new byte[1];
			do
			{
				provider.GetBytes(box);
			}
			while (!(box[0] < n * (Byte.MaxValue / n)));

			var k = (box[0] % n);
			n--;
			var value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void QuickShuffle<T>(this T[] array)
	{
		var random = new Random();
		var n = array.Length;
		while (n > 1)
		{
			n--;
			var k = random.Next(n + 1);
			var value = array[k];
			array[k] = array[n];
			array[n] = value;
		}
	}

	public static void QuickShuffle<T>(this IList<T> list)
	{
		var random = new Random();
		var n = list.Count;
		while (n > 1)
		{
			n--;
			var k = random.Next(n + 1);
			var value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}	
}