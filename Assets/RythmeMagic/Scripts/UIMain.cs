using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
	bool isCombo;
	[SerializeField] Text textCombo;
	Animator comboAnimator;

	[SerializeField] Text textScore;
	// Start is called before the first frame update
	void Awake()
	{
		comboAnimator = textCombo.GetComponent<Animator>();
	}

	public void Init()
	{
		BreakCombo();
		SetScore(0);
	}

	public void SetScore(int score)
	{
		textScore.text = score.ToString();
	}

	public void SetCombo(int combo)
	{
		isCombo = true;
		comboAnimator.SetTrigger("combo");
		textCombo.text = combo + " HIT";
	}

	public void BreakCombo()
	{
		if (!isCombo) return;

		isCombo = false;
		comboAnimator.SetTrigger("break");
	}
}
