using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("estado_civil")]
    public class EstadoCivil
    {
        [Key]
        [Column("id_estado_civil")]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        public ICollection<Colaborador> Colaboradores { get; set; }
    }
}
