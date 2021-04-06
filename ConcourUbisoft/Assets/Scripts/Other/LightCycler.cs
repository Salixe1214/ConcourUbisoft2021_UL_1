using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCycler : MonoBehaviour
{
	[SerializeField] private Color emmisionColor = Color.yellow;
	[SerializeField] private float emmisionIntensity = 10f;
	[SerializeField] private float interval = 0.5f;
	private Renderer[] _renderers;
	private int currentIndex = 0;
	private bool cycling = true;

	void Start()
	{
		_renderers = GetComponentsInChildren<Renderer>();
		StartCoroutine(Cycle());
	}

	IEnumerator Cycle()
	{
		while (cycling)
		{
			Color baseColor = _renderers[currentIndex].material.color;
			_renderers[currentIndex].material.color = emmisionColor;
			_renderers[currentIndex].material.SetColor("_EmissionColor", emmisionColor * emmisionIntensity);
			_renderers[currentIndex].material.EnableKeyword("_EMISSION");
			yield return new WaitForSeconds(interval);
			_renderers[currentIndex].material.color = baseColor;
			_renderers[currentIndex].material.DisableKeyword("_EMISSION");
			currentIndex = (currentIndex + 1) % _renderers.Length;
		}
	}
}