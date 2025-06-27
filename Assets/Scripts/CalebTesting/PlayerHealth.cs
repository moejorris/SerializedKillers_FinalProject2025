using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject HeartHolder;
    [SerializeField] private Image[] redHearts;
    [SerializeField] private Image[] whiteHearts;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip damageSound;
    public float speed = 1;
    //private float maxHealth = 20;
    //public float health;

    private bool whiteHeartDrain = false;
    private float whiteHeartTimer = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //health = maxHealth;
        //UpdateHealth();
    }

    void PlaySound(AudioClip clip)
    {
        GameObject soundObject = Instantiate(new GameObject(), transform);
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;

        audioSource.PlayOneShot(clip);
        Destroy(soundObject, clip.length);
    }

    public void TakeDamage(float newHealth)
    {
        PlaySound(damageSound);
        //health -= newHealth;
        //if (health < 0) health = 0;

        whiteHeartDrain = false;
        whiteHeartTimer = 0.2f;
        UpdateHealth(newHealth);
    }

    public void HealDamage(float newHealth)
    {
        PlaySound(healSound);
        //health += newHealth;
        //if (health > maxHealth) health = maxHealth;

        UpdateHealth(newHealth);
    }

    public void UpdateHealth(float health)
    {
        List<float> heartAmounts = new List<float>();

        foreach (Image redHeart in redHearts)
        {
            heartAmounts.Add(redHeart.fillAmount);
        }

        redHearts[4].fillAmount = (health - 16) / 4;
        redHearts[3].fillAmount = (health - 12) / 4;
        redHearts[2].fillAmount = (health - 8) / 4;
        redHearts[1].fillAmount = (health - 4) / 4;
        redHearts[0].fillAmount = health / 4;

        for (int i = 0; i < heartAmounts.Count; i++)
        {
            if (heartAmounts[i] != redHearts[i].fillAmount)
            {
                redHearts[i].transform.parent.GetComponent<Animation>().Rewind();
                redHearts[i].transform.parent.GetComponent<Animation>().Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    TakeDamage(8.0f);
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    HealDamage(1.0f);
        //}

        if (whiteHeartTimer > 0)
        {
            whiteHeartTimer -= Time.deltaTime;
            if (whiteHeartTimer <= 0)
            {
                whiteHeartTimer = 0;
                whiteHeartDrain = true;
            }
        }


        if (whiteHearts[4].fillAmount > redHearts[4].fillAmount) // white is higher
        {
            if (whiteHeartDrain) whiteHearts[4].fillAmount -= Time.deltaTime * speed;
        }
        else if (whiteHearts[4].fillAmount <= redHearts[4].fillAmount && redHearts[4].fillAmount <= 0) // white is equal or lower to red and red is 0
        {
            whiteHearts[4].fillAmount = redHearts[4].fillAmount;

            if (whiteHearts[3].fillAmount > redHearts[3].fillAmount && whiteHearts[4].fillAmount <= 0)
            {
                if (whiteHeartDrain) whiteHearts[3].fillAmount -= Time.deltaTime * speed;
            }
            else if (whiteHearts[3].fillAmount <= redHearts[3].fillAmount && redHearts[3].fillAmount <= 0)
            {
                whiteHearts[3].fillAmount = redHearts[3].fillAmount;

                if (whiteHearts[2].fillAmount > redHearts[2].fillAmount && whiteHearts[3].fillAmount <= 0)
                {
                    if (whiteHeartDrain) whiteHearts[2].fillAmount -= Time.deltaTime * speed;
                }
                else if (whiteHearts[2].fillAmount <= redHearts[2].fillAmount && redHearts[2].fillAmount <= 0)
                {
                    whiteHearts[2].fillAmount = redHearts[2].fillAmount;

                    if (whiteHearts[1].fillAmount > redHearts[1].fillAmount && whiteHearts[2].fillAmount <= 0)
                    {
                        if (whiteHeartDrain) whiteHearts[1].fillAmount -= Time.deltaTime * speed;
                    }
                    else if (whiteHearts[1].fillAmount <= redHearts[1].fillAmount && redHearts[1].fillAmount <= 0)
                    {
                        whiteHearts[1].fillAmount = redHearts[1].fillAmount;

                        if (whiteHearts[0].fillAmount > redHearts[0].fillAmount && whiteHearts[1].fillAmount <= 0)
                        {
                            if (whiteHeartDrain) whiteHearts[0].fillAmount -= Time.deltaTime * speed;
                        }
                        else if (whiteHearts[0].fillAmount <= redHearts[0].fillAmount && redHearts[1].fillAmount <= 0)
                        {
                            whiteHearts[0].fillAmount = redHearts[0].fillAmount;
                        }
                        else if (redHearts[0].fillAmount == 1)
                        {
                            whiteHearts[0].fillAmount = 1;
                        }
                        else
                        {
                            whiteHearts[0].fillAmount = redHearts[0].fillAmount;
                        }
                    }
                    else if (redHearts[1].fillAmount == 1)
                    {
                        whiteHearts[1].fillAmount = 1;
                        whiteHearts[0].fillAmount = 1;

                        redHearts[0].fillAmount = 1;
                    }
                    else
                    {
                        whiteHearts[1].fillAmount = redHearts[1].fillAmount;
                    }
                }
                else if (redHearts[2].fillAmount == 1)
                {
                    whiteHearts[2].fillAmount = 1;
                    whiteHearts[1].fillAmount = 1;
                    whiteHearts[0].fillAmount = 1;

                    redHearts[1].fillAmount = 1;
                    redHearts[0].fillAmount = 1;
                }
                else
                {
                    whiteHearts[2].fillAmount = redHearts[2].fillAmount;
                }
            }
            else if (redHearts[3].fillAmount == 1)
            {
                whiteHearts[3].fillAmount = 1;
                whiteHearts[2].fillAmount = 1;
                whiteHearts[1].fillAmount = 1;
                whiteHearts[0].fillAmount = 1;

                redHearts[2].fillAmount = 1;
                redHearts[1].fillAmount = 1;
                redHearts[0].fillAmount = 1;
            }
            else
            {
                whiteHearts[3].fillAmount = redHearts[3].fillAmount;
            }
        }
        else if (redHearts[4].fillAmount == 1) // red is full, white is equal
        {
            whiteHearts[4].fillAmount = 1;
            whiteHearts[3].fillAmount = 1;
            whiteHearts[2].fillAmount = 1;
            whiteHearts[1].fillAmount = 1;
            whiteHearts[0].fillAmount = 1;

            redHearts[3].fillAmount = 1;
            redHearts[2].fillAmount = 1;
            redHearts[1].fillAmount = 1;
            redHearts[0].fillAmount = 1;
        }
        else // anything else
        {
            whiteHearts[4].fillAmount = redHearts[4].fillAmount;
        }
    }
}
