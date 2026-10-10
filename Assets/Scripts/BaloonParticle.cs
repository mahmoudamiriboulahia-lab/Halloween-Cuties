using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class BaloonParticle : MonoBehaviour {

	public bool active;

	void Awake()
	{
		active = false;
	}

	public void PlayBaloonParticle()
	{
		StartCoroutine(BaloonParticleCoroutine());
	}

	IEnumerator BaloonParticleCoroutine()
	{
		active = true;

		GetComponent<ParticleSystem>().Play();

//		float timer = 0;

//		while (timer <= 1.4f)
//		{
//			yield return new WaitForEndOfFrame();
//			GetComponent<ParticleSystem>().Simulate(Time.unscaledDeltaTime, true, false);
//			timer += Time.unscaledDeltaTime;
//		}

		yield return new WaitForSeconds(1.4f);

		GetComponent<ParticleSystem>().Stop();

		active = false;
	}

	public void PlayAddTimeAnimation()
	{
		StartCoroutine(AddTimeCoroutine());
	}

	IEnumerator AddTimeCoroutine()
	{
		active = true;

		transform.GetChild(0).GetComponent<Animator>().Play("TextAnimationActive", 0, 0);

		yield return new WaitForSeconds(0.35f);

		active = false;
	}

	public void PlayLoseTimeAnimation()
	{
		StartCoroutine(LoseTimeCoroutine());
	}

	IEnumerator LoseTimeCoroutine()
	{
		active = true;

		transform.GetChild(0).GetComponent<Animator>().Play("TextAnimationActive", 0, 0);

		yield return new WaitForSeconds(0.35f);

		active = false;
	}

	public void PlayAddPointAnimation(int points)
	{
		StartCoroutine(AddPointCoroutine(points));
	}

	IEnumerator AddPointCoroutine(int points)
	{
		active = true;

		transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "+" + points.ToString();

		transform.GetChild(0).GetComponent<Animator>().Play("TextAnimationActive", 0, 0);

		yield return new WaitForSeconds(0.35f);

		active = false;
	}
}
