using System.Collections;
using System.Linq;

namespace Java2Dotnet.Spider.Lib
{
	public static class ObjectUtils
	{
		/// <summary>
		/// Generate a hash value from an array of bits
		/// </summary>
		/// <remarks>voir http://blog.roblevine.co.uk for comparison of hash algorithm implementations</remarks>
		/// <param name="data">array of bits to hash</param>
		/// <returns></returns>
		public static int HashBytes(BitArray data)
		{
			// convert bit array to integer array
			int[] intArray = new int[(data.Length + 31) / 32];
			data.CopyTo(intArray, 0);
			// compute the hash from integer array values
			unchecked
			{
				return intArray.Aggregate(23, (current, n) => current*37 + n);
			}
		}

		/// <summary>
		/// Check if two arrays of bits are equals
		/// Returns true if every bit of this first array is equal to the corresponding bit of the second, false otherwise
		/// </summary>
		public static bool Equals(BitArray a, BitArray b)
		{
			if (a.Length != b.Length) return false;

			var enumA = a.GetEnumerator();
			var enumB = b.GetEnumerator();

			while (enumA.MoveNext() && enumB.MoveNext())
			{
				if ((bool)enumA.Current != (bool)enumB.Current) return false;
			}
			return true;
		}

	}
}
