﻿using GameInterfaces.GraphicsInterface;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.MonoGameGraphics
{
    public class MonoGameGraphicsContext : IGraphicsContext<RenderTarget2D, Texture2D>
    {
        public readonly GraphicsDevice device;
        public readonly SpriteBatch spriteBatch;

        private RenderTarget2D lastRenderTarget = null;

        //Constructors
        public MonoGameGraphicsContext(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            this.device = device;
            this.spriteBatch = spriteBatch;
        }

        private void ChangeRenderTarget(RenderTarget2D surface)
        {
            if (lastRenderTarget != surface)
            {
                device.SetRenderTarget(surface);
                if (surface != null) device.Clear(new Color(255, 255, 255, 0));
                lastRenderTarget = surface;
            }
        }

        //Clear surface
        public void Clear(RenderTarget2D surface, System.Drawing.Color color)
        {
            ChangeRenderTarget(surface);
            device.Clear(new Color(color.R, color.G, color.B, color.A));
        }
        public void Clear(System.Drawing.Color color) => Clear(null, color);

        //Create a target
        public RenderTarget2D CreateTarget(int width, int height) {
            RenderTarget2D target = new RenderTarget2D(device, width, height);
            ChangeRenderTarget(target);
            return target;
        }

        //Create a source from target
        public Texture2D TargetToSource(RenderTarget2D target) => target;

        //Draw texture
        public void Draw(Texture2D subject, RenderTarget2D target, System.Drawing.Rectangle destination) => Draw(subject, target, destination, null);
        public void Draw(Texture2D subject, RenderTarget2D target, System.Drawing.Rectangle destination, System.Drawing.Rectangle? source)
        {
            ChangeRenderTarget(target);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(subject, DrawingToXna(destination), source.HasValue ? DrawingToXna(source.Value) : (Rectangle?) null, Color.White);
            spriteBatch.End();
        }

        //Retrieve size of texture
        public System.Drawing.Size GetSize(Texture2D surface) => new System.Drawing.Size(surface.Width, surface.Height);

        //Scale texture
        public RenderTarget2D Scale(RenderTarget2D surface, double scaleX, double scaleY, bool antialias)
        {
            System.Drawing.Size newSize = new System.Drawing.Size((int)(surface.Width * scaleX), (int)(surface.Height * scaleY));
            RenderTarget2D newTarget = CreateTarget(newSize.Width, newSize.Height);
            Draw(surface, newTarget, new System.Drawing.Rectangle(new System.Drawing.Point(0,0), newSize));
            return newTarget;
        }
        public RenderTarget2D Scale(RenderTarget2D surface, double scale, bool antialias) => Scale(surface, scale, scale, antialias);

        //Update screen
        public void Update(RenderTarget2D surface) => Draw(surface, null, new System.Drawing.Rectangle(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight));
        public void Update(RenderTarget2D surface, System.Drawing.Rectangle destination)
        {
            //System.Console.WriteLine(destination);
            Draw(surface, null, destination);
        }

        //Dispose surfaces
        public void DisposeSurface(RenderTarget2D surface) => surface.Dispose();
        public void DisposeSource(Texture2D source) => source.Dispose();

        //Utility conversion from System.Drawing.Rectangle to Microsoft.Xna.Framework
        private static Rectangle DrawingToXna(System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void Reset() => ChangeRenderTarget(null);

        //Redirection of subclasses of Texture2D (as type parameter 1 extends type parameter 2)
        System.Drawing.Size IGraphicsContext<RenderTarget2D, Texture2D>.GetSize(RenderTarget2D surface) => GetSize(surface);
        void IGraphicsContext<RenderTarget2D, Texture2D>.Draw(RenderTarget2D subject, RenderTarget2D target, System.Drawing.Rectangle destination) => Draw(subject, target, destination);
        void IGraphicsContext<RenderTarget2D, Texture2D>.Draw(RenderTarget2D subject, RenderTarget2D target, System.Drawing.Rectangle destination, System.Drawing.Rectangle? source) => Draw(subject, target, destination, source);
    }
}
