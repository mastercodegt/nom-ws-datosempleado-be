using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("permiso")]
    public class Permiso
    {
        [Key]
        [Column("id_permiso")]
        public long Id { get; set; }

        [Column("id_rol")]
        public long RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        public Rol Rol { get; set; }

        [Column("id_opcion")]
        public long OpcionId { get; set; }

        [ForeignKey(nameof(OpcionId))]
        public Opcion Opcion { get; set; }
    }
}
