using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Order : MonoBehaviour
{
    public Slider DecreasingTimer;
    public float DecreasingDuration;
    public string OrderName;
    public TextMeshProUGUI OrderNameHolder;
    public TextMeshProUGUI TableIDHolder;
    public Sprite[] OrderIngredients;
    public GameObject IngredientList;
    public Vector2 IngredientImageSize;
    public Color[] TimeSliderColors;

    public void addIngredientIcons()
    {
        foreach (var item in OrderIngredients)
        {
            GameObject Ingredient = new GameObject();
            var IngredientImageHolder = Ingredient.AddComponent<Image>();
            IngredientImageHolder.sprite = item;
            //change ingredient scale
            IngredientImageHolder.rectTransform.sizeDelta = IngredientImageSize;
            Parent(IngredientList.transform, Ingredient, 0);
        }
    }
    public void UpdateOrderUI(int TableID)
    {
        OrderNameHolder.text = OrderName;
        TableIDHolder.text = "Table #" + TableID;
    }

    private void Update()
    {
        ChangeSliderColor(DecreasingTimer);
    }

    void ChangeSliderColor(Slider slider)
    {
        var sliderFillColour = slider.transform.GetChild(1).GetComponent<Image>();
        float percentage = (slider.value / slider.maxValue)*100;

        if (percentage == 0)
        {
            sliderFillColour.color = TimeSliderColors[0];
        }
        else if (percentage <= 25)
        {
            sliderFillColour.color = TimeSliderColors[2];
        }
        else if (percentage <= 50 && percentage > 25)
        {
            sliderFillColour.color = TimeSliderColors[1];
        }
        else if (percentage <= 100 && percentage > 50)
        {
            sliderFillColour.color = TimeSliderColors[0];
        }
    }
    private void Parent(Transform Parent, GameObject child, int state)
    {
        switch (state)
        {
            case 0:
                child.transform.SetParent(Parent);
                break;
            case 1:
                child.transform.SetParent(null);
                break;
        }
    }

}
