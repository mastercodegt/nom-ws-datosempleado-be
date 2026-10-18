GitHub Copilot

README — nom-ws-datosempleado-be
===============================

Resumen
-------
Proyecto ASP.NET Web API que usa Entity Framework Core y Npgsql para Postgres. El DbContext está en Data/ApplicationDbContext.cs y las entidades principales incluyen Usuario, Rol, Permiso, Colaborador, Profesion, EstadoCivil, Modulo y Opcion. las tablas para gestion de acceso se crearon para demostrar como garantizar los permisos y accesos a la plataforma 

Conexión a la base de datos
---------------------------
- El proyecto usa Npgsql (Npgsql.EntityFrameworkCore.PostgreSQL).
- Las cadenas de conexión están en appsettings.json. En este repositorio se usa un túnel SSH opcional (configurado también en appsettings.json) que permite conectar Host=127.0.0.1:5433 hacia la base de datos remota.
- No incluir credenciales en commits; use variables de entorno o Secret Manager para producción.

Ejemplo (sanitizado) de appsettings.json — ConnectionStrings:

{
  "ConnectionStrings": {
	"DefaultConnection": "Host=127.0.0.1;Port=5433;Database=mi_basedatos;Username=mi_usuario;Password=MI_PASSWORD"
  }
}

Cómo se establece la relación Usuario ↔ Rol
- En ApplicationDbContext.OnModelCreating se mapea usuario.id_rol (Propiedad RolId) con FK hacia rol.id_rol.
- EF añade relaciones a partir de las propiedades RolId y de navegación Rol en la clase Usuario.

Versiones utilizadas
--------------------
- .NET: TargetFramework en el proyecto es net10.0 (usar .NET 10 SDK).
  - Paquetes relevantes en el .csproj:
	- Microsoft.EntityFrameworkCore.SqlServer 10.0.13 (presente aunque se usa Postgres)
	- Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3
	- SSH.NET 2026.0.0
- Node.js: No hay package.json ni frontend en este repo; Node.js no es necesario para ejecutar este API. Si existe un frontend separado, indicar la versión allí.

Requisitos previos
------------------
- .NET 10 SDK instalado.
- PostgreSQL compatible (con acceso desde la máquina o mediante túnel SSH).
- (Opcional) Credenciales / permisos en la BD para crear tablas y ejecutar migraciones.
- Herramientas: dotnet CLI; si usa migraciones EF Core: dotnet-ef (dotnet tool install --global dotnet-ef).

Comandos comunes
----------------
- Restaurar y ejecutar:

```
dotnet restore

dotnet build

dotnet run --project nom-ws-datosempleado-be
```
- Ejecutar migraciones (si hay migrations en el proyecto):

```
dotnet ef database update --project nom-ws-datosempleado-be
```

Seguridad y buenas prácticas
----------------------------
- No subir appsettings.json con credenciales al control de versiones.
- Preferir variables de entorno, Azure Key Vault o Secret Manager.
- Al insertar usuarios desde la API, almacenar siempre hashes de contraseña (en el controlador se usa HashSha256 en el código existente — considerar usar bcrypt/Argon2 en producción).

Script SQL (creación de tablas e inserts)
-----------------------------------------
Incluye esquema y datos de ejemplo. Ejecutar en PostgreSQL.

-- ============================================
-- 1. PROFESION
-- ============================================

CREATE TABLE profesion (
	id_profesion BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(100) NOT NULL,
	estado BOOLEAN NOT NULL DEFAULT TRUE,

	CONSTRAINT pk_profesion
		PRIMARY KEY (id_profesion)
);

-- ============================================
-- 2. ESTADO CIVIL
-- ============================================

CREATE TABLE estado_civil (
	id_estado_civil BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(50) NOT NULL,
	estado BOOLEAN NOT NULL DEFAULT TRUE,

	CONSTRAINT pk_estado_civil
		PRIMARY KEY (id_estado_civil)
);

-- ============================================
-- 3. COLABORADOR
-- ============================================

CREATE TABLE colaborador (
	id_colaborador BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
	direccion VARCHAR(255),
	edad INTEGER NOT NULL,
	id_profesion BIGINT NOT NULL,
	id_estado_civil BIGINT NOT NULL,

	CONSTRAINT pk_colaborador
		PRIMARY KEY (id_colaborador),

	CONSTRAINT fk_colaborador_profesion
		FOREIGN KEY (id_profesion)
		REFERENCES profesion(id_profesion),

	CONSTRAINT fk_colaborador_estado_civil
		FOREIGN KEY (id_estado_civil)
		REFERENCES estado_civil(id_estado_civil),

	CONSTRAINT chk_colaborador_edad
		CHECK (edad BETWEEN 0 AND 120)
);

-- ============================================
-- 4. ROL
-- ============================================

CREATE TABLE rol (
	id_rol BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(100) NOT NULL,
	estado BOOLEAN NOT NULL DEFAULT TRUE,

	CONSTRAINT pk_rol
		PRIMARY KEY (id_rol)
);

-- ============================================
-- 5. USUARIO
-- ============================================

CREATE TABLE usuario (
	id_usuario BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
	id_rol BIGINT NOT NULL,
	correo VARCHAR(150) NOT NULL,
	contrasena VARCHAR(255) NOT NULL,
	estado BOOLEAN NOT NULL DEFAULT TRUE,

	CONSTRAINT pk_usuario
		PRIMARY KEY (id_usuario),

	CONSTRAINT fk_usuario_rol
		FOREIGN KEY (id_rol)
		REFERENCES rol(id_rol),

	CONSTRAINT uq_usuario_correo
		UNIQUE (correo)
);

-- ============================================
-- 6. MODULO
-- ============================================

CREATE TABLE modulo (
	id_modulo BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(100) NOT NULL,
	estado BOOLEAN NOT NULL DEFAULT TRUE,

	CONSTRAINT pk_modulo
		PRIMARY KEY (id_modulo)
);

-- ============================================
-- 7. OPCION
-- ============================================

CREATE TABLE opcion (
	id_opcion BIGINT GENERATED ALWAYS AS IDENTITY,
	nombre VARCHAR(100) NOT NULL,
	id_modulo BIGINT NOT NULL,
	estado BOOLEAN NOT NULL DEFAULT TRUE,

	CONSTRAINT pk_opcion
		PRIMARY KEY (id_opcion),

	CONSTRAINT fk_opcion_modulo
		FOREIGN KEY (id_modulo)
		REFERENCES modulo(id_modulo)
);

-- ============================================
-- 8. PERMISO
-- ============================================

CREATE TABLE permiso (
	id_permiso BIGINT GENERATED ALWAYS AS IDENTITY,
	id_rol BIGINT NOT NULL,
	id_opcion BIGINT NOT NULL,

	CONSTRAINT pk_permiso
		PRIMARY KEY (id_permiso),

	CONSTRAINT fk_permiso_rol
		FOREIGN KEY (id_rol)
		REFERENCES rol(id_rol),

	CONSTRAINT fk_permiso_opcion
		FOREIGN KEY (id_opcion)
		REFERENCES opcion(id_opcion),

	CONSTRAINT uq_permiso_rol_opcion
		UNIQUE (id_rol, id_opcion)
);

-- ============================================
-- 9. ÍNDICES
-- ============================================

-- Colaborador
CREATE INDEX idx_colaborador_profesion
ON colaborador(id_profesion);

CREATE INDEX idx_colaborador_estado_civil
ON colaborador(id_estado_civil);

CREATE INDEX idx_colaborador_nombre_apellido
ON colaborador(nombre, apellido);

-- Usuario
CREATE INDEX idx_usuario_rol
ON usuario(id_rol);

CREATE INDEX idx_usuario_nombre_apellido
ON usuario(nombre, apellido);

-- Evitar correos duplicados sin importar mayúsculas
CREATE UNIQUE INDEX uq_usuario_correo_lower
ON usuario(LOWER(correo));

-- Opción
CREATE INDEX idx_opcion_modulo
ON opcion(id_modulo);

-- Permiso
CREATE INDEX idx_permiso_opcion
ON permiso(id_opcion);

-- Inserts de ejemplo
BEGIN;

-- ============================================
-- 1. PROFESIONES
-- ============================================
INSERT INTO profesion (nombre, estado) VALUES
('Ingeniero en Sistemas', TRUE),
('Administrador de Empresas', TRUE),
('Contador Público', TRUE),
('Médico General', TRUE),
('Abogado', TRUE),
('Arquitecto', TRUE),
('Ingeniero Industrial', TRUE),
('Diseñador Gráfico', TRUE),
('Psicólogo', TRUE),
('Técnico en Informática', TRUE);

-- ============================================
-- 2. ESTADOS CIVILES
-- ============================================
INSERT INTO estado_civil (nombre, estado) VALUES
('Soltero', TRUE),
('Casado', TRUE),
('Divorciado', TRUE),
('Viudo', TRUE),
('Unión de hecho', TRUE);

-- ============================================
-- 3. COLABORADORES
-- ============================================
INSERT INTO colaborador
(nombre, apellido, direccion, edad, id_profesion, id_estado_civil)
VALUES
('Carlos', 'Ramírez', 'Zona 1, Ciudad de Guatemala', 28, 1, 1),
('María', 'López', 'Zona 10, Ciudad de Guatemala', 32, 2, 2),
('José', 'Martínez', 'Mixco, Guatemala', 45, 3, 2),
('Ana', 'García', 'Villa Nueva, Guatemala', 26, 4, 1),
('Luis', 'Hernández', 'Zona 15, Ciudad de Guatemala', 38, 5, 3),
('Sofía', 'Morales', 'Antigua Guatemala, Sacatepéquez', 29, 6, 1),
('Pedro', 'Castillo', 'Zona 7, Ciudad de Guatemala', 41, 7, 2),
('Lucía', 'Fernández', 'Zona 14, Ciudad de Guatemala', 25, 8, 1),
('Jorge', 'Pérez', 'Amatitlán, Guatemala', 50, 9, 4),
('Daniela', 'Vásquez', 'Zona 11, Ciudad de Guatemala', 27, 10, 1),
('Roberto', 'Mendoza', 'San Lucas Sacatepéquez', 35, 1, 2),
('Gabriela', 'Ruiz', 'Zona 16, Ciudad de Guatemala', 31, 2, 5),
('Fernando', 'Ortiz', 'Fraijanes, Guatemala', 42, 3, 3),
('Valeria', 'Sánchez', 'Zona 12, Ciudad de Guatemala', 24, 8, 1),
('Andrés', 'Gómez', 'Carretera a El Salvador, Guatemala', 37, 7, 2);

-- ============================================
-- 4. ROLES
-- ============================================
INSERT INTO rol (nombre, estado) VALUES
('Administrador', TRUE),
('Supervisor', TRUE),
('Recursos Humanos', TRUE),
('Colaborador', TRUE),
('Auditor', TRUE);

-- ============================================
-- 5. USUARIOS
-- ============================================
-- Contraseñas ficticias de prueba.
INSERT INTO usuario
(nombre, apellido, id_rol, correo, contrasena, estado)
VALUES
('Carlos', 'Ramírez', 1, 'carlos.ramirez@example.com', 'DEMO_NO_ES_HASH_001', TRUE),
('María', 'López', 2, 'maria.lopez@example.com', 'DEMO_NO_ES_HASH_002', TRUE),
('José', 'Martínez', 3, 'jose.martinez@example.com', 'DEMO_NO_ES_HASH_003', TRUE),
('Ana', 'García', 4, 'ana.garcia@example.com', 'DEMO_NO_ES_HASH_004', TRUE),
('Luis', 'Hernández', 5, 'luis.hernandez@example.com', 'DEMO_NO_ES_HASH_005', TRUE),
('Sofía', 'Morales', 4, 'sofia.morales@example.com', 'DEMO_NO_ES_HASH_006', TRUE),
('Pedro', 'Castillo', 2, 'pedro.castillo@example.com', 'DEMO_NO_ES_HASH_007', TRUE),
('Lucía', 'Fernández', 3, 'lucia.fernandez@example.com', 'DEMO_NO_ES_HASH_008', TRUE),
('Jorge', 'Pérez', 4, 'jorge.perez@example.com', 'DEMO_NO_ES_HASH_009', FALSE),
('Daniela', 'Vásquez', 1, 'daniela.vasquez@example.com', 'DEMO_NO_ES_HASH_010', TRUE);

-- ============================================
-- 6. MODULOS
-- ============================================
INSERT INTO modulo (nombre, estado) VALUES
('Administración', TRUE),
('Recursos Humanos', TRUE),
('Seguridad', TRUE),
('Reportes', TRUE),
('Configuración', TRUE);

-- ============================================
-- 7. OPCIONES
-- ============================================
INSERT INTO opcion (nombre, id_modulo, estado) VALUES
('Crear usuarios', 1, TRUE),
('Editar usuarios', 1, TRUE),
('Eliminar usuarios', 1, TRUE),
('Consultar usuarios', 1, TRUE),
('Crear colaboradores', 2, TRUE),
('Editar colaboradores', 2, TRUE),
('Eliminar colaboradores', 2, TRUE),
('Consultar colaboradores', 2, TRUE),
('Crear roles', 3, TRUE),
('Editar roles', 3, TRUE),
('Asignar permisos', 3, TRUE),
('Consultar permisos', 3, TRUE),
('Generar reportes', 4, TRUE),
('Exportar Excel', 4, TRUE),
('Exportar PDF', 4, TRUE),
('Consultar estadísticas', 4, TRUE),
('Configurar sistema', 5, TRUE),
('Administrar módulos', 5, TRUE),
('Administrar opciones', 5, TRUE),
('Consultar configuración', 5, TRUE);

-- ============================================
-- 8. PERMISOS
-- ============================================
INSERT INTO permiso (id_rol, id_opcion) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(1, 5),
(1, 6),
(1, 7),
(1, 8),
(1, 9),
(1, 10),
(1, 11),
(1, 12),
(1, 13),
(1, 14),
(1, 15),
(1, 16),
(1, 17),
(1, 18),
(1, 19),
(1, 20),
(2, 4),
(2, 6),
(2, 8),
(2, 13),
(2, 14),
(3, 5),
(3, 6),
(3, 7),
(3, 8),
(3, 13);

COMMIT;
