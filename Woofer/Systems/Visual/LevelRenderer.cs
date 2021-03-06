﻿using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Scenes;

using GameInterfaces.Controller;

namespace WooferGame.Systems.Visual
{
    [ComponentSystem("level_renderer", ProcessingCycles.Render),
        Watching(typeof(LevelRenderable))]
    public class LevelRenderer : ComponentSystem
    {

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            /*var bg = r.GetLayerGraphics("background");
            bg.Clear(System.Drawing.Color.FromArgb(32, 255, 0, 255));
            bg.Complete();*/

            var layer = r.GetLayerGraphics("level");

            CameraView view = Owner.CurrentViewport;

            foreach (LevelRenderable tile in WatchedComponents)
            {
                tile.Owner.Components.Get<Renderable>().Render(layer, view, r);
            }

            layer.Complete();
        }
    }
}
