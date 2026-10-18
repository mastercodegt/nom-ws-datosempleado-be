using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("modulo")]
    public class Modulo
    {
        [Key]
        [Column("id_modulo")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        public ICollection<Opcion> Opciones { get; set; }
    }
}
