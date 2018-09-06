﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Events;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;

namespace EntityComponentSystem.ComponentSystems
{
    public abstract class ComponentSystem
    {
        private static readonly Dictionary<Type, string> identifierCache = new Dictionary<Type, string>();

        public string SystemName { get; private set; }

        private const byte _input = 0b001;
        private const byte _tick = 0b010;
        private const byte _render = 0b100;

        public Scene Owner { get; set; }
        public string[] Watching { get; protected set; } = new string[0];
        public string[] Listening { get; protected set; } = new string[0];
        private byte _processing_loop = 0;

        public bool InputProcessing
        {
            get => (_processing_loop & _input) != 0;
            set => _processing_loop = (byte)(value ? (_processing_loop | _input) : (_processing_loop & ~_input));
        }
        public bool TickProcessing
        {
            get => (_processing_loop & _tick) != 0;
            set => _processing_loop = (byte)(value ? (_processing_loop | _tick) : (_processing_loop & ~_tick));
        }
        public bool RenderProcessing
        {
            get => (_processing_loop & _render) != 0;
            set => _processing_loop = (byte)(value ? (_processing_loop | _render) : (_processing_loop & ~_render));
        }

        protected List<Component> WatchedComponents = new List<Component>();
        
        public ComponentSystem()
        {
            try
            {
                this.SystemName = IdentifierOf(this.GetType());
            }
            catch (Exception ex)
            {
                throw new AttributeException(
                    $"Required attribute of class 'ComponentSystemAttribute' not found in derived class '{GetType()}'", ex);
            }
        }

        public virtual void RemoveWatchedComponent(Component component)
        {
            WatchedComponents.Remove(component);
        }

        public virtual void AddWatchedComponent(Component component)
        {
            WatchedComponents.Add(component);
        }

        public virtual void EventFired(object sender, Event e)
        {
        }

        public virtual void Input()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
        }

        public static string IdentifierOf<T>() where T : ComponentSystem
        {
            return IdentifierOf(typeof(T));
        }

        public static string IdentifierOf(Type type)
        {
            if (!identifierCache.ContainsKey(type)) identifierCache[type] = (type.GetCustomAttributes(typeof(ComponentSystemAttribute), false).First() as ComponentSystemAttribute).SystemName;
            return identifierCache[type];
        }
    }

    public sealed class ComponentSystemAttribute : Attribute
    {
        public readonly string SystemName;

        public ComponentSystemAttribute(string systemName)
        {
            SystemName = systemName;
        }
    }
}
