using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace SpaceMouse.Effects
{
    public class SpriteSheetEffect : Effects.ImageEffect
    {
        /* Atributos */
        public float FrameCounter;                   //???
        public const int SWITCH_FRAME = 100;        //Velocidad del cambio de frame

        public Vector2 currentFrame;                 //Frame (posición en X/Y) actual
        public Vector2 amountOfFrames;               //Cantidad total de frames (fila-columna) del sprite

        //Ancho de cada frame
        public int FrameWidth
        {
            get
            {
                if (image.texture != null)
                    return image.texture.Width / (int)amountOfFrames.X;
                return 0;
            }
        }

        //Alto de cada frame
        public int FrameHeight
        {
            get
            {
                if (image.texture != null)
                    return image.texture.Height / (int)amountOfFrames.Y;
                return 0;
            }
        }

        /*
         * Constructor
         * 
         */
        public SpriteSheetEffect()
        {
            //Cantidad de frames que tiene el sprite
            amountOfFrames = new Vector2(4, 1);
            //Posicion actual (Frame) del sprite
            currentFrame = new Vector2(1, 0);
            //Contador
            FrameCounter = 0;
        }

        public override void LoadContent(ref Image image)
        {
            base.LoadContent(ref image);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Si la imagen está activa (se tocan teclas, etc...)
            if (image.isActive)
            {
                //Cambia el n° de frame
                FrameCounter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                //Cambia el frame (posición en X)
                if (FrameCounter >= SWITCH_FRAME)
                {
                    FrameCounter = 0;
                    currentFrame.X++;

                    //Si se pasó de los límites lo reinicia
                    if (currentFrame.X * FrameWidth >= image.texture.Width - FrameWidth)
                        currentFrame.X = 1;
                }
            }
            else
            {
                if (Player.Instance.Jumping)
                    currentFrame.X = 3;
                else
                    currentFrame.X = 0;

                FrameCounter = 0;
            }

            //Dibuja el sprite en la posición correspondiente
            image.sourceRect = new Rectangle((int)currentFrame.X * FrameWidth, (int)currentFrame.Y * FrameHeight,
                                            FrameWidth, FrameHeight);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
