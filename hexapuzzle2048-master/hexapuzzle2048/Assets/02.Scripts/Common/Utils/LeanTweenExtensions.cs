using UnityEngine;
using UnityEngine.UI;

using TMPro;

public static class LeanTweenExtensions
{
	public static void LeanCancel<T>(this T component) where T : Component
	{
		LeanTween.cancel(component.gameObject);
	}

	public static LTDescr LeanValue<T>(this T component, float from, float to, float time) where T : Component
	{
		return LeanTween.value(component.gameObject, from, to, time);
	}

	public static LTDescr LeanAlpha(this Graphic graphic, float to, float time)
	{
		return LeanTween.value(graphic.gameObject, graphic.color.a, to, time).
			setOnUpdate(a => {
				Color color = graphic.color;
				color.a = a;
				graphic.color = color;
			});
	}

	public static LTDescr LeanAlpha(this Graphic graphic, float from, float to, float time)
	{
		Color color = graphic.color;
		color.a = from;
		graphic.color = color;

		return graphic.LeanAlpha(to, time);
	}

	public static LTDescr LeanAlpha(this TMP_Text text, float to, float time)
	{
		return LeanTween.value(text.gameObject, text.alpha, to, time).
			setOnUpdate(a => text.alpha = a);
	}

	public static LTDescr LeanAlpha(this TMP_Text text, float from, float to, float time)
	{
		text.alpha = from;

		return text.LeanAlpha(to, time);
	}

	public static LTDescr LeanMove<T>(this T component, Vector3 to, float time) where T : Component
	{
		return LeanTween.move(component.gameObject, to, time);
	}

	public static LTDescr LeanMove<T>(this T component, Vector3 from, Vector3 to, float time) where T : Component
	{
		component.transform.position = from;

		return LeanTween.move(component.gameObject, to, time);
	}

	public static LTDescr LeanMove<T>(this T component, LTBezierPath to, float time) where T : Component
	{
		return LeanTween.move(component.gameObject, to, time);
	}

	public static LTDescr LeanScale<T>(this T component, Vector3 to, float time) where T : Component
	{
		return LeanTween.scale(component.gameObject, to, time);
	}

	public static LTDescr LeanScale<T>(this T component, Vector3 from, Vector3 to, float time) where T : Component
	{
		component.transform.localScale = from;

		return LeanTween.scale(component.gameObject, to, time);
	}
}