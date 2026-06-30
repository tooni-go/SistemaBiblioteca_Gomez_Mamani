PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS TipoSocio (
    IdTipoSocio INTEGER PRIMARY KEY,
    Descripcion TEXT NOT NULL,
    MaxLibrosSimultaneos INTEGER NOT NULL,
    DiasPrestamo INTEGER NOT NULL,
    MultaPorDia REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS Estado (
    IdEstado INTEGER PRIMARY KEY,
    Descripcion TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Libro (
    ISBN TEXT PRIMARY KEY,
    Titulo TEXT NOT NULL,
    Autor TEXT NOT NULL,
    Genero TEXT NOT NULL,
    CantidadCopias INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS Socio (
    NroSocio INTEGER PRIMARY KEY,
    Nombre TEXT NOT NULL,
    Apellido TEXT NOT NULL,
    Email TEXT NOT NULL,
    IdTipoSocio INTEGER NOT NULL,
    Activo INTEGER NOT NULL,
    FOREIGN KEY (IdTipoSocio) REFERENCES TipoSocio (IdTipoSocio)
);

CREATE TABLE IF NOT EXISTS Prestamo (
    IdPrestamo INTEGER PRIMARY KEY AUTOINCREMENT,
    NroSocio INTEGER NOT NULL,
    ISBN TEXT NOT NULL,
    FechaPrestamo TEXT NOT NULL,
    FechaVencimiento TEXT NOT NULL,
    FechaDevolucion TEXT,
    IdEstado INTEGER NOT NULL,
    FOREIGN KEY (NroSocio) REFERENCES Socio (NroSocio),
    FOREIGN KEY (ISBN) REFERENCES Libro (ISBN),
    FOREIGN KEY (IdEstado) REFERENCES Estado (IdEstado)
);

CREATE TABLE IF NOT EXISTS Reserva (
    IdReserva INTEGER PRIMARY KEY AUTOINCREMENT,
    NroSocio INTEGER NOT NULL,
    ISBN TEXT NOT NULL,
    FechaReserva TEXT NOT NULL,
    IdEstado INTEGER NOT NULL,
    FOREIGN KEY (NroSocio) REFERENCES Socio (NroSocio),
    FOREIGN KEY (ISBN) REFERENCES Libro (ISBN),
    FOREIGN KEY (IdEstado) REFERENCES Estado (IdEstado)
);

CREATE TABLE IF NOT EXISTS Multa (
    IdMulta INTEGER PRIMARY KEY AUTOINCREMENT,
    NroSocio INTEGER NOT NULL,
    IdPrestamo INTEGER NOT NULL,
    Monto REAL NOT NULL,
    DiasDemora INTEGER NOT NULL,
    FechaGeneracion TEXT NOT NULL,
    Abonada INTEGER NOT NULL,
    FOREIGN KEY (NroSocio) REFERENCES Socio (NroSocio),
    FOREIGN KEY (IdPrestamo) REFERENCES Prestamo (IdPrestamo)
);

INSERT INTO TipoSocio (IdTipoSocio, Descripcion, MaxLibrosSimultaneos, DiasPrestamo, MultaPorDia) VALUES
    (1, 'Común', 3, 7, 150),
    (2, 'Estudiante', 5, 14, 75),
    (3, 'Docente', 8, 30, 50);

INSERT INTO Estado (IdEstado, Descripcion) VALUES
    (1, 'Activo'),
    (2, 'Devuelto'),
    (3, 'Vencido'),
    (4, 'Pendiente'),
    (5, 'Cumplida'),
    (6, 'Cancelada');

INSERT INTO Libro (ISBN, Titulo, Autor, Genero, CantidadCopias) VALUES
    ('978-0141439518', 'Orgullo y prejuicio', 'Jane Austen', 'Ficción', 4),
    ('978-8497594628', 'Cien años de soledad', 'Gabriel García Márquez', 'Ficción', 3),
    ('978-9875665673', 'El aleph', 'Jorge Luis Borges', 'Ficción', 2),
    ('978-8491050691', 'Breve historia del tiempo', 'Stephen Hawking', 'Ciencia', 5),
    ('978-9500305334', 'Sapiens', 'Yuval Noah Harari', 'Historia', 3),
    ('978-8498386964', 'Don Quijote de la Mancha', 'Miguel de Cervantes', 'Ficción', 2);

INSERT INTO Socio (NroSocio, Nombre, Apellido, Email, IdTipoSocio, Activo) VALUES
    (1001, 'Ana', 'Gómez', 'ana.gomez@email.com', 1, 1),
    (1002, 'Lucas', 'Mamani', 'lucas.mamani@email.com', 2, 1),
    (1003, 'María', 'Fernández', 'maria.fernandez@email.com', 3, 1),
    (1004, 'Carlos', 'Ruiz', 'carlos.ruiz@email.com', 1, 1),
    (1005, 'Sofía', 'López', 'sofia.lopez@email.com', 2, 0),
    (1006, 'Diego', 'Pérez', 'diego.perez@email.com', 3, 1);

INSERT INTO Prestamo (NroSocio, ISBN, FechaPrestamo, FechaVencimiento, FechaDevolucion, IdEstado) VALUES
    (1001, '978-0141439518', '2026-06-20', '2026-06-27', NULL, 1),
    (1002, '978-8497594628', '2026-06-15', '2026-06-29', NULL, 1),
    (1003, '978-9875665673', '2026-06-01', '2026-07-01', NULL, 1);

INSERT INTO Prestamo (NroSocio, ISBN, FechaPrestamo, FechaVencimiento, FechaDevolucion, IdEstado) VALUES
    (1004, '978-8491050691', '2026-05-01', '2026-05-08', NULL, 3),
    (1002, '978-9500305334', '2026-05-10', '2026-05-24', NULL, 3);

INSERT INTO Prestamo (NroSocio, ISBN, FechaPrestamo, FechaVencimiento, FechaDevolucion, IdEstado) VALUES
    (1001, '978-8498386964', '2026-04-01', '2026-04-08', '2026-04-07', 2),
    (1006, '978-0141439518', '2026-03-10', '2026-04-09', '2026-04-05', 2);

INSERT INTO Reserva (NroSocio, ISBN, FechaReserva, IdEstado) VALUES
    (1006, '978-9875665673', '2026-06-18', 4),
    (1004, '978-8497594628', '2026-06-22', 4);

INSERT INTO Multa (NroSocio, IdPrestamo, Monto, DiasDemora, FechaGeneracion, Abonada) VALUES
    (1004, 4, 7800, 52, '2026-05-09', 0);
