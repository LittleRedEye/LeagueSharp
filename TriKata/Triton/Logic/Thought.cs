namespace Triton.Logic
{
    public abstract class Thought
    {
        public abstract bool ShouldActualize(object context);
        public abstract void Actualize(object context);
    }
}