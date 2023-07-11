using System.Collections.Generic;

namespace AzureFunctionsNH.Models
{
    public class Variable
    {
        public static bool isListaAvailable;
        public static List<Variable> variableList = new List<Variable>();

        public int idLine;
        public string fieldName;
        public string tagName;
        public int type;
    }
}