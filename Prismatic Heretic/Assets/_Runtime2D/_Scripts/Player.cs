using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;

    public int maxStamina = 100;
    public static int currentStamina;

    public static int bluePowerDelay = 50;
    public static int redPowerDelay = 50;
    public static int yellowPowerDelay = 50;

    public static bool inDialog = false;

    public HealthStaminaBar healthStaminaInfo;

    public GameObject pauseMenuUIHead;

    public GameObject gameOverUIHead;

    public static bool GameIsPaused = false;
    public static bool GameOverPaused = false;
    public ParticleSystem trail;
    private bool boost=false;

    public Material purpleMaterial;
    public Material blueMaterial;
    public Material redMaterial;

   
    public void Awake()
    {
        GameIsPaused = false;
        GameOverPaused = false;
        inDialog = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        trail.Stop();
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        healthStaminaInfo.SetMaxHealth(maxHealth);
        healthStaminaInfo.SetMaxStamina(maxStamina);
        InvokeRepeating("PowerRestore", 0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 100)
        {
            currentHealth = 100;
        }
        if (currentStamina > 100)
        {
            currentStamina = 100;
        }
        if (currentHealth <= 0&&!GameIsPaused)
        {
            currentHealth = 0;
            Pause();
            MainMenu.previousLevel = 0;
            gameOverUIHead.SetActive(true);
        }

        if (WarpSword.isBoosted)
        {
            if (!boost)
            {
                StartCoroutine(Speedboost());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused)
            {
                pauseMenuUIHead.SetActive(true);
                Pause();
            }
            else
            {
                pauseMenuUIHead.SetActive(false);
                Resume();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && GameIsPaused == false)
        {
            TakeDamage(20);
            TakeStamina(20);
        }
    }

    public void Resume()
    {
        pauseMenuUIHead.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthStaminaInfo.SetHealth(currentHealth);
    }

    public void GainHealth(int heal)
    {
        currentHealth += heal;

        healthStaminaInfo.SetHealth(currentHealth);
    }

    public void GainStamina(int heal)
    {
        currentStamina += heal;

        healthStaminaInfo.SetStamina(currentStamina);
    }

    public void TakeStamina(int staminaLose)
    {
        currentStamina -= staminaLose;

        healthStaminaInfo.SetStamina(currentStamina);
    }

    public void PowerRestore()
    {
        if(bluePowerDelay != 50)
        {
            bluePowerDelay += 1;
        }
        if (redPowerDelay != 50)
        {
            redPowerDelay += 1;
        }
        if (yellowPowerDelay != 50)
        {
            yellowPowerDelay += 1;
        }
    }

    private IEnumerator Speedboost()
    {
        if (!AttackAOE.berserk)
        {
            this.GetComponent<SpriteRenderer>().material = blueMaterial;
        }
        boost = true;
        trail.Play();
        this.GetComponent<Movement2D>().speed *= 1.5f;
        this.GetComponent<Animator>().speed = 1.5f;
        yield return new WaitForSeconds(5.0f);
        trail.Stop();
        if (!AttackAOE.berserk)
        {
            this.GetComponent<SpriteRenderer>().material = purpleMaterial;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().material = redMaterial;
        }
        this.GetComponent<Animator>().speed = 1f;
        this.GetComponent<Movement2D>().speed /= 1.5f;
        boost = false;
        WarpSword.isBoosted = false;
    }
}
