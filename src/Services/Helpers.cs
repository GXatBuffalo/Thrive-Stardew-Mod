
namespace Thrive.src.Services
{
	internal static class Helpers
	{

		public static List<T> PartialFY_Shuffle<T>(IList<T> inputList, Random rand, int properties)
		{
			int n = inputList.Count;
			for (int i = 0; i < properties; i++)
			{
				int k = rand.Next(i+1, n);
				(inputList[i], inputList[k]) = (inputList[k], inputList[i]);
			}
			return inputList.Take(properties).ToList();
		}

	}
}
