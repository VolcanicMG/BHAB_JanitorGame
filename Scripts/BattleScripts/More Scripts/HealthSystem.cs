using System;

public class HealthSystem 
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDead;

    private int healthMax;
    private int health;

    private bool Dead = false;

    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        health = healthMax;
    }

    public void SetHealthAmount(int health)
    {
        this.health = health;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercent()
    {
        return (float)health / healthMax;
    }

    public int GetHealthAmount()
    {
        return health;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            health = 0;
            Dead = true;
        }
        if (health > healthMax) health = healthMax;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public bool isDead()
    {
        return Dead;
    }
}
