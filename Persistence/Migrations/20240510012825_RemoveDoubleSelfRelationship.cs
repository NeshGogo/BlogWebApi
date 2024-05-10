using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDoubleSelfRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_AspNetUsers_FollowerUserId1",
                schema: "BlogPost",
                table: "UserFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_AspNetUsers_FollowingUserId1",
                schema: "BlogPost",
                table: "UserFollowings");

            migrationBuilder.DropIndex(
                name: "IX_UserFollowings_FollowingUserId1",
                schema: "BlogPost",
                table: "UserFollowings");

            migrationBuilder.DropIndex(
                name: "IX_UserFollowers_FollowerUserId1",
                schema: "BlogPost",
                table: "UserFollowers");

            migrationBuilder.DropColumn(
                name: "FollowingUserId1",
                schema: "BlogPost",
                table: "UserFollowings");

            migrationBuilder.DropColumn(
                name: "FollowerUserId1",
                schema: "BlogPost",
                table: "UserFollowers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FollowingUserId1",
                schema: "BlogPost",
                table: "UserFollowings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FollowerUserId1",
                schema: "BlogPost",
                table: "UserFollowers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowings_FollowingUserId1",
                schema: "BlogPost",
                table: "UserFollowings",
                column: "FollowingUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_FollowerUserId1",
                schema: "BlogPost",
                table: "UserFollowers",
                column: "FollowerUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_AspNetUsers_FollowerUserId1",
                schema: "BlogPost",
                table: "UserFollowers",
                column: "FollowerUserId1",
                principalSchema: "BlogPost",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_AspNetUsers_FollowingUserId1",
                schema: "BlogPost",
                table: "UserFollowings",
                column: "FollowingUserId1",
                principalSchema: "BlogPost",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
