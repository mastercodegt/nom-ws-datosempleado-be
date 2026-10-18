using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("rol")]
    public class Rol
    {
        [Key]
        [Column("id_rol")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        // Hacer las colecciones anulables para que no sean consideradas "required"
        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<Permiso>? Permisos { get; set; }
    }
}
