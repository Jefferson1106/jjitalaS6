using System;
using System.Collections.Generic;
using System.Text;

namespace jjitalaS6.Models
{
    public class Estudiante
    {
        public int codigo { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public int edad { get; set; }
    }
}
