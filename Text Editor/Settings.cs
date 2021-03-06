﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Editor
{
    class Settings
    {
        private static Theme current = Theme.NIGHTMARE;
        public static readonly int TEXT_SIZE = 10;
        public static readonly bool SMART_TYPING = true;
        public static readonly String TAB = "  ";
        public static readonly int DOUBLE_CLICK = 5;

        public enum Theme
        {
            DEFAULT,
            NIGHTMARE
        }

        public enum Purpose
        {
            BACKGROUND,
            TEXT,
            CURSOR,
            TEXT_SELECTION,
            LINE_HIGHLIGHT,
            TAB,
            SELECTED_TAB,
            // SYNTAX HIGHLIGHTING
            SYNTAX1,
            SYNTAX2,
            STRING,
            NUM,
            COMMENT
        }

        public static void setTheme(Theme preset)
        {
            current = preset;
        }

        public static Color getColor(Purpose purpose)
        {
            switch (current)
            {
                case Theme.DEFAULT:
                    switch (purpose)
                    {
                        case Purpose.BACKGROUND:
                            return Color.FromArgb(255, 255, 255);
                        case Purpose.TEXT:
                        case Purpose.CURSOR:
                            return Color.FromArgb(0, 0, 0);
                        case Purpose.TEXT_SELECTION:
                            return Color.FromArgb(200, 100, 100, 255);
                        case Purpose.LINE_HIGHLIGHT:
                            return Color.FromArgb(30, 0, 0, 0);
                        case Purpose.TAB:
                            return Color.FromArgb(100, 100, 100);
                        case Purpose.SELECTED_TAB:
                            return Color.FromArgb(180, 180, 180);
                        case Purpose.SYNTAX1:
                            return Color.FromArgb(255, 0, 0);
                        case Purpose.SYNTAX2:
                            return Color.FromArgb(0, 200, 0);
                        case Purpose.STRING:
                            return Color.FromArgb(200, 0, 200);
                        case Purpose.NUM:
                            return Color.FromArgb(0, 200, 0);
                        case Purpose.COMMENT:
                            return Color.FromArgb(200, 200, 0);
                    }
                    break;
                case Theme.NIGHTMARE:
                    switch (purpose)
                    {
                        case Purpose.BACKGROUND:
                            return Color.FromArgb(0, 0, 0);
                        case Purpose.TEXT:
                        case Purpose.CURSOR:
                            return Color.FromArgb(255, 255, 255);
                        case Purpose.TEXT_SELECTION:
                            return Color.FromArgb(200, 255, 100, 0);
                        case Purpose.LINE_HIGHLIGHT:
                            return Color.FromArgb(30, 255, 0, 0);
                        case Purpose.TAB:
                            return Color.FromArgb(100, 0, 0);
                        case Purpose.SELECTED_TAB:
                            return Color.FromArgb(255, 0, 0);
                        case Purpose.SYNTAX1:
                            return Color.FromArgb(255, 0, 0);
                        case Purpose.SYNTAX2:
                            return Color.FromArgb(0, 200, 0);
                        case Purpose.STRING:
                            return Color.FromArgb(200, 0, 200);
                        case Purpose.NUM:
                            return Color.FromArgb(0, 200, 0);
                        case Purpose.COMMENT:
                            return Color.FromArgb(200, 200, 0);
                    }
                    break;
            }
            return Color.FromArgb(0, 0, 0);
        }

        public static String inlineComment(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.PROLOG:
                    return "%";
                default:
                    return "//";
            }
        }

        public static List<String>[] keywords(FileType fileType)
        {
            List<String>[] kw = new List<string>[0];

            switch (fileType)
            {
                case FileType.BAREBONES:
                    kw = new List<string>[2];
                    kw[0] = new List<string>()
                    { "if", "else", "while", "float", "int", "loop", "class", "String", ";",
                      "bool", "true", "false", "function", "field", "var", "return", "void", "include" };
                    kw[1] = new List<string>() { "print", "next", "start", "random" };
                    break;
                case FileType.BONES:
                    kw = new List<string>[1];
                    kw[0] = new List<string>()
                    { "if", "else", "while", "float", "int", "for", "class", "string", ";",
                      "bool", "true", "false", "return", "void", "import" };
                    break;
                case FileType.WACC:
                    kw = new List<string>[1];
                    kw[0] = new List<string>()
                    { "if", "true", "false", "null", "len", "ord", "char", "string", "chr",
                      "bool", "int", "skip", "read", "free", "return", "exit", "print",
                      "println", "while", "then", "else", "fi", "do", "done", "begin", "end",
                      "newpair", "call", "fst", "snd", "pair" };
                    break;
                case FileType.CSHARP:
                    kw = new List<string>[2]; //1
                    kw[0] = new List<string>()
                    { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
                        "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
                        "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
                        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
                        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct",
                        "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
                        "virtual", "void", "volatile", "while", ";", "," };
                    // this is questionable
                    kw[1] = new List<string>()
                    { "List", "String", "Color", "Bitmap", "Graphics", "Dictionary", "Math" };
                    break;
                case FileType.JAVA:
                    kw = new List<string>[1];
                    kw[0] = new List<string>()
                    { "abstract", "continue", "for", "new", "switch",
                        "assert", "default", "package", "synchronized",
                        "boolean", "do", "if", "private", "this", "break", "double", "implements", "protected", "throw",
                        "byte", "else", "import", "public", "throws", "case", "enum", "instanceof", "return", "transient",
                        "catch", "extends", "int", "short", "try", "char", "final", "interface", "static", "void", "null", 
                        "class", "finally", "long", "strictfp", "volatile", "float", "native", "super", "while", ";", "," };
                    break;
                case FileType.VB:
                    kw = new List<string>[1];
                    kw[0] = new List<string>()
                    { "AddHandler", "AddressOf", "Alias", "And", "AndAlso", "As", "Boolean", "ByRef", "Byte", "ByVal", "Call", "Case",
                        "Catch", "CBool", "CByte", "CChar", "CDate", "CDbl", "CDec", "Char", "CInt", "Class", "Constraint", "CLng",
                        "CObj", "Const", "Continue", "CSByte", "CShort", "CSng", "CStr", "CType", "CUInt", "CULng", "CUShort", "Date",
                        "Decimal", "Declare", "Default", "Delegate", "Dim", "DirectCast", "Do", "Double", "Each", "Else", "ElseIf", "End",
                        "End", "EndIf", "Enum", "Erase", "Error", "Event", "Exit", "False", "Finally", "For", "For Each", "Friend",
                        "Function", "Get", "GetType", "GetXMLNamespace", "Global", "GoSub", "GoTo", "Handles", "If", "Implements",
                        "Imports", "In", "Inherits", "Integer", "Interface", "Is", "IsNot", "Let", "Lib", "Like", "Long", "Loop", "Me", "Mod",
                        "Module", "MustInherit", "MustOverride", "MyBase", "MyClass", "Namespace", "Narrowing", "New", "Constraint", "Next",
                        "Not", "Nothing", "NotInheritable", "NotOverridable", "Object", "Of", "On", "Operator", "Option", "Optional",
                        "Or", "OrElse", "Out", "Overloads", "Overridable", "Overrides", "ParamArray", "Partial", "Private", "Property",
                        "Protected", "Public", "RaiseEvent", "ReadOnly", "ReDim", "REM", "RemoveHandler", "Resume", "Return", "SByte",
                        "Select", "Set", "Shadows", "Shared", "Short", "Single", "Static", "Step", "Stop", "String", "Structure", "Sub",
                        "SyncLock", "Then", "Throw", "To", "True", "Try", "TryCast", "TypeOf", "UInteger", "ULong", "UShort", "Using",
                        "Variant", "Wend", "When", "While", "Widening", "With", "WithEvents", "WriteOnly", "Xor" };
                    break;
            }
            return kw;
        }
    }
}
