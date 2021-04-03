using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
	[SerializeField] private Vector2 startTranslation = Vector2.zero;
	[SerializeField] private float speedMultiplier;
	private Vector2 translation;
	private Material material;

	void Start()
	{
		material = GetComponent<Renderer>().material;
		translation = startTranslation;
	}

	public void SetTranslation(Vector2 translation)
	{
		this.translation = translation;
	}

	void Update()
	{
		material.mainTextureOffset = material.mainTextureOffset + (speedMultiplier * Time.deltaTime * translation);
	}
}