using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

namespace _42run.OpenGL
{
    public class SpriteSheet
    {
        public int Id { get; private set; } = -1;

        public SpriteSheet(string path, int spriteWidth, int spriteHeight)
        {
            Id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2DArray, Id);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Bitmap image = new Bitmap(path);
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int columnsCount = image.Width / spriteWidth;
            int rowsCount = image.Height / spriteHeight;

            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.Rgba8, spriteWidth, spriteHeight, columnsCount * rowsCount);

            GL.PixelStore(PixelStoreParameter.UnpackRowLength, image.Width);

            for (int i = 0; i < columnsCount * rowsCount; i++)
            {
                GL.TexSubImage3D(TextureTarget.Texture2DArray,
                                 0, 0, 0, i, spriteWidth, spriteHeight, 1,
                                 OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte,
                                 imageData.Scan0 + (spriteWidth * 4 * (i % columnsCount)) + (image.Width * 4 * spriteHeight * (i / columnsCount)));
            }
            image.UnlockBits(imageData);
        }
    }
}
