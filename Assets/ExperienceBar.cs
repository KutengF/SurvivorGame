using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxExperience(float experience)
    {
        slider.maxValue = experience;
        slider.value = 0;

        fill.color = gradient.Evaluate(0f);
    }

    public void SetExperience(float experience)
    {
        slider.value = experience;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
