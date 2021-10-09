using System.Collections.Generic;
using FusionCore.Test.Models;
using FusionCore.Test.Views;
using UnityEngine;

namespace FusionCore.Test
{
	public class Bootstrapper : MonoBehaviour
	{
		[SerializeField]
		private CharacterPrefab[] _characters;
		[SerializeField]
		private SpawnPoint[] _spawns;

		private Battlefield _battlefield;

		public void Start()
		{
			var spawnPositionsByTeam = new Dictionary<uint, List<Vector3>>();
			
			foreach (var spawn in _spawns)
			{
				var team = spawn.Team;
				
				if (spawnPositionsByTeam.TryGetValue(team, out var spawnPoints))
					spawnPoints.Add(spawn.transform.position);
				else
					spawnPositionsByTeam.Add(team, new List<Vector3>{ spawn.transform.position });

				//Destroy(spawn.gameObject);
			}
			
			_battlefield = new Battlefield(spawnPositionsByTeam);
			_battlefield.Start(_characters);
		}

		public void Update()
		{
			_battlefield.Update(Time.deltaTime);
		}
	}
}