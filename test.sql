CREATE TABLE Rates (
    Id INTEGER PRIMARY KEY,
    Code CHAR(3) NOT NULL,
    Rate REAL NOT NULL
);

INSERT INTO Rates (Id, Code, Rate) VALUES
(1, 'USD', 1.0),
(2, 'PHP', 43.1232),
(3, 'MXN', 18.4);
