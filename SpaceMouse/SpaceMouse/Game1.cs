using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using SpaceMouse.Managers;

namespace SpaceMouse
{
    public class Game1 : Game
    {
        //Atributos
        private static Game1 instance;
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;


        /* Constructor privado
         * */
        private Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /* El get de la instancia del Game1
         * */
        public static Game1 Instance
        {
            get
            {
                //Si instance es null, crea una nueva instancia usando el constructor privado
                if (instance == null)
                {
                    instance = new Game1();
                }
                return instance;
            }
        }

        /* Sirve para inicializar variables. Según Ortega no se usa
         *
         * No lo vamos a usar
         **/
        protected override void Initialize()
        {
            base.Initialize();
        }

        /* Acá se inicializan variables contenido (Imagenes, sonidos, etc) con
         * ContentManager Content. 
         * Ejemplo: Tenemos un a variable Texture2D imagenPJ y la queremos cargar.
         * El archivo .png está en Content//Sprites y se llama PJImagen
         * imagenPJ = Content.Load<Texture2D>("Sprites//PJImagen");
         * Easy. 
         * Al agregar archivos a la carpeta content, hay que hacer click sobre
         * los mismos y setear la acción de compilación a Contenido y Copiar en el directorio
         * a copiar siempre. (Copiar si funciona, aveces)
         * */
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Instance.LoadContent(Content);
        }

        /* Sirve para disposear contenido. Nunca lo usé.
         *
         * No lo vamos a usar
         **/
        protected override void UnloadContent()
        {

        }

        /* Esto es un loop. 1 ciclo = 1 Frame
         * Sirve para llevar a cabo operaciones lógicas
         * */
        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        /* Draw simplemente dibuja con el SpriteBatch
         * spriteBatch texturas. En google si nos fijamos
         * hay muchas formas en las que recibe parámetros,
         * pudiendo así actuar de maneras diferentes al 
         * dibujar una textura, como su opacidad, su rotación, su escala,
         * su color, etc.
         * Para que Draw dibuje, tiene que haber un spriteBatch.Draw(...)
         * entre un spriteBatch.Begin() y un spriteBatch.End()
         * */
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            ScreenManager.Instance.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
