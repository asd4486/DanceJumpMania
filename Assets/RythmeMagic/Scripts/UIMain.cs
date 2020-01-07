using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    [SerializeField] Text textCombo;
    [SerializeField] Text textScore;
    // Start is called before the first frame update
    void Start()
    {
        BreakCombo();
    }

   public void SetScore(int score)
    {
        textScore.text = score.ToString();
    }

    public void SetCombo(int combo)
    {
        if (!textCombo.gameObject.activeSelf)
            textCombo.gameObject.SetActive(true);

        textCombo.text = combo + " HIT";
    }

    public void BreakCombo()
    {
        textCombo.gameObject.SetActive(false);
    }
}
