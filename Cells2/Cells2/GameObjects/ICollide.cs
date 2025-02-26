namespace Cells.GameObjects
{
    public interface ICollide
    {
        void HandleCollision(GameObject other, float deltaTime);
    }
}