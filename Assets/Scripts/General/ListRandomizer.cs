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

        float rng = GameManager.Singleton.IsUsingSeed
            ? GameManager.Singleton.SeededRandom.NextFloat()
            : Random.Range(0f, 1f);

        float cumulative = 0f;
        for (int i = 0; i < Items.Count; i++)
        {
            cumulative += ElementProbabilities[i] / TotaledProbability;
            if (rng < cumulative)
                return Items[i];
        }
        return Items[^1];
    }
}