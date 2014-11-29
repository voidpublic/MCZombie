using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCForge.GUI.Windows_7
{
    public partial class ColoredTextBox : RichTextBox
    {
        public ColoredTextBox()
        {
            InitializeComponent();
            wList = new List<string>();
        }
        private List<string> wList;
        public ColoredTextBox(IContainer container)
        {
            container.Add(this);
            wList = new List<string>();
            InitializeComponent();
        }
        private const string TheColorsOfTheRainbow =
          @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\colortbl;\red0\green0\blue0;\red255\green255\blue0;\red0\green0\blue139;\red0\green100\blue0;\red0\green139\blue139;\red139\green0\blue139;\red128\green128\blue128;\red255\green215\blue0;\red169\green169\blue169;\red0\green0\blue255;\red0\green128\blue0;\red0\green255\blue255;\red255\green0\blue255;\red255\green255\blue255;\red170\green0\blue170;\red139\green0\blue0;}{\fonttbl{\f0\fnil\fcharset0 Calibri;}}\viewkind4\uc1\pard\f0\fs17}";



        private void ColorText(string text)
        {
            if (!text.Contains("&") && !text.Contains("%"))
            {
                text += "\\cf0\\par";
                wList.Add(text);
                WriteToTextBox();
                return;
            }
            var split = text.Split('&', '%');
            var builder = new StringBuilder();

            for (int index = 0; index < split.Length; index++)
            {
                string elString = split[index];
                if (String.IsNullOrEmpty(elString))
                    continue;
                string color = GetColor(elString[0]);
                if (String.IsNullOrEmpty(color))
                {
                    builder.Append(elString);
                    continue;
                }
                elString = elString.Substring(1);
                elString = color + elString;
                builder.Append(elString);
            }
            builder.Append("\\cf0\\par");
            wList.Add(builder.ToString());
            WriteToTextBox();

        }

        private void WriteToTextBox()
        {
            if (wList.Count > 200) wList.RemoveAt(0);
            string all = TheColorsOfTheRainbow.Remove(TheColorsOfTheRainbow.Length - 1);
            all = wList.Aggregate(all, (current, s) => current + s);
            all += '}';
            Rtf = all;
        }

        private string GetColor(char p)
        {
            switch (p)
            {
                case 'e': return "\\cf2";
                case '1': return "\\cf3";
                case '2': return "\\cf4";
                case '3': return "\\cf5";
                case '4': return "\\cf6";
                case '5': return "\\cf15";
                case '7': return "\\cf7";
                case '6': return "\\cf8";
                case '8': return "\\cf9";
                case '9': return "\\cf10";
                case 'a': return "\\cf11";
                case 'b': return "\\cf12";
                case 'c': return "\\cf16";
                case 'd': return "\\cf13";
                case 'f': return "\\cf14";
                default: return null;
            }
        }
    }
}
