using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilLibs.UI.FUI
{
	public class FKnobControl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		[SerializeField] public Transform knob;
		[SerializeField] Image knobImage;

		public event System.Action<float> OnRotationChanged;

		public int MinStep = 0;

		private float _angle;
		private bool _interactable = true;
		private float _prevAngle = 0;

		public float Angle
		{
			get => Angle;
		}
		public void SetInteractable(bool interactable)
		{
			_interactable = interactable;
			knobImage?.color = interactable ? Color.white : Color.gray8;
		}
		public void SetAngle(float angle)
		{
			_angle = angle;
			ClampAngle();
			SetKnobRotation();
		}

		public void AutoPickChildKnob()
		{
			if (transform.childCount > 0)
				knob = transform.GetChild(0);
			if (knob != null)
				knobImage = knob.GetComponent<Image>();
		}

		private void ClampAngle()
		{
			if (MinStep > 0 && MinStep <= 180)
			{
				_angle = Mathf.Round(_angle / MinStep) * MinStep;
			}
		}


		private Vector2 mousePos;

		public void OnDrag(PointerEventData eventData)
		{
			if (!_interactable)
				return;

			mousePos = (Vector2)KInputManager.GetMousePos();
			_angle = Mathf.Atan2(
				mousePos.y - transform.position.y,
				mousePos.x - transform.position.x) * Mathf.Rad2Deg;

			_angle -= 90f;
			if (_angle > 0)
				_angle -= 360f;

			SetKnobRotation();
		}


		public void OnEndDrag(PointerEventData eventData)
		{
			ClampAngle();
			SetKnobRotation();
			if (_prevAngle == _angle)
				return;
			OnRotationChanged?.Invoke(-_angle / 360f);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_prevAngle = _angle;
		}
		private void SetKnobRotation()
		{
			var targetRotation = Quaternion.Euler(new Vector3(0, 0, _angle));
			knob.transform.rotation = targetRotation;
		}
	}
}
