using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaAdoExamen
{
    public class Cliente
    {
        public string id { get; set; }
        public string empresa { get; set; }
        public string contacto { get; set; }
        public string cargo { get; set; }
        public string ciudad { get; set; }
        public int telefono { get; set; }
        public List<string> pedidos { get; set; }
    }
}
