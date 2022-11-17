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
    //This class uses two SOLID principles:
    //Single Responsibilty: This class is only responsible for one thing, loading content. The only reason it would ever change is to change how content is loaded.
    //Open/Closed: This class can be added to in cases where additional content needs to be loaded in another way, so that it still adheres to single responsibility, but that shouldn't require modification of any existing code.
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
