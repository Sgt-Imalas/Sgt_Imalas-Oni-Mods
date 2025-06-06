using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom
{
	class Chemical_BallCrusherMill_RandomFabricator : ComplexFabricatorRandomOutput
	{
		public override Dictionary<Tag, RecipeRandomResult> GetRandomOutputSelection() => ModDb.RandomRecipeResults.BallCrusher_RandomResults;
		
	}
}
