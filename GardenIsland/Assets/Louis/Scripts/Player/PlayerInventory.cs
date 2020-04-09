using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool[] medicFlower = { false, false, false };
    public int[] maxPetal = { 3, 5, 4 };
    public int[] currentPetal = { 0, 0, 0 };

    public float[] waterCycle = { 2.1f, 1.5f, 1.9f };
    public bool[] flowerHydratation = { true, true,true};

    public int waterStock = 0;

    public int fertilizerStock = 0;
    public int fertilizerMax = 0;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    private void Update()
    {
        if (fertilizerStock >= fertilizerMax)
        {
            PetalRecover(1);
            fertilizerStock = 0;
        }

        FlowerHydratation();
    }
    public void PetalRecover(int petal)
    {
        int weakFlower = 0;

        for (int i = 0; i < medicFlower.Length; i++)
        {
            if (medicFlower[i] && (currentPetal[i] < maxPetal[i]))
            {
                // petal priority
                weakFlower = (currentPetal[weakFlower] > currentPetal[i]) ? i : weakFlower;

                // else use a water cycle exception
                weakFlower = ((currentPetal[weakFlower] == currentPetal[i]) &&
                    (waterCycle[weakFlower] >= waterCycle[i])) ? i : weakFlower;
            }
        }
        if(currentPetal[weakFlower] < maxPetal[weakFlower])
            currentPetal[weakFlower] += petal;
    }

    public void FlowerHydratation()
    {
        for (int i = 0; i < medicFlower.Length; i++)
        {
            if (flowerHydratation[i] && medicFlower[i])
                StartCoroutine(WaterAbsorb(i, waterCycle[i]));
        }
    }
    public IEnumerator WaterAbsorb(int flower, float cycle)
    {
        flowerHydratation[flower] = false;

        yield return new WaitForSeconds(cycle);

        if (waterStock > 0)
            waterStock -= 1;
        else
            currentPetal[flower] -= 1;

        flowerHydratation[flower] = true;
    }
}
