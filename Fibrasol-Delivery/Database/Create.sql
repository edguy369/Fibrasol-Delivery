-- ---------------------------------------------------
--  .NET Core Identity v.4 Required Tables 
-- ---------------------------------------------------
CREATE TABLE `Users` (
    `Id` VARCHAR(10) NOT NULL,
    `UserName` VARCHAR(100) DEFAULT NULL,
    `NormalizedUserName` VARCHAR(100) DEFAULT NULL,
    `Email` VARCHAR(100) DEFAULT NULL,
    `NormalizedEmail` VARCHAR(100) DEFAULT NULL,
    `EmailConfirmed` BIT(1) NOT NULL,
    `PasswordHash` VARCHAR(500) DEFAULT NULL,
    `SecurityStamp` VARCHAR(50) DEFAULT NULL,
    `ConcurrencyStamp` VARCHAR(50) DEFAULT NULL,
    `PhoneNumber` VARCHAR(10) DEFAULT NULL,
    `PhoneNumberConfirmed` BIT(1) NOT NULL,
    `TwoFactorEnabled` BIT(1) NOT NULL,
    `LockoutEnd` TIMESTAMP(6) NULL DEFAULT NULL,
    `LockoutEnabled` BIT(1) NOT NULL,
    `AccessFailedCount` INT NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `Users` (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES('c5ba2f50', 'root@codingtipi.com', 'ROOT@CODINGTIPI.COM', 'root@codingtipi.com', 'ROOT@CODINGTIPI.COM', 1, 'AQAAAAEAACcQAAAAEAfds+tdeFMLAC95x0bwcTzpe0Esnox9r+8tErks/rvCr67cPM5XNPfmZmXdMpwejg==', '73544099-51fe-4c98-95ee-d771000a22fd', '73544099-51fe-4c98-95ee-d771000a22fd', 0, 0, 0, 0);

CREATE TABLE `Roles` (
    `Id` VARCHAR(10) NOT NULL,
    `Name` VARCHAR(50) DEFAULT NULL,
    `NormalizedName` VARCHAR(50) DEFAULT NULL,
    `ConcurrencyStamp` VARCHAR(50) DEFAULT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `Roles` (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES('5836155d', 'Admin', 'ADMIN', 'bbe1da74-74f6-4f83-80a2-299c8f43663c');

INSERT INTO `Roles` (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES('ff94b31a', 'User', 'USER', '3d2ebb4b-8f63-4913-bcc9-3d5247f2cea1');

CREATE TABLE `UserClaims` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` VARCHAR(10) NOT NULL,
    `ClaimType` VARCHAR(250) DEFAULT NULL,
    `ClaimValue` VARCHAR(250) DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `FK_UserClaims_Users_UserId` (`UserId`),
    CONSTRAINT `FK_UserClaims_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `RoleClaims` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `RoleId` VARCHAR(10) NOT NULL,
    `ClaimType` VARCHAR(25) DEFAULT NULL,
    `ClaimValue` VARCHAR(25) DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `FK_RoleClaims_Roles_RoleId` (`RoleId`),
    CONSTRAINT `FK_RoleClaims_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserLogins` (
    `LoginProvider` VARCHAR(25) NOT NULL,
    `ProviderKey` VARCHAR(25) NOT NULL,
    `ProviderDisplayName` VARCHAR(25) DEFAULT NULL,
    `UserId` VARCHAR(10) NOT NULL,
    PRIMARY KEY (`LoginProvider`,`ProviderKey`),
    KEY `FK_UserLogins_Users_UserId` (`UserId`),
    CONSTRAINT `FK_UserLogins_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserRoles` (
    `UserId` VARCHAR(10) NOT NULL,
    `RoleId` VARCHAR(10) NOT NULL,
    PRIMARY KEY (`UserId`,`RoleId`),
    KEY `FK_UserRoles_Roles_RoleId` (`RoleId`),
    CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `UserRoles` (UserId, RoleId) VALUES ('c5ba2f50', '5836155d');

CREATE TABLE `UserTokens` (
    `UserId` VARCHAR(10) NOT NULL,
    `LoginProvider` VARCHAR(128) NOT NULL,
    `Name` VARCHAR(128) NOT NULL,
    `Value` VARCHAR(128) DEFAULT NULL,
    PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
    CONSTRAINT `FK_UserTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*CLIENTES*/
CREATE TABLE `Clients` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(150) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;