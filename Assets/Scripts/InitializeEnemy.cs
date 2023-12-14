using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeEnemy : MonoBehaviour
{
    public List<GameObject> helmet = new List<GameObject>();
    public List<GameObject> head = new List<GameObject>();
    public List<GameObject> hair = new List<GameObject>();
    public List<Material> hairColor = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        // create a random enemy

        int randomHead = Random.Range(0, head.Count);
        for (int i = 0; i < head.Count; i++)
        {   
            if(randomHead != i)
                head[i].SetActive(false);
            else
                head[i].SetActive(true);
        }

        int wearHelmet = Random.Range(0, 11);

        int randomHelmet = Random.Range(0, helmet.Count);

        // 60% for enemy to have helmet on
        if (wearHelmet >= 4)
        {
            for (int i = 0;i < helmet.Count; i++)
            {
                if(randomHelmet != i)
                    helmet[i].SetActive(false);
                else
                    helmet[i].SetActive(true);
            }

            foreach(GameObject hair in hair)
                hair.SetActive(false);
        }
        else
        {
            int hairIndex = Random.Range(0, hair.Count);

            for (int i = 0; i < hair.Count; i++)
            {
                hair[i].SetActive(i == hairIndex);
            }

            SkinnedMeshRenderer hairMaterial = hair[hairIndex].GetComponent<SkinnedMeshRenderer>();
            hairMaterial.material = hairColor[Random.Range(0, hairColor.Count)];

            foreach (GameObject helmet in helmet)
                helmet.SetActive(false);
        }
        
    }


}
