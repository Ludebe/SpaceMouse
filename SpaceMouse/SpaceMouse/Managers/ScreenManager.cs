using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpaceMouse.Screens;

namespace SpaceMouse.Managers
{
    public class ScreenManager
    {
        //Atributos
        private static ScreenManager instance;
        public Vector2 dimensions;
        private Screen currentScreen;
        private Screen oldScreen;
        public Boolean isTransitioning;

        //Constructor privado
        private ScreenManager()
        {
            //La primera pantalla del juego van a ser las imágenes de presentación (SplashScreens)
            currentScreen = new SplashScreen();
            oldScreen = currentScreen;

            dimensions.X = 800;
            dimensions.Y = 600;

            SetDimensions(dimensions);
        }

        public static ScreenManager Instance
        {
            get
            {
                //Si instance es null, crea una nueva instancia usando el constructor privado
                if (instance == null)
                {
                    //Se instancia desde el constructor
                    instance = new ScreenManager();


                }
                return instance;
            }
        }

        // |-----------------Métodos comunes-------------------|

        public void LoadContent(ContentManager Content)
        {
            currentScreen.LoadContent(Content);
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            InputManager.Instance.Update();
            Transition(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }

        // |------------Métodos propios-----------------------|

        public void ChangeScreen(String screenName)
        {
            currentScreen = (Screen)Activator.CreateInstance(Type.GetType("SpaceMouse.Screens." + screenName));
            isTransitioning = true;
        }

        private void Transition(GameTime gameTime)
        {
            if (isTransitioning)
            {
                //Acá iría el fade in-out en Image.Update()
                oldScreen = currentScreen;
                currentScreen.LoadContent(Game1.Instance.Content);
                isTransitioning = false;
            }
        }

        // |-------------Getters & Setters-------------------|

        public Vector2 GetDimensions()
        {
            return this.dimensions;
        }

        public void SetDimensions(Vector2 dimensions)
        {
            this.dimensions = dimensions;

            //Si sólo cambiamos las dimensiones no pasa nada, hay que aplicar cambios y cambiar el PreferedBackBuffer
            Game1.Instance.graphics.PreferredBackBufferWidth = Convert.ToInt32(dimensions.X);
            Game1.Instance.graphics.PreferredBackBufferHeight = Convert.ToInt32(dimensions.Y);
            Game1.Instance.graphics.ApplyChanges();
        }
    }
}
