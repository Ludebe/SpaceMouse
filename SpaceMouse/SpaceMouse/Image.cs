using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using SpaceMouse.Effects;

namespace SpaceMouse
{
    public class Image
    {
        //Atributos
        public Vector2 speed;
        public Vector2 position;
        public Vector2 scale;
        public String fontName, text;

        [XmlElement("path")]
        public string path;

        public float alpha;
        public Boolean isActive;

        [XmlIgnore]
        public Texture2D texture;
        [XmlIgnore]
        public Rectangle sourceRect;

        //Para centrar la imágen y no dibujarla desde arriba a la izquierda
        private Vector2 origin;
        [XmlIgnore]
        private RenderTarget2D renderTarget;
        private SpriteFont font;

        //Se crea uin diccionario con índice String y valor Effect
        private Dictionary<String, ImageEffect> effectDictionary;
        public FadeEffect fadeEffect;
        public SpriteSheetEffect spriteSheetEffect;
        public ArrayList effects;

        /* Van a existir 3 constructores:
         * El primero existe por si se quiere serializar con
         * xml. 
         * El segundo y tercero existen porque en caso de ser
         * más dinámico el tema de las variables, en xml no se podría
         * hacer. La diferencia entre el constructor 2 y 3 es el texto.*/

        //Constructor para serializar
        public Image()
        {
            path = text = String.Empty;
            effects = new ArrayList();
            position = Vector2.Zero;
            fontName = "MyFont";
            scale = Vector2.One;
            alpha = 1.0F;
            sourceRect = Rectangle.Empty;

            effectDictionary = new Dictionary<string, ImageEffect>();
        }

        //Constructor 'dinámico' sin texto
        public Image(String path, Vector2 position, Vector2 scale, float alpha)
        {
            speed = Vector2.Zero;
            this.path = path;
            this.position = position;
            this.scale = scale;
            this.alpha = alpha;

            text = String.Empty;
            effects = new ArrayList();
            fontName = "MyFont";

            effectDictionary = new Dictionary<string, ImageEffect>();
        }

        //Constructor 'dinámico' con texto
        public Image(String path, String text, Vector2 position, String fontName,
                     Vector2 scale, float alpha)
        {
            speed = Vector2.Zero;
            effects = new ArrayList();
            this.path = path;
            this.text = text;
            this.position = position;
            this.fontName = fontName;
            this.scale = scale;
            this.alpha = alpha;

            effectDictionary = new Dictionary<string, ImageEffect>();
        }

        /* Lo que va a hacer este LoadContent, como se sabe, es instanciar variables.
         * La diferencia es que si alguna variable está vacía, la deja así ya
         * que gracias al diseño no se generan Exceptions.
         * Lo que permite la clase Image, además de convertir la textura en una Imagen
         * para poder aprovechar más funciones (scale, alpha, efectos, etc) es que permite
         * también ingresar texto y definir el tamaño con base en si la textura es más
         * grande en dimensiones o lo es el texto. Si el texto es más grande, el 'source' se
         * adapta al texto.
         * 
         * Aclaración:
         * El tema de la spriteFont y el texto.
         * Si no hay texto, se setea como "" o String.Empty que es lo mismo,
         * y si no se le pasa una fuente por parámetro a Image, carga una por default.
         * En realidad si no hay texto, no haría falta ejecutar medio LoadContent(), así
         * que éste método se puede mejorar para no malgastar procesos innecesarios.
         * */
        public void Loadcontent()
        {
            //Si el path no está vacío, carga la textura.
            if (path != String.Empty)
                texture = Game1.Instance.Content.Load<Texture2D>(path);

            //Instancia la fuente y la dimensión que pertenecerá al sourceRect (Área de la image)
            font = Game1.Instance.Content.Load<SpriteFont>("Fonts/" + fontName);

            //Se define el sourceRect
            sourceRect = DefinirSourceRect();

            //Por lo que tengo entendido un render target es un sector en 
            //memoria en la placa de video, todo lo que maneja (mínimo) xna y monogame a la hora
            //de dibujar es un render target (o 2, si es doble buffer).
            //"All a render target is, is a section of graphics memory to which things can be drawn."
            //Ver en el commit de GitHub el link para más info

            //Se setea el renderTarget a sólo el área del sourceRect
            renderTarget = new RenderTarget2D(Game1.Instance.graphics.GraphicsDevice, (int)sourceRect.Width, (int)sourceRect.Height);

            //Se setea el render target de ScreenManager al render recién creado
            Game1.Instance.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            Game1.Instance.graphics.GraphicsDevice.Clear(Color.Transparent);

            //Dibuja la imagen o el texto
            Game1.Instance.spriteBatch.Begin();
            if (texture != null)
                Game1.Instance.spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            if (text != null)
                Game1.Instance.spriteBatch.DrawString(font, text, Vector2.Zero, Color.White);
            Game1.Instance.spriteBatch.End();

            //Acá me cagaste ^~^U
            texture = renderTarget;

            //Al setear el graphicsDevice nulo, vuelve a cargarse el default.
            Game1.Instance.graphics.GraphicsDevice.SetRenderTarget(null);

            //Se setea un efecto al diccionario. Si hay más, agregalos.
            SetImageEffect<FadeEffect>(ref fadeEffect);
            SetImageEffect<SpriteSheetEffect>(ref spriteSheetEffect);

            //Si el String de efectos no está vacío, los activa.
            if (effects.Count != 0)
            {
                foreach (String ef in effects)
                    ActivateImageEffect(ef);
            }
        }

        public void Update(GameTime gametime)
        {
            //Ejecuta los efectos del diccionario que estén activos.
            foreach (var effect in effectDictionary)
            {
                if (effect.Value.isActive)
                    effect.Value.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Si pasamos una position x=100 y=100, la punta suprerior izquierda 
            //sería la que esté en esa posición realmente.
            //Lo que hace origin es que esas coordenadas en position
            //sean el centro de la imágen.
            origin = new Vector2(sourceRect.X / 2, sourceRect.Y / 2);
            spriteBatch.Draw(texture, position + origin, sourceRect, Color.White * alpha,
                0.0F, origin, scale, SpriteEffects.None, 0.0F);
        }


        // |------------------Métodos-Funciones---------------------|

        /* Se cambian los valores del effect pasado por parámetro
         * sino es que se cambia el mismo.
         * Como se pasa por referencia, cualquier efecto que le 
         * pasemos, si lo cambiamos también se cambia en el diccionario,
         * por eso lo dejamos como público en el caso de FadeEffect
         * */
        private void SetImageEffect<T>(ref T effect)
        {
            //Si el efecto es nulo, lo instancia con Activator.
            if (effect == null)
                effect = (T)Activator.CreateInstance(typeof(T));

            //Sino, lo carga.
            else
            {
                var obj = this;
                (effect as ImageEffect).LoadContent(ref obj);
            }

            //Al final, lo agrega al diccionario.
            effectDictionary.Add(effect.GetType().ToString().Replace("SpaceMouse.", String.Empty), (effect as ImageEffect));
        }

        /* Se activa un efecto del diccionario de efectos
         * */
        public void ActivateImageEffect(String effects)
        {
            //Activa los que estén el el String effects. (Por ejemplo, en la clase SplashScreen
            //se crea la imagen, y luego se le setea effects = "ImageEffects.FadeEffect". 
            //Si "ImageEffects.FadeEffect está en el diccionario, lo activa.)
            effects = "ImageEffects." + effects;
            if (effectDictionary.ContainsKey(effects))
            {
                effectDictionary[effects].isActive = true;
                var obj = this;
                effectDictionary[effects].LoadContent(ref obj);
            }
        }

        /* Bueno... te imaginas (?
         * */
        public void DeactivateImageEffect(String effect)
        {
            //blabla
            if (effectDictionary.ContainsKey(effect))
            {
                effectDictionary[effect].isActive = false;
                //Unload content no necesario. Garbage colector OP
            }
        }

        /* Se encarga de instanciar correctamente el sourceRect
         * calculando las medidas de la textura y/o del texto*/
        private Rectangle DefinirSourceRect()
        {
            Rectangle sourceRect = new Rectangle();

            Vector2 dimensions = Vector2.Zero;
            if (texture != null)
                dimensions.X += texture.Width;

            //MeasureString devuelve un Vector2 con el Width y Height del texto 
            //teniendo en cuenta las propiedades de la fuente
            dimensions.X += font.MeasureString(text).X;

            if (texture != null)
                dimensions.Y += Math.Max(texture.Height, font.MeasureString(text).Y);

            else
                dimensions.Y += font.MeasureString(text).Y;

            if (sourceRect == Rectangle.Empty)
                sourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            return sourceRect;
        }
    }
}
