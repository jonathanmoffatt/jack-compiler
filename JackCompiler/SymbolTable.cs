using System;
using System.Collections.Generic;
using System.Linq;

namespace JackCompiler
{
    public class SymbolTable
    {
        private List<SymbolLookup> classSymbols;
        private List<SymbolLookup> subroutineSymbols;
        private bool isMethod;

        public static bool IsClassOrSubroutine(IdentifierKind kind)
        {
            return kind == IdentifierKind.Class || kind == IdentifierKind.Subroutine;
        }

        public static bool IsFieldOrStatic(IdentifierKind kind)
        {
            return kind == IdentifierKind.Field || kind == IdentifierKind.Static;
        }

        public static bool IsArgumentOrVar(IdentifierKind kind)
        {
            return kind == IdentifierKind.Argument || kind == IdentifierKind.Var;
        }

        public SymbolTable()
        {
            classSymbols = new List<SymbolLookup>();
            subroutineSymbols = new List<SymbolLookup>();
        }

        public void ResetSubroutineTable(bool isMethod)
        {
            subroutineSymbols = new List<SymbolLookup>();
            this.isMethod = isMethod;
        }

        public SymbolLookup Add(string name, IdentifierKind kind, string classType)
        {
            SymbolLookup symbol = null;
            if (IsFieldOrStatic(kind))
            {
                int number = classSymbols.Count(s => s.Kind == kind);
                symbol = new SymbolLookup { Name = name, Kind = kind, Number = number, ClassType = classType };
                classSymbols.Add(symbol);
            }
            if (IsArgumentOrVar(kind))
            {
                int number = subroutineSymbols.Count(s => s.Kind == kind);
                if (isMethod && kind == IdentifierKind.Argument) number++;
                symbol = new SymbolLookup { Name = name, Kind = kind, Number = number, ClassType = classType };
                subroutineSymbols.Add(symbol);
            }
            return symbol;
        }

        public SymbolLookup Get(string name)
        {
            SymbolLookup subroutineSymbol = subroutineSymbols.SingleOrDefault(s => s.Name == name);
            return subroutineSymbol ?? classSymbols.SingleOrDefault(s => s.Name == name);
        }
    }

    public class SymbolLookup
    {
        public string Name { get; set; }
        public IdentifierKind Kind { get; set; }
        public int Number { get; set; }
        public string ClassType { get; set; }
    }


}
