-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-07-2025 a las 13:09:08
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `turismotriunvirato`
--

DELIMITER $$
--
-- Procedimientos
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetActiveCarouselItems` ()   BEGIN
    SELECT
        Id,
        ImageUrl,
        AltText,
        Title,
        LinkUrl,
        IsActive,
        CreatedDate,
        UpdatedDate
    FROM
        View_DestinationCarouselItem
    WHERE
        IsActive = TRUE;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `GetActiveHotDestiny` ()   BEGIN
SELECT
        Id,
        Title,
        PictureDestiny,
        Price,
        `From`, -- Usar comillas invertidas si 'From' es palabra reservada
        DetailDestinyURL,
        IsHotWeek,
        TripType -- ¡ASEGÚRATE DE INCLUIR ESTA COLUMNA!
    FROM
        view_destination
    WHERE
        IsHotWeek = TRUE;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `user`
--

CREATE TABLE `user` (
  `Id` int(11) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Surname` varchar(100) NOT NULL,
  `Country` varchar(100) DEFAULT NULL,
  `Rol` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `view_destination`
--

CREATE TABLE `view_destination` (
  `Id` int(11) NOT NULL,
  `Title` varchar(255) NOT NULL,
  `PictureDestiny` varchar(255) NOT NULL,
  `Price` decimal(10,2) NOT NULL,
  `From` varchar(255) NOT NULL,
  `DetailDestinyURL` varchar(255) NOT NULL,
  `IsHotWeek` tinyint(1) NOT NULL,
  `TripType` varchar(50) NOT NULL DEFAULT 'IDA Y VUELTA'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `view_destination`
--

INSERT INTO `view_destination` (`Id`, `Title`, `PictureDestiny`, `Price`, `From`, `DetailDestinyURL`, `IsHotWeek`, `TripType`) VALUES
(1, 'Cataratas de Iguazu', '/img/CatarataIguazu.png', 410000.00, 'Desde Buenos Aires', '/Destinos/CataratasIguazu', 1, 'IDA Y VUELTA'),
(2, 'Bariloche', '/img/Bariloche.png', 580000.00, 'Desde Buenos Aires', '/Destinos/Bariloche', 1, 'IDA Y VUELTA'),
(3, 'Camboriú Brasil', '/img/Camboriu.png', 720000.00, 'Desde Buenos Aires', '/Destinos/CamboriuBrasil', 1, 'IDA Y VUELTA');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `view_destinationcarouselitem`
--

CREATE TABLE `view_destinationcarouselitem` (
  `Id` int(11) NOT NULL,
  `ImageUrl` varchar(255) DEFAULT NULL,
  `AltText` varchar(255) DEFAULT NULL,
  `Title` varchar(255) NOT NULL,
  `LinkUrl` varchar(255) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedDate` datetime DEFAULT current_timestamp(),
  `UpdatedDate` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `view_destinationcarouselitem`
--

INSERT INTO `view_destinationcarouselitem` (`Id`, `ImageUrl`, `AltText`, `Title`, `LinkUrl`, `IsActive`, `CreatedDate`, `UpdatedDate`) VALUES
(1, '/img/Destino1.jpg', 'Cordoba', 'Córdoba', '/destinos/cordoba', 1, '2025-07-24 23:42:21', '2025-07-25 05:07:08'),
(2, '/img/Destino2.jpg', 'Viñedos Mendocinos', 'Mendoza', '/destinos/mendoza', 1, '2025-07-24 23:42:21', '2025-07-25 05:07:13');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `view_destination`
--
ALTER TABLE `view_destination`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `view_destinationcarouselitem`
--
ALTER TABLE `view_destinationcarouselitem`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `user`
--
ALTER TABLE `user`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `view_destination`
--
ALTER TABLE `view_destination`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `view_destinationcarouselitem`
--
ALTER TABLE `view_destinationcarouselitem`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
