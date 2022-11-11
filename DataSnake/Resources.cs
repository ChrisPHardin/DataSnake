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

    public class Resources : Game
    {

        public Texture2D LoadTexture(string textureName, Game gameVar)
        {
            Content.RootDirectory = "Content";
            Texture2D texToReturn;
            texToReturn = gameVar.Content.Load<Texture2D>(textureName);
            return texToReturn;
        }

        public SoundEffect LoadSFX(string sfxName, Game gameVar)
        {
            Content.RootDirectory = "Content";
            SoundEffect sfxToReturn;
            sfxToReturn = gameVar.Content.Load<SoundEffect>(sfxName);
            return sfxToReturn;
        }

        public Song LoadSong(string songName, Game gameVar)
        {
            Content.RootDirectory = "Content";
            Song songToReturn;
            songToReturn = gameVar.Content.Load<Song>(songName);
            return songToReturn;
        }

        public SpriteFont LoadFont(string fontName, Game gameVar)
        {
            Content.RootDirectory = "Content";
            SpriteFont fontToReturn;
            fontToReturn = gameVar.Content.Load<SpriteFont>(fontName);
            return fontToReturn;
        }
    }
}
