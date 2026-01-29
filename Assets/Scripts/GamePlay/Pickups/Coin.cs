public class Coin : LevelItem // 继承 LevelItem
{
    private void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 【关键】调用父类的记录方法
            OnInteract();
        }
    }
}