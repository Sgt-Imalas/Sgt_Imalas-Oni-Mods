using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	public class PlaceWallIndicator : KMonoBehaviour
	{
		[SerializeField]
		public CellOffset CellOffset = new CellOffset(0, 0);
		public Orientation VisOrientation = Orientation.Neutral;



		bool isFlippedVertically, isFlippedHorizontally = false;

		[MyCmpReq]
		public KBatchedAnimController kbac;

		internal void RefreshRotation(Orientation buildingOrientation)
		{

			Orientation baseRotation = VisOrientation;
			if ((int)buildingOrientation < 4) //building is rotated in some way and not flipped
				baseRotation = (Orientation)(((int)VisOrientation - (int)buildingOrientation + 4) % 4);


			isFlippedVertically = buildingOrientation == Orientation.FlipV;
			isFlippedHorizontally = buildingOrientation == Orientation.FlipH;

			switch (VisOrientation)
			{
				case Orientation.Neutral:
					SetVisualRotation(isFlippedVertically ? Orientation.R180 : baseRotation);
					break;
				case Orientation.R90:
					SetVisualRotation(isFlippedHorizontally ? Orientation.R270 : baseRotation);
					break;
				case Orientation.R180:
					SetVisualRotation(!isFlippedVertically ? baseRotation : Orientation.Neutral);
					break;
				case Orientation.R270:
					SetVisualRotation(!isFlippedHorizontally ? baseRotation : Orientation.R90);
					break;
			}

		}

		internal void SetVisualRotation(Orientation visRotation)
		{
			//have all lines allign with each other, going from bottom left to top right
			kbac.flipX = (visRotation == Orientation.R180||visRotation == Orientation.Neutral);
			switch (visRotation)
			{				
				case Orientation.Neutral:
					kbac.Rotation = 0;
					kbac.Offset = new Vector3(0, 0);
					break;
				case Orientation.R90:
					kbac.Rotation = 90;
					kbac.Offset = new Vector3(0.5f, 0.5f);
					break;
				case Orientation.R180:
					kbac.Rotation = 180;
					kbac.Offset = new Vector3(0.0f, 1.0f);
					break;
				case Orientation.R270:
					kbac.Rotation = 270;
					kbac.Offset = new Vector3(-0.5f, 0.5f);
					break;
			}
		}

		internal void InitRotation(Orientation visOrientation)
		{
			VisOrientation = visOrientation;
			SetVisualRotation(visOrientation);
		}

		internal void UpdateTint(Color c)
		{
			kbac.TintColour = c;
		}
	}
}
