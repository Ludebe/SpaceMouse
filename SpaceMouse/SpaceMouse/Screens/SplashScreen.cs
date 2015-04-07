using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpaceMouse.Managers;
using SpaceMouse.Screens;

namespace SpaceMouse.Screens
{
    /*Aclaración 1: Para que el juego cargue correctamente las
     * SplashImages se tienen que llamar como archivo 'SplashImageN',
     * siendo la última letra N el número de splash. (Se empieza desde 0)
     *  Ejemplo: SplashImage0 - SplashImage1 - SplashImage2*/
    public class SplashScreen : Screen
    {
        //Path base de las SplashImages
        public String imagesPath;
        private int splashImageNumber;
        private int splashImageNumberLimit;

        private MouseState oldMouseState;
        private MouseState newMouseState;


        //La imágen negra
        private Image fadeImage;
        //Implementación Image
        private List<Image> splashImages;


        public SplashScreen()
        {
            splashImages = new List<Image>();
            imagesPath = "SplashImages/SplashImage";
            newMouseState = Mouse.GetState();
            oldMouseState = newMouseState;
        }

        /* Se cargan todas las SplashImages de la carpeta en un while(true)
         * y cuando se genera un ContentLoadException significará que ya no hay imágenes
         * que cargar, por lo tanto finaliza la carga de contenido.
         * 
         * ------------------------------------------------------
         * 
         * Ahora en vez de cargar el contenido como Texture2D y guardarlo en un ArrayList
         * de texture2Ds se crean instancias de Image y se llama a su método LoadContent()
         * para que image se haga cargo. Y al final, en el Draw de ésta clase
         * se llama al Draw de la image, ya no se dibuja la textura manualmente desde 
         * esta clase para poder así tener la posibilidad de manejar cosas como
         * el alpha, el scale, etc.
         * */
        public override void LoadContent(ContentManager Content)
        {
            try
            {
                splashImageNumber = 0;
                while (true)
                {
                    Image image = new Image(imagesPath + splashImageNumber.ToString(), Vector2.Zero, Vector2.One, 1.0F);

                    image.Loadcontent();
                    splashImages.Add(image);
                    splashImageNumber++;
                }
            }

            catch (ContentLoadException)
            {
                splashImageNumberLimit = splashImageNumber - 1;
                splashImageNumber = 0;
                fadeImage = new Image("Fade", Vector2.Zero, Vector2.One, 1.0F);
                //No se pone solo FadeEffect, se pone las subcarpetas con puntos y el efecto al final
                //Estuve como 25 min para darme cuenta -.-" t-t En el video no lo dice porque no lo tiene en subfolder
                fadeImage.effects.Add("ImageEffects.FadeEffect");
                fadeImage.isActive = true;

                //Las modificaciones que forman parte del 
                //efecto de la imagen se realizan después del LoadContent()
                fadeImage.Loadcontent();
                fadeImage.fadeEffect.fadeSpeed = 0.5F;
                fadeImage.fadeEffect.pauseTimeSeconds = 3.0F;
                fadeImage.fadeEffect.pausaEnAlphaUno = false;
            }
        }

        /*Se encarga del fadeIn y fadeOut de las splashImages
         * */
        public override void Update(GameTime gameTime)
        {
            newMouseState = Mouse.GetState();
            if (splashImages.Count == 0)
                ChangeSplashImage();

            //No necesario porque las splashimages no tienen efectos, 
            //lo comento para no gastar proceso.
            //splashImages[splashImageNumber].Update(gameTime);
            fadeImage.Update(gameTime);

            //Cambia de splashImage al tocar enter o luego del fade
            if (InputManager.Instance.KeyReleased(Keys.Enter) || (oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Pressed) || fadeImage.alpha == 1.0F)
            {
                ChangeSplashImage();
            }


            oldMouseState = newMouseState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Llama al método Draw de la Image correspondiente
            if (splashImages.Count != 0)
                splashImages[splashImageNumber].Draw(spriteBatch);

            fadeImage.Draw(spriteBatch);
        }

        /* Cambia de splashImage, si se supea el límite, se va
         * al MenuScreen*/
        private void ChangeSplashImage()
        {
            if(splashImages.Count != 0)
                splashImages[splashImageNumber].isActive = false;

            //Para saltearse la splashimage actual y continúe con la siguiente
            fadeImage.alpha = 1.0F;
            splashImageNumber++;

            //Si el índice se pasa del límite, cambia de SplashScreen a MenuScreen
            if (splashImageNumber > splashImageNumberLimit)
            {
                ScreenManager.Instance.ChangeScreen("MenuScreen");
            }
        }
    }
}
