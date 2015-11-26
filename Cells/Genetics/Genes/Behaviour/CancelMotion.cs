namespace Cells.Genetics.Genes.Behaviour
{
    public class CancelMotion
    {
        public byte Marker { get { return 0x00; } }
        public int Size { get { return 2; } }

        public int Run(Individual individual, float deltaTime)
        {
            individual.Force = (-individual.Velocity / deltaTime) * individual.Mass;
            return 0;
        }
    }
}
