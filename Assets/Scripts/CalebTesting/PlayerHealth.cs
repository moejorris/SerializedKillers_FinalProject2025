using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject HeartHolder;
    [SerializeField] private Image[] redHearts;
    [SerializeField] private Image[] whiteHearts;
    public float speed = 1;
    private float maxHealth = 20;
    public float health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        UpdateHealth();


    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health < 0) health = 0;
        UpdateHealth();
    }

    public void HealDamage(float hp)
    {
        health += hp;
        if (health > maxHealth) health = maxHealth;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        redHearts[4].fillAmount = (health - 16) / 4;
        redHearts[3].fillAmount = (health - 12) / 4;
        redHearts[2].fillAmount = (health - 8) / 4;
        redHearts[1].fillAmount = (health - 4) / 4;
        redHearts[0].fillAmount = health / 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(8.0f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            HealDamage(1.0f);
        }

        if (whiteHearts[4].fillAmount > redHearts[4].fillAmount) // white is higher
        {
            whiteHearts[4].fillAmount -= Time.deltaTime * speed;
        }
        else if (whiteHearts[4].fillAmount <= redHearts[4].fillAmount && redHearts[4].fillAmount <= 0) // white is equal or lower to red and red is 0
        {
            whiteHearts[4].fillAmount = redHearts[4].fillAmount;

            if (whiteHearts[3].fillAmount > redHearts[3].fillAmount && whiteHearts[4].fillAmount <= 0)
            {
                whiteHearts[3].fillAmount -= Time.deltaTime * speed;
            }
            else if (whiteHearts[3].fillAmount <= redHearts[3].fillAmount && redHearts[3].fillAmount <= 0)
            {
                whiteHearts[3].fillAmount = redHearts[3].fillAmount;

                if (whiteHearts[2].fillAmount > redHearts[2].fillAmount && whiteHearts[3].fillAmount <= 0)
                {
                    whiteHearts[2].fillAmount -= Time.deltaTime * speed;
                }
                else if (whiteHearts[2].fillAmount <= redHearts[2].fillAmount && redHearts[2].fillAmount <= 0)
                {
                    whiteHearts[2].fillAmount = redHearts[2].fillAmount;

                    if (whiteHearts[1].fillAmount > redHearts[1].fillAmount && whiteHearts[2].fillAmount <= 0)
                    {
                        whiteHearts[1].fillAmount -= Time.deltaTime * speed;
                    }
                    else if (whiteHearts[1].fillAmount <= redHearts[1].fillAmount && redHearts[1].fillAmount <= 0)
                    {
                        whiteHearts[1].fillAmount = redHearts[1].fillAmount;

                        if (whiteHearts[0].fillAmount > redHearts[0].fillAmount && whiteHearts[1].fillAmount <= 0)
                        {
                            whiteHearts[0].fillAmount -= Time.deltaTime * speed;
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
