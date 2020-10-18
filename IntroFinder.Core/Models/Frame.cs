﻿namespace IntroFinder.Core.Models
{
    internal class Frame
    {
        public Frame(byte[] data, int position)
        {
            Data = data;
            Position = position;
        }

        public byte[] Data { get; }

        public int Position { get; }
    }
}