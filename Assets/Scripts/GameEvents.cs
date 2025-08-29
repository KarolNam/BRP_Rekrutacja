public static class GameEvents
{
    public delegate void OnEnemyKilled(IEnemy enemy);
    public static OnEnemyKilled EnemyKilled;
}

