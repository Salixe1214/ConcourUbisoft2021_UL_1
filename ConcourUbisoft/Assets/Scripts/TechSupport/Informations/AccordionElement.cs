using System;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{
	[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(LayoutElement))]
	public class AccordionElement : Toggle
	{

		[SerializeField] private float minHeight = 18f;
		
		private LayoutElement _layoutElement;

		protected override void Awake()
		{
			base.Awake();
			toggleTransition = ToggleTransition.None;
			_layoutElement = gameObject.GetComponent<LayoutElement>();
			onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(bool state)
		{
			if (_layoutElement == null)
				return;
			

			if (state)
			{
				_layoutElement.preferredHeight = -1f;
			}
			else
			{
				_layoutElement.preferredHeight = minHeight;
			}
		}

	}
}
