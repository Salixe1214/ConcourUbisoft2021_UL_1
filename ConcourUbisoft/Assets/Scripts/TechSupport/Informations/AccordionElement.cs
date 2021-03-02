using System;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{
	[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(LayoutElement))]
	public class AccordionElement : Toggle
	{

		public float minHeight = 18f;
		
		private LayoutElement _layoutElement;

		protected override void Awake()
		{
			base.Awake();
			toggleTransition = ToggleTransition.None;
			isOn = false; // default to not expand
			interactable = true;
			_layoutElement = gameObject.GetComponent<LayoutElement>();
			onValueChanged.AddListener(OnValueChanged);
		}

				
		protected override void OnValidate()
		{
			base.OnValidate();
			
			if (group == null)
			{
				ToggleGroup tg = GetComponentInParent<ToggleGroup>();
				
				if (tg != null)
				{
					group = tg;
				}
			}
			
			LayoutElement le = gameObject.GetComponent<LayoutElement>();
			
			if (le != null)
			{
				if (isOn)
				{
					le.preferredHeight = -1f;
				}
				else
				{
					le.preferredHeight = minHeight;
				}
			}
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
