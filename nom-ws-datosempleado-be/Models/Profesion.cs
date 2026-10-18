using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("profesion")]
    public class Profesion
    {
        [Key]
        [Column("id_profesion")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        public ICollection<Colaborador> Colaboradores { get; set; }
    }
}
