-- SQL Queries for CoreAPI Database
-- Database: JwtRoleApiDb
-- Generated based on Entity Framework Core Models

-- ============================================================================
-- TABLE CREATION QUERIES
-- ============================================================================
use JwtRoleApiDb


-- Create AspNetRoles Table
CREATE TABLE [AspNetRoles] (
	[Id] NVARCHAR(450) PRIMARY KEY,
	[Name] NVARCHAR(256) NULL,
	[NormalizedName] NVARCHAR(256) NULL UNIQUE,
	[ConcurrencyStamp] NVARCHAR(MAX) NULL
);

-- Create AspNetUsers Table
CREATE TABLE [AspNetUsers] (
	[Id] NVARCHAR(450) PRIMARY KEY,
	[FullName] NVARCHAR(256) DEFAULT '',
	[UserName] NVARCHAR(256) NULL,
	[NormalizedUserName] NVARCHAR(256) NULL UNIQUE,
	[Email] NVARCHAR(256) NULL,
	[NormalizedEmail] NVARCHAR(256) NULL,
	[EmailConfirmed] BIT DEFAULT 0,
	[PasswordHash] NVARCHAR(MAX) NULL,
	[SecurityStamp] NVARCHAR(MAX) NULL,
	[ConcurrencyStamp] NVARCHAR(MAX) NULL,
	[PhoneNumber] NVARCHAR(MAX) NULL,
	[PhoneNumberConfirmed] BIT DEFAULT 0,
	[TwoFactorEnabled] BIT DEFAULT 0,
	[LockoutEnd] DATETIMEOFFSET NULL,
	[LockoutEnabled] BIT DEFAULT 0,
	[AccessFailedCount] INT DEFAULT 0,
	[CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
	[IsActive] BIT NULL
);

-- Create AspNetUserRoles Table (Many-to-Many)
CREATE TABLE [AspNetUserRoles] (
	[UserId] NVARCHAR(450) NOT NULL,
	[RoleId] NVARCHAR(450) NOT NULL,
	PRIMARY KEY ([UserId], [RoleId]),
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE,
	FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);

-- Create AspNetUserClaims Table
CREATE TABLE [AspNetUserClaims] (
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[UserId] NVARCHAR(450) NOT NULL,
	[ClaimType] NVARCHAR(MAX) NULL,
	[ClaimValue] NVARCHAR(MAX) NULL,
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- Create AspNetUserLogins Table
CREATE TABLE [AspNetUserLogins] (
	[LoginProvider] NVARCHAR(450) NOT NULL,
	[ProviderKey] NVARCHAR(450) NOT NULL,
	[ProviderDisplayName] NVARCHAR(MAX) NULL,
	[UserId] NVARCHAR(450) NOT NULL,
	PRIMARY KEY ([LoginProvider], [ProviderKey]),
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- Create AspNetUserTokens Table
CREATE TABLE [AspNetUserTokens] (
	[UserId] NVARCHAR(450) NOT NULL,
	[LoginProvider] NVARCHAR(450) NOT NULL,
	[Name] NVARCHAR(450) NOT NULL,
	[Value] NVARCHAR(MAX) NULL,
	PRIMARY KEY ([UserId], [LoginProvider], [Name]),
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- Create AspNetRoleClaims Table
CREATE TABLE [AspNetRoleClaims] (
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[RoleId] NVARCHAR(450) NOT NULL,
	[ClaimType] NVARCHAR(MAX) NULL,
	[ClaimValue] NVARCHAR(MAX) NULL,
	FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);

-- Create Policies Table
CREATE TABLE [Policies] (
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(MAX) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[IsActive] BIT DEFAULT 1
);

-- Create PolicyRoles Table (Many-to-Many)
CREATE TABLE [PolicyRoles] (
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[PolicyId] INT NOT NULL,
	[RoleId] NVARCHAR(450) NOT NULL,
	FOREIGN KEY ([PolicyId]) REFERENCES [Policies]([Id]) ON DELETE CASCADE,
	FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);

-- Create RefreshTokens Table
CREATE TABLE [RefreshTokens] (
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[UserId] NVARCHAR(450) NOT NULL,
	[Token] NVARCHAR(MAX) NOT NULL,
	[ExpiresAt] DATETIME2 NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL,
	[RevokedAt] DATETIME2 NULL,
	[CreatedByIp] NVARCHAR(MAX) NULL,
	[ReplacedBy] NVARCHAR(MAX) NULL,
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- Create AuditLogs Table
CREATE TABLE [AuditLogs] (
	[Id] BIGINT PRIMARY KEY IDENTITY(1,1),
	[UserId] NVARCHAR(450) NULL,
	[Action] NVARCHAR(MAX) NOT NULL,
	[IpAddress] NVARCHAR(MAX) NULL,
	[UserAgent] NVARCHAR(MAX) NULL,
	[CreatedAt] DATETIME2 NOT NULL,
	[Details] NVARCHAR(MAX) NULL
);

-- ============================================================================
-- SELECT QUERIES
-- ============================================================================

-- Get all users with their roles
SELECT 
	u.[Id],
	u.[FullName],
	u.[UserName],
	u.[Email],
	u.[CreatedAt],
	u.[IsActive],
	r.[Name] AS [RoleName]
FROM [AspNetUsers] u
LEFT JOIN [AspNetUserRoles] ur ON u.[Id] = ur.[UserId]
LEFT JOIN [AspNetRoles] r ON ur.[RoleId] = r.[Id]
ORDER BY u.[CreatedAt] DESC;

-- Get user by username
SELECT * FROM [AspNetUsers]
WHERE [NormalizedUserName] = UPPER(@NormalizedUserName);

-- Get user by email
SELECT * FROM [AspNetUsers]
WHERE [NormalizedEmail] = UPPER(@NormalizedEmail);

-- Get user by ID with all related data
SELECT 
	u.*
FROM [AspNetUsers] u
WHERE u.[Id] = @UserId;

-- Get all active users
SELECT * FROM [AspNetUsers]
WHERE [IsActive] = 1
ORDER BY [CreatedAt] DESC;

-- Get all roles
SELECT * FROM [AspNetRoles]
ORDER BY [Name] ASC;

-- Get roles for a specific user
SELECT r.* FROM [AspNetRoles] r
INNER JOIN [AspNetUserRoles] ur ON r.[Id] = ur.[RoleId]
WHERE ur.[UserId] = @UserId;

-- Get user claims
SELECT * FROM [AspNetUserClaims]
WHERE [UserId] = @UserId;

-- Get role claims
SELECT * FROM [AspNetRoleClaims]
WHERE [RoleId] = @RoleId;

-- Get active refresh tokens for a user
SELECT * FROM [RefreshTokens]
WHERE [UserId] = @UserId
  AND [RevokedAt] IS NULL
  AND [ExpiresAt] > GETUTCDATE()
ORDER BY [CreatedAt] DESC;

-- Get active policies
SELECT * FROM [Policies]
WHERE [IsActive] = 1
ORDER BY [Name] ASC;

-- Get policies for a specific role
SELECT DISTINCT p.* FROM [Policies] p
INNER JOIN [PolicyRoles] pr ON p.[Id] = pr.[PolicyId]
WHERE pr.[RoleId] = @RoleId
  AND p.[IsActive] = 1;

-- Get all policy-role mappings
SELECT 
	p.[Id],
	p.[Name] AS [PolicyName],
	r.[Id] AS [RoleId],
	r.[Name] AS [RoleName]
FROM [PolicyRoles] pr
INNER JOIN [Policies] p ON pr.[PolicyId] = p.[Id]
INNER JOIN [AspNetRoles] r ON pr.[RoleId] = r.[Id]
ORDER BY p.[Name], r.[Name];

-- Get recent audit logs
SELECT * FROM [AuditLogs]
WHERE [CreatedAt] >= DATEADD(DAY, -7, GETUTCDATE())
ORDER BY [CreatedAt] DESC;

-- Get audit logs for a specific user
SELECT * FROM [AuditLogs]
WHERE [UserId] = @UserId
ORDER BY [CreatedAt] DESC;

-- Get audit logs by action
SELECT * FROM [AuditLogs]
WHERE [Action] LIKE @Action
ORDER BY [CreatedAt] DESC;

-- ============================================================================
-- INSERT QUERIES
-- ============================================================================

-- Insert new user
INSERT INTO [AspNetUsers] (
	[Id], [FullName], [UserName], [NormalizedUserName],
	[Email], [NormalizedEmail], [PasswordHash],
	[SecurityStamp], [ConcurrencyStamp], [EmailConfirmed],
	[PhoneNumberConfirmed], [TwoFactorEnabled],
	[LockoutEnabled], [AccessFailedCount], [IsActive]
)
VALUES (
	@Id, @FullName, @UserName, @NormalizedUserName,
	@Email, @NormalizedEmail, @PasswordHash,
	@SecurityStamp, @ConcurrencyStamp, @EmailConfirmed,
	@PhoneNumberConfirmed, @TwoFactorEnabled,
	@LockoutEnabled, @AccessFailedCount, @IsActive
);

-- Insert new role
INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp);

-- Assign user to role
INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
VALUES (@UserId, @RoleId);

-- Insert user claim
INSERT INTO [AspNetUserClaims] ([UserId], [ClaimType], [ClaimValue])
VALUES (@UserId, @ClaimType, @ClaimValue);

-- Insert policy
INSERT INTO [Policies] ([Name], [Description], [IsActive])
VALUES (@Name, @Description, @IsActive);

-- Assign policy to role
INSERT INTO [PolicyRoles] ([PolicyId], [RoleId])
VALUES (@PolicyId, @RoleId);

-- Insert refresh token
INSERT INTO [RefreshTokens] (
	[UserId], [Token], [ExpiresAt], [CreatedAt],
	[CreatedByIp]
)
VALUES (
	@UserId, @Token, @ExpiresAt, GETUTCDATE(),
	@CreatedByIp
);

-- Insert audit log
INSERT INTO [AuditLogs] (
	[UserId], [Action], [IpAddress], [UserAgent],
	[CreatedAt], [Details]
)
VALUES (
	@UserId, @Action, @IpAddress, @UserAgent,
	GETUTCDATE(), @Details
);

-- ============================================================================
-- UPDATE QUERIES
-- ============================================================================

-- Update user information
UPDATE [AspNetUsers]
SET [FullName] = @FullName,
	[Email] = @Email,
	[NormalizedEmail] = @NormalizedEmail,
	[PhoneNumber] = @PhoneNumber,
	[ConcurrencyStamp] = @ConcurrencyStamp
WHERE [Id] = @UserId;

-- Update user password
UPDATE [AspNetUsers]
SET [PasswordHash] = @PasswordHash,
	[SecurityStamp] = @SecurityStamp,
	[ConcurrencyStamp] = @ConcurrencyStamp
WHERE [Id] = @UserId;

-- Activate user
UPDATE [AspNetUsers]
SET [IsActive] = 1
WHERE [Id] = @UserId;

-- Deactivate user
UPDATE [AspNetUsers]
SET [IsActive] = 0
WHERE [Id] = @UserId;

-- Update user lockout
UPDATE [AspNetUsers]
SET [LockoutEnd] = @LockoutEnd,
	[AccessFailedCount] = @AccessFailedCount
WHERE [Id] = @UserId;

-- Confirm user email
UPDATE [AspNetUsers]
SET [EmailConfirmed] = 1
WHERE [Id] = @UserId;

-- Update role name
UPDATE [AspNetRoles]
SET [Name] = @Name,
	[NormalizedName] = @NormalizedName,
	[ConcurrencyStamp] = @ConcurrencyStamp
WHERE [Id] = @RoleId;

-- Update policy
UPDATE [Policies]
SET [Name] = @Name,
	[Description] = @Description,
	[IsActive] = @IsActive
WHERE [Id] = @PolicyId;

-- Revoke refresh token
UPDATE [RefreshTokens]
SET [RevokedAt] = GETUTCDATE()
WHERE [Id] = @TokenId;

-- ============================================================================
-- DELETE QUERIES
-- ============================================================================

-- Delete user (cascades to related tables)
DELETE FROM [AspNetUsers]
WHERE [Id] = @UserId;

-- Delete role (cascades to related tables)
DELETE FROM [AspNetRoles]
WHERE [Id] = @RoleId;

-- Remove user from role
DELETE FROM [AspNetUserRoles]
WHERE [UserId] = @UserId AND [RoleId] = @RoleId;

-- Remove all roles from user
DELETE FROM [AspNetUserRoles]
WHERE [UserId] = @UserId;

-- Delete user claim
DELETE FROM [AspNetUserClaims]
WHERE [Id] = @ClaimId;

-- Delete all user claims
DELETE FROM [AspNetUserClaims]
WHERE [UserId] = @UserId;

-- Delete policy
DELETE FROM [Policies]
WHERE [Id] = @PolicyId;

-- Remove expired refresh tokens
DELETE FROM [RefreshTokens]
WHERE [ExpiresAt] < GETUTCDATE()
  AND [RevokedAt] IS NULL;

-- Delete old audit logs (older than 90 days)
DELETE FROM [AuditLogs]
WHERE [CreatedAt] < DATEADD(DAY, -90, GETUTCDATE());

-- ============================================================================
-- STATISTICS & AGGREGATE QUERIES
-- ============================================================================

-- Get user count
SELECT COUNT(*) AS [TotalUsers] FROM [AspNetUsers];

-- Get active user count
SELECT COUNT(*) AS [ActiveUsers]
FROM [AspNetUsers]
WHERE [IsActive] = 1;

-- Get role count
SELECT COUNT(*) AS [TotalRoles] FROM [AspNetRoles];

-- Users per role
SELECT 
	r.[Name] AS [RoleName],
	COUNT(ur.[UserId]) AS [UserCount]
FROM [AspNetRoles] r
LEFT JOIN [AspNetUserRoles] ur ON r.[Id] = ur.[RoleId]
GROUP BY r.[Id], r.[Name]
ORDER BY [UserCount] DESC;

-- Active refresh tokens by user
SELECT 
	u.[UserName],
	COUNT(rt.[Id]) AS [ActiveTokenCount]
FROM [AspNetUsers] u
LEFT JOIN [RefreshTokens] rt ON u.[Id] = rt.[UserId]
WHERE rt.[RevokedAt] IS NULL
  AND rt.[ExpiresAt] > GETUTCDATE()
GROUP BY u.[Id], u.[UserName]
HAVING COUNT(rt.[Id]) > 0
ORDER BY [ActiveTokenCount] DESC;

-- Audit logs summary
SELECT 
	[Action],
	COUNT(*) AS [Count],
	MAX([CreatedAt]) AS [LastOccurrence]
FROM [AuditLogs]
WHERE [CreatedAt] >= DATEADD(DAY, -30, GETUTCDATE())
GROUP BY [Action]
ORDER BY [Count] DESC;

-- User login attempts (audit logs)
SELECT 
	[UserId],
	[IpAddress],
	COUNT(*) AS [AttemptCount],
	MAX([CreatedAt]) AS [LastAttempt]
FROM [AuditLogs]
WHERE [Action] LIKE '%Login%'
  AND [CreatedAt] >= DATEADD(DAY, -7, GETUTCDATE())
GROUP BY [UserId], [IpAddress]
ORDER BY [AttemptCount] DESC;

-- ============================================================================
-- MAINTENANCE QUERIES
-- ============================================================================

-- Rebuild indexes
ALTER INDEX ALL ON [AspNetUsers] REBUILD;
ALTER INDEX ALL ON [AspNetRoles] REBUILD;
ALTER INDEX ALL ON [RefreshTokens] REBUILD;
ALTER INDEX ALL ON [AuditLogs] REBUILD;

-- Update statistics
UPDATE STATISTICS [AspNetUsers];
UPDATE STATISTICS [AspNetRoles];
UPDATE STATISTICS [AspNetUserRoles];
UPDATE STATISTICS [RefreshTokens];
UPDATE STATISTICS [AuditLogs];
UPDATE STATISTICS [Policies];

-- Check database integrity
DBCC CHECKDB (JwtRoleApiDb);

-- ============================================================================
-- SAMPLE DATA QUERIES
-- ============================================================================

-- Insert sample roles
INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName])
VALUES 
	(NEWID(), 'Admin', 'ADMIN'),
	(NEWID(), 'User', 'USER'),
	(NEWID(), 'Moderator', 'MODERATOR');

-- Insert sample policies
INSERT INTO [Policies] ([Name], [Description], [IsActive])
VALUES 
	('View Dashboard', 'Permission to view dashboard', 1),
	('Manage Users', 'Permission to manage users', 1),
	('View Reports', 'Permission to view reports', 1);
