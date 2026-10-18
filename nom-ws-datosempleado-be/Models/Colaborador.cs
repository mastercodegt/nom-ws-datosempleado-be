using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nom_ws_datosempleado_be.Models
{
    [Table("colaborador")]
    public class Colaborador
    {
        [Key]
        [Column("id_colaborador")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("apellido")]
        public string Apellido { get; set; }

        [MaxLength(255)]
        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("edad")]
        public int Edad { get; set; }

        [Column("id_profesion")]
        public long ProfesionId { get; set; }

        [ForeignKey(nameof(ProfesionId))]
        public Profesion Profesion { get; set; }

        [Column("id_estado_civil")]
        public long EstadoCivilId { get; set; }

        [ForeignKey(nameof(EstadoCivilId))]
        public EstadoCivil EstadoCivil { get; set; }
    }
}
