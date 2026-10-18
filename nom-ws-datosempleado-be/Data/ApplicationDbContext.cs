using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using nom_ws_datosempleado_be.Models;

namespace nom_ws_datosempleado_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Profesion> Profesiones { get; set; }
        public DbSet<EstadoCivil> EstadosCiviles { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Opcion> Opciones { get; set; }
        public DbSet<Permiso> Permisos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profesion>(entity =>
            {
                entity.ToTable("profesion");
                entity.HasKey(e => e.Id).HasName("pk_profesion");
                entity.Property(e => e.Id).HasColumnName("id_profesion");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();
            });

            modelBuilder.Entity<EstadoCivil>(entity =>
            {
                entity.ToTable("estado_civil");
                entity.HasKey(e => e.Id).HasName("pk_estado_civil");
                entity.Property(e => e.Id).HasColumnName("id_estado_civil");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();
            });

            modelBuilder.Entity<Colaborador>(entity =>
            {
                entity.ToTable("colaborador");
                entity.HasKey(e => e.Id).HasName("pk_colaborador");
                entity.Property(e => e.Id).HasColumnName("id_colaborador");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Apellido).HasColumnName("apellido").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(255);
                entity.Property(e => e.Edad).HasColumnName("edad").IsRequired();
                entity.Property(e => e.ProfesionId).HasColumnName("id_profesion").IsRequired();
                entity.Property(e => e.EstadoCivilId).HasColumnName("id_estado_civil").IsRequired();

                entity.HasOne(e => e.Profesion)
                    .WithMany(p => p.Colaboradores)
                    .HasForeignKey(e => e.ProfesionId)
                    .HasConstraintName("fk_colaborador_profesion");

                entity.HasOne(e => e.EstadoCivil)
                    .WithMany(ec => ec.Colaboradores)
                    .HasForeignKey(e => e.EstadoCivilId)
                    .HasConstraintName("fk_colaborador_estado_civil");

                entity.HasCheckConstraint("chk_colaborador_edad", "edad BETWEEN 0 AND 120");

                entity.HasIndex(e => new { e.Nombre, e.Apellido }).HasDatabaseName("idx_colaborador_nombre_apellido");
                entity.HasIndex(e => e.ProfesionId).HasDatabaseName("idx_colaborador_profesion");
                entity.HasIndex(e => e.EstadoCivilId).HasDatabaseName("idx_colaborador_estado_civil");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("rol");
                entity.HasKey(e => e.Id).HasName("pk_rol");
                entity.Property(e => e.Id).HasColumnName("id_rol");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");
                entity.HasKey(e => e.Id).HasName("pk_usuario");
                entity.Property(e => e.Id).HasColumnName("id_usuario");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Apellido).HasColumnName("apellido").HasMaxLength(100).IsRequired();
                entity.Property(e => e.RolId).HasColumnName("id_rol").IsRequired();
                entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
                entity.Property(e => e.Contrasena).HasColumnName("contrasena").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.RolId)
                    .HasConstraintName("fk_usuario_rol");

                entity.HasIndex(e => e.RolId).HasDatabaseName("idx_usuario_rol");
                entity.HasIndex(e => new { e.Nombre, e.Apellido }).HasDatabaseName("idx_usuario_nombre_apellido");
                entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("uq_usuario_correo");
            });

            modelBuilder.Entity<Modulo>(entity =>
            {
                entity.ToTable("modulo");
                entity.HasKey(e => e.Id).HasName("pk_modulo");
                entity.Property(e => e.Id).HasColumnName("id_modulo");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();
            });

            modelBuilder.Entity<Opcion>(entity =>
            {
                entity.ToTable("opcion");
                entity.HasKey(e => e.Id).HasName("pk_opcion");
                entity.Property(e => e.Id).HasColumnName("id_opcion");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ModuloId).HasColumnName("id_modulo").IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();

                entity.HasOne(e => e.Modulo)
                    .WithMany(m => m.Opciones)
                    .HasForeignKey(e => e.ModuloId)
                    .HasConstraintName("fk_opcion_modulo");

                entity.HasIndex(e => e.ModuloId).HasDatabaseName("idx_opcion_modulo");
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.ToTable("permiso");
                entity.HasKey(e => e.Id).HasName("pk_permiso");
                entity.Property(e => e.Id).HasColumnName("id_permiso");
                entity.Property(e => e.RolId).HasColumnName("id_rol").IsRequired();
                entity.Property(e => e.OpcionId).HasColumnName("id_opcion").IsRequired();

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Permisos)
                    .HasForeignKey(e => e.RolId)
                    .HasConstraintName("fk_permiso_rol");

                entity.HasOne(e => e.Opcion)
                    .WithMany(o => o.Permisos)
                    .HasForeignKey(e => e.OpcionId)
                    .HasConstraintName("fk_permiso_opcion");

                entity.HasIndex(e => e.OpcionId).HasDatabaseName("idx_permiso_opcion");
                entity.HasIndex(e => new { e.RolId, e.OpcionId }).IsUnique().HasDatabaseName("uq_permiso_rol_opcion");
            });
        }
    }
}
