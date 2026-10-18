using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("id_rol")]
        public long RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        public Rol Rol { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("correo")]
        public string Correo { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("contrasena")]
        public string Contrasena { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        public ICollection<Permiso> Permisos { get; set; }
    }
}
