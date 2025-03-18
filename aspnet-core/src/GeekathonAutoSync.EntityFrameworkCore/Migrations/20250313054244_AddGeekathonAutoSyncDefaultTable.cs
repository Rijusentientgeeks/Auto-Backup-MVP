using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekathonAutoSync.Migrations
{
    /// <inheritdoc />
    public partial class AddGeekathonAutoSyncDefaultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackupFrequencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupFrequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackUPTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackUPTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloudStorages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DBTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageMasterTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageMasterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackUpStorageConfiguations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    StorageMasterTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CloudStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NFS_IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NFS_AccessUserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NFS_Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NFS_LocationPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AWS_AccessKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AWS_SecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AWS_BucketName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AWS_Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AWS_backUpPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AZ_AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AZ_AccountKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackUpStorageConfiguations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackUpStorageConfiguations_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackUpStorageConfiguations_CloudStorages_CloudStorageId",
                        column: x => x.CloudStorageId,
                        principalTable: "CloudStorages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BackUpStorageConfiguations_StorageMasterTypes_StorageMasterTypeId",
                        column: x => x.StorageMasterTypeId,
                        principalTable: "StorageMasterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceConfiguations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    BackUPTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DBTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServerIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DBInitialCatalog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrivateKeyPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackUpInitiatedPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sourcepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackUpStorageConfiguationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceConfiguations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceConfiguations_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SourceConfiguations_BackUPTypes_BackUPTypeId",
                        column: x => x.BackUPTypeId,
                        principalTable: "BackUPTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SourceConfiguations_BackUpStorageConfiguations_BackUpStorageConfiguationId",
                        column: x => x.BackUpStorageConfiguationId,
                        principalTable: "BackUpStorageConfiguations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SourceConfiguations_DBTypes_DBTypeId",
                        column: x => x.DBTypeId,
                        principalTable: "DBTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BackUpLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    SourceConfiguationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BackupLogStatus = table.Column<int>(type: "int", nullable: true),
                    BackUpStorageConfiguationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackUpLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackUpLogs_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackUpLogs_BackUpStorageConfiguations_BackUpStorageConfiguationId",
                        column: x => x.BackUpStorageConfiguationId,
                        principalTable: "BackUpStorageConfiguations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BackUpLogs_SourceConfiguations_SourceConfiguationId",
                        column: x => x.SourceConfiguationId,
                        principalTable: "SourceConfiguations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BackUpSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    SourceConfiguationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BackupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BackupTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    BackUpFrequencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackUpSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackUpSchedules_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackUpSchedules_BackupFrequencies_BackUpFrequencyId",
                        column: x => x.BackUpFrequencyId,
                        principalTable: "BackupFrequencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BackUpSchedules_SourceConfiguations_SourceConfiguationId",
                        column: x => x.SourceConfiguationId,
                        principalTable: "SourceConfiguations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackUpLogs_BackUpStorageConfiguationId",
                table: "BackUpLogs",
                column: "BackUpStorageConfiguationId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpLogs_SourceConfiguationId",
                table: "BackUpLogs",
                column: "SourceConfiguationId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpLogs_TenantId",
                table: "BackUpLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpSchedules_BackUpFrequencyId",
                table: "BackUpSchedules",
                column: "BackUpFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpSchedules_SourceConfiguationId",
                table: "BackUpSchedules",
                column: "SourceConfiguationId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpSchedules_TenantId",
                table: "BackUpSchedules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpStorageConfiguations_CloudStorageId",
                table: "BackUpStorageConfiguations",
                column: "CloudStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpStorageConfiguations_StorageMasterTypeId",
                table: "BackUpStorageConfiguations",
                column: "StorageMasterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BackUpStorageConfiguations_TenantId",
                table: "BackUpStorageConfiguations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceConfiguations_BackUpStorageConfiguationId",
                table: "SourceConfiguations",
                column: "BackUpStorageConfiguationId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceConfiguations_BackUPTypeId",
                table: "SourceConfiguations",
                column: "BackUPTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceConfiguations_DBTypeId",
                table: "SourceConfiguations",
                column: "DBTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceConfiguations_TenantId",
                table: "SourceConfiguations",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackUpLogs");

            migrationBuilder.DropTable(
                name: "BackUpSchedules");

            migrationBuilder.DropTable(
                name: "BackupFrequencies");

            migrationBuilder.DropTable(
                name: "SourceConfiguations");

            migrationBuilder.DropTable(
                name: "BackUPTypes");

            migrationBuilder.DropTable(
                name: "BackUpStorageConfiguations");

            migrationBuilder.DropTable(
                name: "DBTypes");

            migrationBuilder.DropTable(
                name: "CloudStorages");

            migrationBuilder.DropTable(
                name: "StorageMasterTypes");
        }
    }
}
