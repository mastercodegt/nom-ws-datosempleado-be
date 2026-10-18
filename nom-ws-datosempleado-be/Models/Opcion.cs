using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("opcion")]
    public class Opcion
    {
        [Key]
        [Column("id_opcion")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("id_modulo")]
        public long ModuloId { get; set; }

        [ForeignKey(nameof(ModuloId))]
        public Modulo Modulo { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        public ICollection<Permiso> Permisos { get; set; }
    }
}
