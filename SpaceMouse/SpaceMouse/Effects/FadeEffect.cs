using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace SpaceMouse.Effects
{
    public class FadeEffect : Effects.ImageEffect
    {
        //Atributos

        //Velocidad de fade
        public float fadeSpeed;
        //El tiempo en segundos en que se dentendrá el fade effect
        //(Sirve como por ejemplo para que las splash image queden mostrándose
        //en pantalla x segundos, y no se muestre y al toque haga fadeout)
        public float pauseTimeSeconds;
        //Boolean de control
        private Boolean increasing;
        //Si es true (el por defecto), la pausa se realiza cuando Alpha=1. sino en alpha=0
        public Boolean pausaEnAlphaUno;
        //Boolean de control
        private Boolean onPause;
        //Contador para el onPause
        private float onPauseCounter;

        public FadeEffect()
        {
            //Default
            fadeSpeed = 1.0F;
            pauseTimeSeconds = 0;
            increasing = false;
            onPause = false;
            pausaEnAlphaUno = true;
        }

        public override void LoadContent(ref Image image)
        {
            base.image = image;
        }

        /* Este método lleva a cabo los cambios de valores
         * en alpha para el efecto Fade.
         * */
        public override void Update(GameTime gameTime)
        {
            // Lo hace sólo si la imagen está activa. Sino, alpha = 1.0
            if (image.isActive)
            {
                //Si no está en la pausa, incrementa-decrementa el alpha
                if (!onPause)
                {
                    if (increasing && !onPause)
                        image.alpha += fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else
                        image.alpha -= fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    //Si alcanza 1.0 o 0.0, cambia el increasing
                    if (image.alpha >= 1.0F)
                    {
                        increasing = false;
                        image.alpha = 1.0F;

                        if (pausaEnAlphaUno)
                            onPause = true;
                    }

                    else if (image.alpha <= 0.0F)
                    {
                        increasing = true;
                        image.alpha = 0.0F;
                        if (!pausaEnAlphaUno)
                            onPause = true;
                    }
                }

                //Si está en pausa
                else
                {
                    //El contador de pausa se va sumando hasta alcanzar el tiempo límite de pausa (pauseTimeSeconds)
                    onPauseCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (onPauseCounter >= pauseTimeSeconds)
                    {
                        onPause = false;
                        onPauseCounter = 0;
                    }
                }
            }


            else
                image.alpha = 1.0F;
        }
    }
}
