using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.UI
{
    public abstract class InteractableUIElement : UIElement
    {
        public InteractableUIElement(int y, int x) : base(y, x)
        {
        }

        /// <summary>
        /// Updates the logic of the InteractableUIElement.
        /// </summary>
        public abstract void Update();
    }
}
