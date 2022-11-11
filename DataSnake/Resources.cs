using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace DataSnake
{

    public class Resources<T> : Game
    {

        public T LoadContent(string contentName, Game gameVar)
        {
            Content.RootDirectory = "Content";
            T content;
            content = gameVar.Content.Load<T>(contentName);
            return content;
        }
    }
}
