-- Jobito Database Setup Script
-- Generated manually from EF Core Snapshot

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

-- 1. Create AspNetUsers
IF OBJECT_ID(N'[AspNetUsers]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(255) NOT NULL,
        [Role] nvarchar(max) NOT NULL,
        [Classification] nvarchar(100) NULL,
        [Location] nvarchar(max) NULL,
        [AvatarUrl] nvarchar(max) NULL,
        [BannerUrl] nvarchar(max) NULL,
        [Bio] nvarchar(max) NULL,
        [Dob] nvarchar(max) NULL,
        [Gender] nvarchar(max) NULL,
        [SocialLinksJson] nvarchar(max) NULL,
        [SkillsJson] nvarchar(max) NULL,
        [ExperiencesJson] nvarchar(max) NULL,
        [EducationsJson] nvarchar(max) NULL,
        [PortfoliosJson] nvarchar(max) NULL,
        [ServicesJson] nvarchar(max) NULL,
        [NotificationPreferences] nvarchar(max) NOT NULL DEFAULT N'{}',
        [LanguagePreference] nvarchar(10) NOT NULL DEFAULT N'ar',
        [ThemePreference] nvarchar(10) NOT NULL DEFAULT N'light',
        [IsActive] bit NOT NULL DEFAULT 0,
        [IsPhoneVerified] bit NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [DeletionRequestedAt] datetime2 NULL,
        [GoogleId] nvarchar(255) NULL,
        [RegistrationData] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        [ServiceRadiusKm] int NOT NULL DEFAULT 0,
        [Latitude] decimal(10, 7) NULL,
        [Longitude] decimal(10, 7) NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END

-- 2. Create AspNetRoles
IF OBJECT_ID(N'[AspNetRoles]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END

-- 3. Create Companies
IF OBJECT_ID(N'[Companies]', 'U') IS NULL
BEGIN
    CREATE TABLE [Companies] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Industry] nvarchar(100) NULL,
        [Description] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [Website] nvarchar(255) NULL,
        [ContactEmail] nvarchar(255) NULL,
        [Phone] nvarchar(50) NULL,
        [LogoUrl] nvarchar(max) NULL,
        [VerificationStatus] nvarchar(50) NOT NULL DEFAULT N'Pending',
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [Classification] nvarchar(100) NULL,
        [FoundedYear] nvarchar(50) NULL,
        [FoundedMonth] nvarchar(50) NULL,
        [FoundedDay] nvarchar(50) NULL,
        [Employees] nvarchar(50) NULL,
        [TaxId] nvarchar(50) NULL,
        [LicenseNumber] nvarchar(100) NULL,
        [OfficialNationalId] nvarchar(50) NULL,
        [CrDocumentUrl] nvarchar(max) NULL,
        [SocialLinks] nvarchar(max) NULL,
        [Benefits] nvarchar(max) NULL,
        [TechStack] nvarchar(max) NULL,
        [LocationTags] nvarchar(max) NULL,
        [OfficePhoto1Url] nvarchar(max) NULL,
        [OfficePhoto2Url] nvarchar(max) NULL,
        [RejectionReason] nvarchar(max) NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
    );
END

-- 4. Create Categories
IF OBJECT_ID(N'[Categories]', 'U') IS NULL
BEGIN
    CREATE TABLE [Categories] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [NameEn] nvarchar(150) NULL,
        [Description] nvarchar(max) NULL,
        [DescriptionEn] nvarchar(max) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END

-- 5. Create Jobs
IF OBJECT_ID(N'[Jobs]', 'U') IS NULL
BEGIN
    CREATE TABLE [Jobs] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [Title] nvarchar(255) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Classification] nvarchar(50) NULL,
        [Address] nvarchar(max) NULL,
        [Salary] decimal(10, 2) NULL,
        [SalaryMin] decimal(10, 2) NULL,
        [SalaryMax] decimal(10, 2) NULL,
        [PriceType] nvarchar(50) NOT NULL DEFAULT N'Fixed',
        [IsNegotiable] bit NOT NULL DEFAULT 0,
        [WorkTime] nvarchar(max) NULL,
        [WorkLocationType] nvarchar(max) NULL,
        [JobType] nvarchar(max) NULL,
        [FieldOfWork] nvarchar(max) NULL,
        [Skills] nvarchar(max) NULL,
        [Images] nvarchar(max) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [SlotsAvailable] int NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [ExpiresAt] datetime2 NULL,
        [CompanyId] bigint NULL,
        [UserId] nvarchar(450) NULL,
        [CategoryId] bigint NULL,
        [Latitude] decimal(10, 7) NULL,
        [Longitude] decimal(10, 7) NULL,
        CONSTRAINT [PK_Jobs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Jobs_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Jobs_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Jobs_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL
    );
END

-- 6. Create JobApplications
IF OBJECT_ID(N'[JobApplications]', 'U') IS NULL
BEGIN
    CREATE TABLE [JobApplications] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [JobId] bigint NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Status] nvarchar(50) NOT NULL DEFAULT N'Applied',
        [AppliedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [ResumeUrl] nvarchar(max) NULL,
        [CoverLetter] nvarchar(max) NULL,
        [PortfolioUrl] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        CONSTRAINT [PK_JobApplications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JobApplications_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_JobApplications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END

-- 7. Create Favorites
IF OBJECT_ID(N'[Favorites]', 'U') IS NULL
BEGIN
    CREATE TABLE [Favorites] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [JobId] bigint NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Favorites] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Favorites_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Favorites_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([Id]) ON DELETE CASCADE
    );
END

-- 8. Create Identity tables
IF OBJECT_ID(N'[AspNetUserClaims]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END

IF OBJECT_ID(N'[AspNetUserLogins]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END

IF OBJECT_ID(N'[AspNetUserRoles]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END

IF OBJECT_ID(N'[AspNetUserTokens]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END

IF OBJECT_ID(N'[AspNetRoleClaims]', 'U') IS NULL
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END

-- 9. Create HelpCategories & HelpArticles
IF OBJECT_ID(N'[HelpCategories]', 'U') IS NULL
BEGIN
    CREATE TABLE [HelpCategories] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [NameEn] nvarchar(100) NULL,
        [Icon] nvarchar(50) NULL,
        CONSTRAINT [PK_HelpCategories] PRIMARY KEY ([Id])
    );
END

IF OBJECT_ID(N'[HelpArticles]', 'U') IS NULL
BEGIN
    CREATE TABLE [HelpArticles] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [Title] nvarchar(255) NOT NULL,
        [TitleEn] nvarchar(255) NULL,
        [Content] nvarchar(max) NOT NULL,
        [ContentEn] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [IsHelpfulYes] int NOT NULL DEFAULT 0,
        [IsHelpfulNo] int NOT NULL DEFAULT 0,
        [CategoryId] bigint NOT NULL,
        CONSTRAINT [PK_HelpArticles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HelpArticles_HelpCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [HelpCategories] ([Id]) ON DELETE CASCADE
    );
END

-- 10. Create Ratings & Testimonials
IF OBJECT_ID(N'[Ratings]', 'U') IS NULL
BEGIN
    CREATE TABLE [Ratings] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [RatingValue] smallint NOT NULL,
        [Comment] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [RaterType] nvarchar(20) NOT NULL,
        [CompanyId] bigint NULL,
        [UserId] nvarchar(450) NULL,
        CONSTRAINT [PK_Ratings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Ratings_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]),
        CONSTRAINT [FK_Ratings_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
    );
END

IF OBJECT_ID(N'[Testimonials]', 'U') IS NULL
BEGIN
    CREATE TABLE [Testimonials] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Body] nvarchar(max) NOT NULL,
        [BodyEn] nvarchar(max) NULL,
        [IsFeatured] bit NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Testimonials] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Testimonials_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END

-- 11. Create DailyStats & JobUpdates
IF OBJECT_ID(N'[DailyStats]', 'U') IS NULL
BEGIN
    CREATE TABLE [DailyStats] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CompanyId] bigint NOT NULL,
        [Date] datetime2 NOT NULL,
        [JobsViewed] int NOT NULL DEFAULT 0,
        [JobsApplied] int NOT NULL DEFAULT 0,
        [ViewsOrganic] int NOT NULL DEFAULT 0,
        [ViewsDirect] int NOT NULL DEFAULT 0,
        [ViewsSocial] int NOT NULL DEFAULT 0,
        [ViewsOther] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_DailyStats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DailyStats_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
    );
END

IF OBJECT_ID(N'[JobUpdates]', 'U') IS NULL
BEGIN
    CREATE TABLE [JobUpdates] (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [CompanyId] bigint NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_JobUpdates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JobUpdates_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
    );
END

-- 12. Create JobCategories (Join Table)
IF OBJECT_ID(N'[JobCategories]', 'U') IS NULL
BEGIN
    CREATE TABLE [JobCategories] (
        [CategoriesId] bigint NOT NULL,
        [JobsManyId] bigint NOT NULL,
        CONSTRAINT [PK_JobCategories] PRIMARY KEY ([CategoriesId], [JobsManyId]),
        CONSTRAINT [FK_JobCategories_Categories_CategoriesId] FOREIGN KEY ([CategoriesId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_JobCategories_Jobs_JobsManyId] FOREIGN KEY ([JobsManyId]) REFERENCES [Jobs] ([Id]) ON DELETE CASCADE
    );
END

-- Indexes with Existence Checks
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'EmailIndex' AND object_id = OBJECT_ID('[AspNetUsers]'))
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UserNameIndex' AND object_id = OBJECT_ID('[AspNetUsers]'))
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Jobs_CompanyId' AND object_id = OBJECT_ID('[Jobs]'))
BEGIN
    CREATE INDEX [IX_Jobs_CompanyId] ON [Jobs] ([CompanyId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Jobs_UserId' AND object_id = OBJECT_ID('[Jobs]'))
BEGIN
    CREATE INDEX [IX_Jobs_UserId] ON [Jobs] ([UserId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_JobApplications_JobId' AND object_id = OBJECT_ID('[JobApplications]'))
BEGIN
    CREATE INDEX [IX_JobApplications_JobId] ON [JobApplications] ([JobId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_JobApplications_UserId_JobId' AND object_id = OBJECT_ID('[JobApplications]'))
BEGIN
    CREATE UNIQUE INDEX [IX_JobApplications_UserId_JobId] ON [JobApplications] ([UserId], [JobId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Favorites_JobId' AND object_id = OBJECT_ID('[Favorites]'))
BEGIN
    CREATE INDEX [IX_Favorites_JobId] ON [Favorites] ([JobId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Favorites_UserId_JobId' AND object_id = OBJECT_ID('[Favorites]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Favorites_UserId_JobId] ON [Favorites] ([UserId], [JobId]);
END

PRINT 'Database Setup Script Completed Successfully.';
