using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListRandomizer<T>
{
    public List<T> Items = new();
    public List<float> ElementProbabilities = new();

    public float TotaledProbability
    {
        get
        {
            float totalProbability = 0;
            for (int i = 0; i < Items.Count; i++)
                totalProbability += ElementProbabilities[i];
            return totalProbability;
        }
    }

    public int Count { get { return Items.Count; } }

    public void AddItem(T item, float probability)
    {
        // add to probability if list already contains item
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Equals(item))
            {
                ElementProbabilities[i] += probability;
                return;
            }
        }

        Items.Add(item);
        ElementProbabilities.Add(probability);
    }

    public void RemoveItemAt(int index)
    {
        Items.RemoveAt(index);
        ElementProbabilities.RemoveAt(index);
    }

    public T GetRandom()
    {
        if (Items.Count == 0) return default;

        float stackedPastProbabilities = 0;

        for (int i = 0; i < Items.Count; i++)
        {
            float rng = Random.Range(0f, 1f);
            float chosenProbability = (ElementProbabilities[i] / TotaledProbability) + stackedPastProbabilities;
            stackedPastProbabilities += chosenProbability;

            if (rng < chosenProbability)
                return Items[i];
        }
        return Items[^1];
    }
}