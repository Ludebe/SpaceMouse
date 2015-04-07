using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace SpaceMouse.Effects
{
    public class ImageEffect
    {
        public Image image;
        public Boolean isActive;
        public ImageEffect()
        {
            isActive = false;
        }

        public virtual void Initializate()
        {

        }

        public virtual void LoadContent(ref Image image)
        {
            this.image = image;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw()
        {

        }
    }
}
