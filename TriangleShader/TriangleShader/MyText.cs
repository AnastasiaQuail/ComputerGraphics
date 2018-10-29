using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Windows;
namespace GameFramework
{
    class MyTextWriter
    {
        private SharpDX.DirectWrite.Factory fontFactory;
        private TextFormat textFormat;
        private SolidColorBrush textBrush;
        RenderTarget renderTarget;
        Vector2 start;
        float offset;

        public MyTextWriter(RenderTarget renderTarget, Vector2 start)
        {
            this.renderTarget = renderTarget;
            // Create the DirectWrite factory object.
            fontFactory = new SharpDX.DirectWrite.Factory();
            textFormat = new TextFormat(fontFactory, "Segoe UI", 14);
            textBrush = new SharpDX.Direct2D1.SolidColorBrush(renderTarget, SharpDX.Color4.White);
            offset = 20;
        }
        public void SetTextFormat(string font,float fontSize)
        {
            textFormat = new TextFormat(fontFactory,font,fontSize );
        }
        public void WriteLine(List<String> text)
        {
            int k = 0;
            foreach (string s in text)
            {
                TextLayout textLayout = new TextLayout(fontFactory, s, textFormat, 450.0f, 100.0f);
                renderTarget.DrawTextLayout(new Vector2(10, 10 + offset*k), textLayout, textBrush);
                k++;
            }
        }

    }
}
