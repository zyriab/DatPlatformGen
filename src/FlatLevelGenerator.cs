using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatLevelGenerator : MonoBehaviour
{
	public int levelLength = 200;

	public List<Vector3> platforms = new List<Vector3>();

	public void GeneratePlatforms()
	{
		// platformes x -> trous de maximum ...px -- platformes y -> position Y fixe !!!

		int voidSize = 0;
		int randomPick;

		Vector3 _pointBuffer = new Vector3(0,0);

		for(int i = 0; i < levelLength; i++)
		{
			_pointBuffer.x = i;

			randomPick = Random.Range(0, 10);

			if(randomPick != 0)
			{
				platforms.Add(_pointBuffer);
				voidSize = 0;
			}
			else if(randomPick == 0 && voidSize < 5) 
			{
				voidSize = Random.Range(3,7);

				i += voidSize-1;
			}
		}
	}

	public void GenerateUpperPlatforms()
	{
		int lastPlatformPos;
		int platformLength;
		int randomPick;

		Vector3 _pointBuffer = new Vector3(0, 3);

		for(int i = 0; i < levelLength; i++)
		{
			_pointBuffer.x = i;

			randomPick = Random.Range(0, 30);

			if(randomPick == 0)
			{
				platformLength = Random.Range(3, 9);

				while(_pointBuffer.x != platformLength+i)
				{
					platforms.Add(_pointBuffer);
					_pointBuffer.x += 1;
				}
			}
		}
	}

	public void Init()
	{
		GeneratePlatforms();
		GenerateUpperPlatforms();
	}
}
