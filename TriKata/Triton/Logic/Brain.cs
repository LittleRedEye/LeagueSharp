using System;
using System.Collections.Generic;

namespace Triton.Logic
{
    public class Brain
    {
        List<Thought> _thoughts = new List<Thought>();

        public Brain()
        {
        }

        public void Think(object context)
        {
            try
            {
                foreach (var thought in _thoughts)
                {
                    if (thought.ShouldActualize(context))
                    {
                        thought.Actualize(context);

                        if (!(thought is ParallelThought))
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public List<Thought> Thoughts
        {
            get { return _thoughts; }
        }
    }
}
